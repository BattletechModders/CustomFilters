using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using BattletechPerformanceFix.MechLabFix;
using CustomComponents;
using FluffyUnderware.DevTools.Extensions;
using SVGImporter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomFilters;

internal static class UIHandler
{
    private class button_info
    {
        public GameObject go;
        public HBSDOTweenToggle toggle;

        public HBSTooltip tooltip;

        public GameObject go_icon;
        public SVGImage icon;

        public GameObject go_text;
        public TextMeshProUGUI text;

        public GameObject go_tag;
        public TextMeshProUGUI tag;
    }

    private static List<HBSDOTweenToggle> tabs;
    private static List<button_info> buttons;
    private static MechLabPanel mechlab;
    private static HBSRadioSet tab_radioset;

    public static MechLabInventoryWidget widget => mechlab.inventoryWidget;
    private static SVGCache icon_cache => mechlab.dataManager.SVGCache;

    private static TabInfo current_tab;
    private static ButtonInfo current_button;

    public static void Init()
    {
        widget.filterBtnAll.SetActive(false);
        widget.filterBtnEquipmentHeatsink.SetActive(false);
        widget.filterBtnEquipmentJumpjet.SetActive(false);
        widget.filterBtnEquipmentUpgrade.SetActive(false);

        widget.filterBtnWeaponEnergy.SetActive(false);
        widget.filterBtnWeaponMissile.SetActive(false);
        widget.filterBtnWeaponBallistic.SetActive(false);
        widget.filterBtnWeaponSmall.SetActive(false);

        tab_radioset.defaultButton = tabs.FirstOrDefault();
        tab_radioset.Reset();

        TabPressed(Control.Settings.Tabs.FirstOrDefault());
    }

    public static void PreInit(MechLabPanel mechLabPanel)
    {
        if (tabs == null)
        {
            Control.LogDebug("No tabs found - create new");
            tabs = new List<HBSDOTweenToggle>();
            buttons = new List<button_info>();

            mechlab = mechLabPanel;
            InitTabs();
        }
        else if (mechlab != mechLabPanel)
        {
            Control.LogDebug("other mechlab widget, droping");
            foreach (var toggle in tabs)
            {
                if (toggle != null)
                    toggle.gameObject.Destroy();
            }
            tabs.Clear();
            foreach (var b in buttons)
            {
                if (b.go != null)
                    b.go.Destroy();
            }
            buttons.Clear();

            mechlab = mechLabPanel;
            InitTabs();
        }
        else
        {
            Control.LogDebug("already modified");
        }
    }

