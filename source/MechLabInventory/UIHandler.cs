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
using Object = UnityEngine.Object;

namespace CustomFilters.MechLabInventory;

internal static class UIHandler
{
    private static List<HBSDOTweenToggle> tabs;
    private static List<CustomButtonInfo> buttons;
    private static MechLabPanel mechLab;
    private static HBSRadioSet _tabRadioSet;

    public static MechLabInventoryWidget Widget => mechLab.inventoryWidget;
    private static SVGCache IconCache => mechLab.dataManager.SVGCache;

    private static TabInfo _currentTab;
    private static ButtonInfo _currentButton;

    public static void Init()
    {
        Widget.filterBtnAll.SetActive(false);
        Widget.filterBtnEquipmentHeatsink.SetActive(false);
        Widget.filterBtnEquipmentJumpjet.SetActive(false);
        Widget.filterBtnEquipmentUpgrade.SetActive(false);

        Widget.filterBtnWeaponEnergy.SetActive(false);
        Widget.filterBtnWeaponMissile.SetActive(false);
        Widget.filterBtnWeaponBallistic.SetActive(false);
        Widget.filterBtnWeaponSmall.SetActive(false);

        _tabRadioSet.defaultButton = tabs.FirstOrDefault();
        _tabRadioSet.Reset();

        TabPressed(Control.Settings.Tabs.FirstOrDefault());
    }

    public static void PreInit(MechLabPanel mechLabPanel)
    {
        if (tabs == null)
        {
            Logging.LogDebug("No tabs found - create new");
            tabs = new();
            buttons = new();

            mechLab = mechLabPanel;
            InitTabs();
        }
        else if (mechLab != mechLabPanel)
        {
            Logging.LogDebug("other mechlab widget, droping");
            foreach (var toggle in tabs)
            {
                if (toggle != null)
                    toggle.gameObject.Destroy();
            }
            tabs.Clear();
            foreach (var b in buttons)
            {
                if (b.Go != null)
                    b.Go.Destroy();
            }
            buttons.Clear();

            mechLab = mechLabPanel;
            InitTabs();
        }
        else
        {
            Logging.LogDebug("already modified");
        }
    }

    private static void InitTabs()
    {
        Logging.LogDebug("-- hide old tabs");
        Widget.tabAllToggleObj.gameObject.SetActive(false);
        Widget.tabAmmoToggleObj.gameObject.SetActive(false);
        Widget.tabEquipmentToggleObj.gameObject.SetActive(false);
        Widget.tabMechPartToggleObj.gameObject.SetActive(false);
        Widget.tabWeaponsToggleObj.gameObject.SetActive(false);

        // ReSharper disable once Unity.InefficientPropertyAccess
        var go = Widget.tabWeaponsToggleObj.gameObject;
        _tabRadioSet = go.transform.parent.GetComponent<HBSRadioSet>();
        _tabRadioSet.ClearRadioButtons();


        foreach (var settingsTab in Control.Settings.Tabs)
        {
            Logging.LogDebug($"--- create tab [{settingsTab.caption}]");

            var tab = Object.Instantiate(Widget.tabWeaponsToggleObj.gameObject, go.transform.parent);
            tab.transform.position = go.transform.position;
            tab.transform.localScale = Vector3.one;
            var radio = tab.GetComponent<HBSDOTweenToggle>();

            radio.OnClicked.RemoveAllListeners();
            radio.OnClicked.AddListener(() => TabPressed(settingsTab));

            tab.SetActive(true);
            tabs.Add(radio);
            var text = tab.GetComponentInChildren<TextMeshProUGUI>(true);
            if (text != null)
                text.SetText(settingsTab.caption);
            _tabRadioSet.RadioButtons.Add(radio);
        }

        Logging.LogDebug($"-- create small buttons");

        go = Widget.filterBtnAll;

        var grid = go.transform.parent.gameObject.GetComponent<GridLayoutGroup>();
        grid.spacing = new(8,8);

        ShowChilds(go, "");

        Widget.filterRadioSet.ClearRadioButtons();
        for (var i = 0; i < 14; i++)
        {
            Logging.LogDebug($"--- Create Button #{i}");
            try
            {
                var info = new CustomButtonInfo(go, i, FilterPressed);
                buttons.Add(info);
                Widget.filterRadioSet.AddButtonToRadioSet(info.Toggle);
            }
            catch (Exception e)
            {
                Logging.LogError(e.Message + " " + e.StackTrace);
            }
        }

        Widget.filterRadioSet.defaultButton = buttons.FirstOrDefault()?.Toggle;
        Widget.filterRadioSet.Reset();
    }

    private static void ShowChilds(GameObject go, string prefix)
    {
        Logging.LogDebug(prefix + " " + go.name);
        foreach (Transform child in go.transform)
        {
            ShowChilds(child.gameObject, prefix + "-");
        }
    }

    private static void FilterPressed(int num)
    {
        Logging.LogDebug($"PRESSED [{num}]");

        if (_currentTab?.buttons == null || _currentTab.buttons.Length <= num)
            return;

        _currentButton = _currentTab.buttons[num];

        MechLabFixPublic.FilterChanged();
    }

