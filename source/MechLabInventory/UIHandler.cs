using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomFilters.TabConfig;
using FluffyUnderware.DevTools.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CustomFilters.MechLabInventory;

internal static class UIHandler
{
    // compatibility
    internal static Func<MechComponentDef, bool>? CustomComponentsFlagsFilter;
    internal static Func<MechLabPanel, MechComponentDef, bool>? CustomComponentsIMechLabFilter;
    internal static Func<FilterInfo, MechComponentDef, bool>? CustomComponentsCategoryFilter;
    internal static Action? BattleTechPerformanceFixFilterChanged;

    private static bool initialized;
    private static readonly List<HBSDOTweenToggle> Tabs = new();
    private static readonly List<CustomButtonInfo> Buttons = new();
    private static MechLabPanel mechLab = null!;
    private static HBSRadioSet _tabRadioSet = null!;

    public static MechLabInventoryWidget Widget => mechLab.inventoryWidget;
    private static SVGCache IconCache => mechLab.dataManager.SVGCache;

    private static TabInfo _currentTab = null!;
    private static ButtonInfo _currentButton = null!;

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

        _tabRadioSet.defaultButton = Tabs.FirstOrDefault();
        _tabRadioSet.Reset();

        TabPressed(Control.Tabs.First());
    }

    public static void PreInit(MechLabPanel mechLabPanel)
    {
        if (!initialized)
        {
            initialized = true;
            Logging.Debug?.Log("No tabs found - create new");
            mechLab = mechLabPanel;
            InitTabs();
        }
        else if (mechLab != mechLabPanel)
        {
            Logging.Debug?.Log("other mechlab widget, droping");
            foreach (var toggle in Tabs)
            {
                if (toggle != null)
                    toggle.gameObject.Destroy();
            }
            Tabs.Clear();
            foreach (var b in Buttons)
            {
                if (b.Go != null)
                    b.Go.Destroy();
            }
            Buttons.Clear();

            mechLab = mechLabPanel;
            InitTabs();
        }
        else
        {
            Logging.Debug?.Log("already modified");
        }
    }

    private static void InitTabs()
    {
        Logging.Debug?.Log("-- hide old tabs");
        Widget.tabAllToggleObj.gameObject.SetActive(false);
        Widget.tabAmmoToggleObj.gameObject.SetActive(false);
        Widget.tabEquipmentToggleObj.gameObject.SetActive(false);
        Widget.tabMechPartToggleObj.gameObject.SetActive(false);
        Widget.tabWeaponsToggleObj.gameObject.SetActive(false);

        // ReSharper disable once Unity.InefficientPropertyAccess
        var go = Widget.tabWeaponsToggleObj.gameObject;
        _tabRadioSet = go.transform.parent.GetComponent<HBSRadioSet>();
        _tabRadioSet.ClearRadioButtons();


        foreach (var settingsTab in Control.Tabs)
        {
            Logging.Debug?.Log($"--- create tab [{settingsTab.Caption}]");

            var tab = Object.Instantiate(Widget.tabWeaponsToggleObj.gameObject, go.transform.parent);
            tab.transform.position = go.transform.position;
            tab.transform.localScale = Vector3.one;
            var radio = tab.GetComponent<HBSDOTweenToggle>();

            radio.OnClicked.RemoveAllListeners();
            radio.OnClicked.AddListener(() => TabPressed(settingsTab));

            tab.SetActive(true);
            Tabs.Add(radio);
            var text = tab.GetComponentInChildren<TextMeshProUGUI>(true);
            if (text != null)
                text.SetText(settingsTab.Caption);
            _tabRadioSet.RadioButtons.Add(radio);
        }

        Logging.Debug?.Log($"-- create small buttons");

        go = Widget.filterBtnAll;

        var grid = go.transform.parent.gameObject.GetComponent<GridLayoutGroup>();
        grid.spacing = new(8,8);

        ShowChilds(go, "");

        Widget.filterRadioSet.ClearRadioButtons();
        for (var i = 0; i < 14; i++)
        {
            Logging.Debug?.Log($"--- Create Button #{i}");
            try
            {
                var info = new CustomButtonInfo(go, i, FilterPressed);
                Buttons.Add(info);
                Widget.filterRadioSet.AddButtonToRadioSet(info.Toggle);
            }
            catch (Exception e)
            {
                Logging.Error?.Log(e.Message + " " + e.StackTrace);
            }
        }

        Widget.filterRadioSet.defaultButton = Buttons.FirstOrDefault()?.Toggle;
        Widget.filterRadioSet.Reset();
    }

    private static void ShowChilds(GameObject go, string prefix)
    {
        Logging.Debug?.Log(prefix + " " + go.name);
        foreach (Transform child in go.transform)
        {
            ShowChilds(child.gameObject, prefix + "-");
        }
    }

    private static void FilterPressed(int num)
    {
        Logging.Debug?.Log($"PRESSED [{num}]");

        if (_currentTab.Buttons == null || _currentTab.Buttons.Length <= num)
            return;

        _currentButton = _currentTab.Buttons[num];

        BattleTechPerformanceFixFilterChanged?.Invoke();
    }

    private static void TabPressed(TabInfo settingsTab)
    {
        Logging.Debug?.Log($"PRESSED [{settingsTab.Caption}]");
        foreach (var buttonInfo in Buttons)
        {
            buttonInfo.Go.SetActive(false);
        }

        _currentTab = settingsTab;

        if (settingsTab.Buttons == null || settingsTab.Buttons.Length == 0)
            return;

        for (var i = 0; i < 14 && i < settingsTab.Buttons.Length; i++)
        {
            Logging.Debug?.Log($"- button {i}");

            var buttonInfo = settingsTab.Buttons[i];
            var button = Buttons[i];
            if (!string.IsNullOrEmpty(buttonInfo.Text))
            {
                Logging.Debug?.Log($"-- set text");
                button.Text.text = buttonInfo.Text;
                button.GoText.SetActive(true);
            }
            else
            {
                button.GoText.SetActive(false);
            }


            if (!string.IsNullOrEmpty(buttonInfo.Icon))
            {
                Logging.Debug?.Log($"-- set icon");
                button.Icon.vectorGraphics = IconCache.GetAsset(buttonInfo.Icon);
                button.GoIcon.SetActive(true);
                if (button.Icon.vectorGraphics == null)
                {
                    Logging.Error?.Log($"Icon {buttonInfo.Icon} not found, replacing with ???");
                    button.Text.text = "???";
                    button.GoText.SetActive(true);
                }
            }
            else
            {
                button.GoIcon.SetActive(false);
            }

            if (!string.IsNullOrEmpty(buttonInfo.Tag))
            {
                Logging.Debug?.Log($"- set tag");
                button.Tag.text = buttonInfo.Tag;
                button.GoTag.SetActive(true);
            }
            else
            {
                button.GoTag.SetActive(false);
            }

            if (!string.IsNullOrEmpty(buttonInfo.Tooltip))
            {
                var state = new HBSTooltipStateData();
                state.SetString(buttonInfo.Tooltip);

                button.Tooltip.SetDefaultStateData(state);

            }
            else
            {
                var state = new HBSTooltipStateData();
                state.SetDisabled();
                button.Tooltip.SetDefaultStateData(state);
            }

            button.Go.SetActive(!buttonInfo.Debug || Control.Settings.ShowDebugButtons);
        }
        Widget.filterRadioSet.Reset();
        FilterPressed(0);
    }

    public static bool ApplyFilter(MechComponentDef? item)
    {
        // ReSharper disable once Unity.NoNullPropagation
        if (mechLab?.activeMechDef == null)
            return true;

        if (item == null)
        {
            Logging.Error?.Log($"-- ITEM IS NULL!");
            return false;
        }
        //Control.LogDebug($"- {item.Description.Id}");

        if (CustomComponentsFlagsFilter != null && !CustomComponentsFlagsFilter(item))
        {
            return false;
        }

        if (!ApplyFilter(item, _currentTab?.Filter))
        {
            //Control.LogDebug($"-- tab filter miss");
            return false;
        }

        if (!ApplyFilter(item, _currentButton?.Filter))
        {
            //Control.LogDebug($"-- button filter miss");
            return false;
        }

        if (CustomComponentsIMechLabFilter != null && !CustomComponentsIMechLabFilter(mechLab, item))
        {
            return false;
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

    private static bool ApplyFilter(MechComponentDef? item, FilterInfo? filter)
    {
        if (item == null)
        {
            Logging.Error?.Log($"-- ITEM IS NULL!");
            return false;
        }

        if (filter == null)
        {
            Logging.Warn?.Log($"--- empty filter");
            return true;
        }

        if (filter.ComponentTypes is { Length: > 0 } && !filter.ComponentTypes.Contains(item.ComponentType))
        {
            Logging.Warn?.Log($"--- not component type");
            return false;
        }

        if (item.ComponentType == ComponentType.Weapon)
        {
            if (item is not WeaponDef weaponDef)
            {
                Logging.Error?.Log($"{item.Description.Id} of type {item.ComponentType} is actually not of type {typeof(WeaponDef)}");
                return false;
            }

            if (filter.WeaponCategories is { Length: > 0 } && !filter.WeaponCategories.Contains(weaponDef.WeaponCategoryValue.Name))
            {
                return false;
            }

            if (filter.UILookAndColorIcons is { Length: > 0 } && !filter.UILookAndColorIcons.Contains(weaponDef.weaponCategoryValue.Icon))
            {
                return false;
            }
        }

        if (item.ComponentType == ComponentType.AmmunitionBox)
        {
            if (item is not AmmunitionBoxDef boxDef)
            {
                Logging.Error?.Log($"{item.Description.Id} of type {item.ComponentType} is actually not of type {typeof(AmmunitionBoxDef)}");
                return false;
            }

            if (filter.AmmoCategories is { Length: > 0 } && !filter.AmmoCategories.Contains(boxDef.Ammo.AmmoCategoryValue.Name))
            {
                return false;
            }

            if (filter.UILookAndColorIcons is { Length: > 0 } && !filter.UILookAndColorIcons.Contains(boxDef.Ammo.AmmoCategoryValue.Icon))
            {
                return false;
            }
        }

        if (CustomComponentsCategoryFilter != null && !CustomComponentsCategoryFilter(filter, item))
        {
            return false;
        }
        return true;
    }

    public static void CallFilter()
    {
        Widget.ApplyFiltering();
    }
}