    private static void InitTabs()
    {
        Control.LogDebug("-- hide old tabs");
        widget.tabAllToggleObj.gameObject.SetActive(false);
        widget.tabAmmoToggleObj.gameObject.SetActive(false);
        widget.tabEquipmentToggleObj.gameObject.SetActive(false);
        widget.tabMechPartToggleObj.gameObject.SetActive(false);
        widget.tabWeaponsToggleObj.gameObject.SetActive(false);

        var go = widget.tabWeaponsToggleObj.gameObject;
        tab_radioset = go.transform.parent.GetComponent<HBSRadioSet>();
        tab_radioset.ClearRadioButtons();


        foreach (var settingsTab in Control.Settings.Tabs)
        {
            Control.LogDebug($"--- create tab [{settingsTab.Caption}]");

            var tab = GameObject.Instantiate(widget.tabWeaponsToggleObj.gameObject);
            tab.SetActive(true);
            tab.transform.position = go.transform.position;
            tab.transform.SetParent(go.transform.parent);
            tab.transform.localScale = Vector3.one;
            var radio = tab.GetComponent<HBSDOTweenToggle>();

            radio.OnClicked.RemoveAllListeners();
            radio.OnClicked.AddListener(() => TabPressed(settingsTab));


            tabs.Add(radio);
            var text = tab.GetComponentInChildren<TextMeshProUGUI>(true);
            if (text != null)
                text.SetText(settingsTab.Caption);
            tab_radioset.RadioButtons.Add(radio);
        }

        Control.LogDebug($"-- create small buttons");

        go = widget.filterBtnAll;

        var grid = go.transform.parent.gameObject.GetComponent<GridLayoutGroup>();
        grid.spacing = new Vector2(8,8);

        ShowChilds(go, "");

        widget.filterRadioSet.ClearRadioButtons();
        for (int i = 0; i < 14; i++)
        {
            Control.LogDebug($"--- Create Button #{i}");
            try
            {
                GameObject button = GameObject.Instantiate(go);
                var info = new button_info();
                info.go = button;


                info.go_icon = button.transform.Find("bttn-bal/bg_fill/bttn_icon").gameObject;
                info.icon = info.go_icon.gameObject.GetComponent<SVGImage>();
                info.tooltip = button.GetComponentInChildren<HBSTooltip>();

                info.go_text = button.transform.Find("bttn-bal/bg_fill/bttn_text").gameObject;
                info.text = info.go_text.gameObject.GetComponent<TextMeshProUGUI>();

                info.go_tag = button.transform.Find("bttn-bal/bg_fill/numberLabel-optional").gameObject;
                info.tag = info.go_tag.GetComponentInChildren<TextMeshProUGUI>();

                info.toggle = button.GetComponentInChildren<HBSDOTweenToggle>();

                button.SetActive(true);
                button.transform.SetParent(go.transform.parent);
                button.transform.localScale = Vector3.one;
                info.toggle.OnClicked.RemoveAllListeners();

                var num = i;
                info.toggle.OnClicked.AddListener(() => FilterPressed(num));

                info.go_tag.SetActive(true);
                info.tag.text = $"# {i}";

                buttons.Add(info);
                widget.filterRadioSet.AddButtonToRadioSet(info.toggle);
            }
            catch (Exception e)
            {
                Control.LogError(e.Message + " " + e.StackTrace);
            }
        }

        widget.filterRadioSet.defaultButton = buttons.FirstOrDefault()?.toggle;
        widget.filterRadioSet.Reset();
    }

    private static void ShowChilds(GameObject go, string prefix)
    {
        Control.LogDebug(prefix + " " + go.name);
        foreach (Transform child in go.transform)
        {
            ShowChilds(child.gameObject, prefix + "-");
        }
    }

    private static void FilterPressed(int num)
    {
        Control.LogDebug($"PRESSED [{num}]");

        if (current_tab?.Buttons == null || current_tab.Buttons.Length <= num)
            return;

        current_button = current_tab.Buttons[num];

        MechLabFixPublic.FilterChanged();
    }

    private static void TabPressed(TabInfo settingsTab)
    {
        Control.LogDebug($"PRESSED [{settingsTab.Caption}]");
        foreach (var buttonInfo in buttons)
        {
            buttonInfo.go.SetActive(false);
        }

        current_tab = settingsTab;

        if (settingsTab?.Buttons == null || settingsTab.Buttons.Length == 0)
            return;

        for (int i = 0; i < 14 && i < settingsTab.Buttons.Length; i++)
        {
            Control.LogDebug($"- button {i}");

            var bdef = settingsTab.Buttons[i];
            var button = buttons[i];
            if (!string.IsNullOrEmpty(bdef.Text))
            {
                Control.LogDebug($"-- set text");
                button.text.text = bdef.Text;
                button.go_text.SetActive(true);
            }
            else
            {
                button.go_text.SetActive(false);
            }


            if (!string.IsNullOrEmpty(bdef.Icon))
            {
                Control.LogDebug($"-- set icon");
                button.icon.vectorGraphics = icon_cache.GetAsset(bdef.Icon);
                button.go_icon.SetActive(true);
                if (button.icon.vectorGraphics == null)
                {
                    Control.LogError($"Icon {bdef.Icon} not found, replacing with ???");
                    button.text.text = "???";
                    button.go_text.SetActive(true);
                }
            }
            else
            {
                button.go_icon.SetActive(false);
            }

            if (!string.IsNullOrEmpty(bdef.Tag))
            {
                Control.LogDebug($"- set tag");
                button.tag.text = bdef.Tag;
                button.go_tag.SetActive(true);
            }
            else
            {
                button.go_tag.SetActive(false);
            }

            if (!string.IsNullOrEmpty(bdef.Tooltip))
            {
                var state = new HBSTooltipStateData();
                state.SetString(bdef.Tooltip);

                button.tooltip.SetDefaultStateData(state);

            }
            else
            {
                var state = new HBSTooltipStateData();
                state.SetDisabled();
                button.tooltip.SetDefaultStateData(state);
            }

            button.go.SetActive(!bdef.Debug || Control.Settings.ShowDebugButtons);
        }
        widget.filterRadioSet.Reset();
        FilterPressed(0);
    }