    private static void TabPressed(TabInfo settingsTab)
    {
        Logging.LogDebug($"PRESSED [{settingsTab.caption}]");
        foreach (var buttonInfo in buttons)
        {
            buttonInfo.Go.SetActive(false);
        }

        _currentTab = settingsTab;

        if (settingsTab?.buttons == null || settingsTab.buttons.Length == 0)
            return;

        for (var i = 0; i < 14 && i < settingsTab.buttons.Length; i++)
        {
            Logging.LogDebug($"- button {i}");

            var buttonInfo = settingsTab.buttons[i];
            var button = buttons[i];
            if (!string.IsNullOrEmpty(buttonInfo.text))
            {
                Logging.LogDebug($"-- set text");
                button.Text.text = buttonInfo.text;
                button.GoText.SetActive(true);
            }
            else
            {
                button.GoText.SetActive(false);
            }


            if (!string.IsNullOrEmpty(buttonInfo.icon))
            {
                Logging.LogDebug($"-- set icon");
                button.Icon.vectorGraphics = IconCache.GetAsset(buttonInfo.icon);
                button.GoIcon.SetActive(true);
                if (button.Icon.vectorGraphics == null)
                {
                    Logging.LogError($"Icon {buttonInfo.icon} not found, replacing with ???");
                    button.Text.text = "???";
                    button.GoText.SetActive(true);
                }
            }
            else
            {
                button.GoIcon.SetActive(false);
            }

            if (!string.IsNullOrEmpty(buttonInfo.tag))
            {
                Logging.LogDebug($"- set tag");
                button.Tag.text = buttonInfo.tag;
                button.GoTag.SetActive(true);
            }
            else
            {
                button.GoTag.SetActive(false);
            }

            if (!string.IsNullOrEmpty(buttonInfo.tooltip))
            {
                var state = new HBSTooltipStateData();
                state.SetString(buttonInfo.tooltip);

                button.Tooltip.SetDefaultStateData(state);

            }
            else
            {
                var state = new HBSTooltipStateData();
                state.SetDisabled();
                button.Tooltip.SetDefaultStateData(state);
            }

            button.Go.SetActive(!buttonInfo.debug || Control.Settings.ShowDebugButtons);
        }
        Widget.filterRadioSet.Reset();
        FilterPressed(0);
    }

    public static bool ApplyFilter(MechComponentDef item)
    {
        // ReSharper disable once Unity.NoNullPropagation
        if (mechLab?.activeMechDef == null)
            return true;

        if (item == null)
        {
            Logging.LogError($"-- ITEM IS NULL!");
            return false;
        }
        //Control.LogDebug($"- {item.Description.Id}");

        if (item.Flags<CCFlags>().HideFromInv)
        {
            //Control.LogDebug($"-- default");
            return false;
        }

        if (!ApplyFilter(item, _currentTab?.filter))
        {
            //Control.LogDebug($"-- tab filter miss");
            return false;
        }

        if (!ApplyFilter(item, _currentButton?.filter))
        {
            //Control.LogDebug($"-- button filter miss");
            return false;
        }

        foreach (var filter in item.GetComponents<IMechLabFilter>())
        {
            try
            {
                if (!filter.CheckFilter(mechLab))
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Logging.LogError("Error in filter", e);
            }
        }

        if (item.ComponentType == ComponentType.JumpJet)
        {
            if (item is JumpJetDef jj)
            {
                if (mechLab.activeMechDef.Chassis.Tonnage < jj.MinTonnage)
                    return false;
                if (mechLab.activeMechDef.Chassis.Tonnage > jj.MaxTonnage)
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

        if (filter.componentTypes.HasAny() && !filter.componentTypes.Contains(item.ComponentType))
        {
            //Control.LogDebug($"--- not component type");
            return false;
        }

        if (item.ComponentType == ComponentType.Weapon)
        {
            if (item is not WeaponDef weaponDef)
            {
                Logging.LogError($"{item.Description.Id} of type {item.ComponentType} is actually not of type {typeof(WeaponDef)}");
                return false;
            }

            if (filter.weaponCategories.HasAny() && !filter.weaponCategories.Contains(weaponDef.WeaponCategoryValue.Name))
            {
                return false;
            }

            if (filter.uiLookAndColorIcons.HasAny() && !filter.uiLookAndColorIcons.Contains(weaponDef.weaponCategoryValue.Icon))
            {
                return false;
            }
        }

        if (item.ComponentType == ComponentType.AmmunitionBox)
        {
            if (item is not AmmunitionBoxDef boxDef)
            {
                Logging.LogError($"{item.Description.Id} of type {item.ComponentType} is actually not of type {typeof(AmmunitionBoxDef)}");
                return false;
            }

            if (filter.ammoCategories.HasAny() && !filter.ammoCategories.Contains(boxDef.Ammo.AmmoCategoryValue.Name))
            {
                return false;
            }

            if (filter.uiLookAndColorIcons.HasAny() && !filter.uiLookAndColorIcons.Contains(boxDef.Ammo.AmmoCategoryValue.Icon))
            {
                return false;
            }
        }

        return (!filter.categories.HasAny() || filter.categories.Any(item.IsCategory)) && (!filter.notCategories.HasAny() || !filter.notCategories.Any(item.IsCategory));
    }

    public static void CallFilter()
    {
        Widget.ApplyFiltering();
    }

    private static bool HasAny<T>(this IReadOnlyCollection<T> array)
    {
        return array != null && array.Count > 0;
    }
}