    public static bool ApplyFilter(MechComponentDef item)
    {
        if (mechlab?.activeMechDef == null)
            return true;

        if (item == null)
        {
            Control.LogError($"-- ITEM IS NULL!");
            return false;
        }
        //Control.LogDebug($"- {item.Description.Id}");

        if (item.Flags<CCFlags>().HideFromInv)
        {
            //Control.LogDebug($"-- default");
            return false;
        }

        bool black = !Control.Settings.ShowBlacklistedSkirmish && mechlab.sim == null ||
                     !Control.Settings.ShowBlacklistedSimGame && mechlab.sim != null;

        if (black && item.ComponentTags.Contains("BLACKLISTED"))
        {
            return false;
        }


        if (!ApplyFilter(item, current_tab?.Filter))
        {
            //Control.LogDebug($"-- tab filter miss");
            return false;
        }

        if (!ApplyFilter(item, current_button?.Filter))
        {
            //Control.LogDebug($"-- button filter miss");
            return false;
        }

        foreach (var filter in item.GetComponents<IMechLabFilter>())
        {
            try
            {
                if (!filter.CheckFilter(mechlab))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Control.LogError("Error in filter", e);
            }
        }

        if (item.ComponentType == ComponentType.JumpJet)
        {
            if (item is JumpJetDef jj)
            {
                if (mechlab.activeMechDef.Chassis.Tonnage < jj.MinTonnage)
                    return false;
                if (mechlab.activeMechDef.Chassis.Tonnage > jj.MaxTonnage)
                    return false;
            }
        }


        return true;
    }

    private static bool ApplyFilter(MechComponentDef item, FilterInfo filter)
    {

        if (filter == null)
        {
            //Control.LogDebug($"--- empty filter");
            return true;
        }

        if (filter.ComponentTypes.HasAny() && !filter.ComponentTypes.Contains(item.ComponentType))
        {
            //Control.LogDebug($"--- not component type");
            return false;
        }

        if (item.ComponentType == ComponentType.Weapon)
        {
            if (item is not WeaponDef weaponDef)
            {
                Control.LogError($"{item.Description.Id} of type {item.ComponentType} is actually not of type {typeof(WeaponDef)}");
                return false;
            }

            if (filter.WeaponCategories.HasAny() && !filter.WeaponCategories.Contains(weaponDef.WeaponCategoryValue.Name))
            {
                return false;
            }

            if (filter.UILookAndColorIcons.HasAny() && !filter.UILookAndColorIcons.Contains(weaponDef.weaponCategoryValue.Icon))
            {
                return false;
            }
        }

        if (item.ComponentType == ComponentType.AmmunitionBox)
        {
            if (item is not AmmunitionBoxDef boxDef)
            {
                Control.LogError($"{item.Description.Id} of type {item.ComponentType} is actually not of type {typeof(AmmunitionBoxDef)}");
                return false;
            }

            if (filter.AmmoCategories.HasAny() && !filter.AmmoCategories.Contains(boxDef.Ammo.AmmoCategoryValue.Name))
            {
                return false;
            }

            if (filter.UILookAndColorIcons.HasAny() && !filter.UILookAndColorIcons.Contains(boxDef.Ammo.AmmoCategoryValue.Icon))
            {
                return false;
            }
        }

        return (!filter.Categories.HasAny() || filter.Categories.Any(item.IsCategory)) && (!filter.NotCategories.HasAny() || !filter.NotCategories.Any(item.IsCategory));
    }

    public static void CallFilter()
    {
        widget.ApplyFiltering();
    }

    private static bool HasAny<T>(this IReadOnlyCollection<T> array)
    {
        return array != null && array.Count > 0;
    }
}