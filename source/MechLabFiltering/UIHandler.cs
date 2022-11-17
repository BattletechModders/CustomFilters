using System;
using System.Collections.Generic;
using System.Linq;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomFilters.MechLabFiltering.TabConfig;
using CustomFilters.MechLabScrolling;
using CustomFilters.Settings;
using FluffyUnderware.DevTools.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CustomFilters.MechLabFiltering;

internal class UIHandler
{
    // compatibility
    internal static Func<MechComponentDef, bool>? CustomComponentsFlagsFilter;
    internal static Func<MechLabPanel, MechComponentDef, bool>? CustomComponentsIMechLabFilter;
    internal static Func<FilterInfo, MechComponentDef, bool>? CustomComponentsCategoryFilter;

    private readonly MechLabSettings _settings = Control.MainSettings.MechLab;

    private readonly List<HBSDOTweenToggle> _tabs = new();
    private readonly List<CustomButtonInfo> _buttons = new();
    private readonly MechLabPanel _mechLab;
    private readonly MechLabInventoryWidget _widget;
    private readonly HBSRadioSet _tabRadioSet;
    private readonly SVGCache _iconCache;

    private TabInfo _currentTab;
    private ButtonInfo _currentButton;

    internal UIHandler(MechLabPanel mechLab)
    {
        _mechLab = mechLab;
        _widget = mechLab.inventoryWidget;
        _iconCache = _mechLab.dataManager.SVGCache;

        _currentTab = _settings.Tabs.First();
        _currentButton = _currentTab.Buttons!.First();

        Log.Main.Debug?.Log("No tabs found - create new");

        Log.Main.Debug?.Log("-- hide old tabs");
        _widget.tabAllToggleObj.gameObject.SetActive(false);
        _widget.tabAmmoToggleObj.gameObject.SetActive(false);
        _widget.tabEquipmentToggleObj.gameObject.SetActive(false);
        _widget.tabMechPartToggleObj.gameObject.SetActive(false);
        _widget.tabWeaponsToggleObj.gameObject.SetActive(false);

        // ReSharper disable once Unity.InefficientPropertyAccess
        var go = _widget.tabWeaponsToggleObj.gameObject;
        _tabRadioSet = go.transform.parent.GetComponent<HBSRadioSet>();
        _tabRadioSet.ClearRadioButtons();

        foreach (var tabInfo in _settings.Tabs)
        {
            Log.Main.Debug?.Log($"--- create tab [{tabInfo.Caption}]");

            var tabGo = Object.Instantiate(_widget.tabWeaponsToggleObj.gameObject, go.transform.parent);
            tabGo.transform.position = go.transform.position;
            tabGo.transform.localScale = Vector3.one;
            var radio = tabGo.GetComponent<HBSDOTweenToggle>();

            radio.OnClicked.RemoveAllListeners();
            var info = tabInfo;
            radio.OnClicked.AddListener(() => TabPressed(info));

            tabGo.SetActive(true);
            _tabs.Add(radio);
            var text = tabGo.GetComponentInChildren<TextMeshProUGUI>(true);
            if (text != null)
                text.SetText(tabInfo.Caption);
            _tabRadioSet.RadioButtons.Add(radio);
        }

        Log.Main.Debug?.Log("-- create small buttons");

        go = _widget.filterBtnAll;

        var grid = go.transform.parent.gameObject.GetComponent<GridLayoutGroup>();
        grid.spacing = new(8,8);

        ShowChildren(go, "");

        _widget.filterRadioSet.ClearRadioButtons();
        for (var i = 0; i < 14; i++)
        {
            Log.Main.Debug?.Log($"--- Create Button #{i}");
            try
            {
                var info = new CustomButtonInfo(go, i, FilterPressed);
                _buttons.Add(info);
                _widget.filterRadioSet.AddButtonToRadioSet(info.Toggle);
            }
            catch (Exception e)
            {
                Log.Main.Error?.Log(e.Message + " " + e.StackTrace);
            }
        }

        _widget.filterRadioSet.defaultButton = _buttons.FirstOrDefault()?.Toggle;
        _widget.filterRadioSet.Reset();
    }

    public void ResetFilters()
    {
        _widget.filterBtnAll.SetActive(false);
        _widget.filterBtnEquipmentHeatsink.SetActive(false);
        _widget.filterBtnEquipmentJumpjet.SetActive(false);
        _widget.filterBtnEquipmentUpgrade.SetActive(false);

        _widget.filterBtnWeaponEnergy.SetActive(false);
        _widget.filterBtnWeaponMissile.SetActive(false);
        _widget.filterBtnWeaponBallistic.SetActive(false);
        _widget.filterBtnWeaponSmall.SetActive(false);

        _tabRadioSet.defaultButton = _tabs.FirstOrDefault();
        _tabRadioSet.Reset();

        TabPressed(_settings.Tabs.First());
    }

    // this should never be called unless pooled objects get removed at some point
    internal void Destroy()
    {
        Log.Main.Debug?.Log("destroying modifications");
        foreach (var toggle in _tabs)
        {
            if (toggle != null)
                toggle.gameObject.Destroy();
        }
        _tabs.Clear();
        foreach (var b in _buttons)
        {
            if (b.Go != null)
                b.Go.Destroy();
        }
        _buttons.Clear();
    }

    private static void ShowChildren(GameObject go, string prefix)
    {
        Log.Main.Debug?.Log(prefix + " " + go.name);
        foreach (Transform child in go.transform)
        {
            ShowChildren(child.gameObject, prefix + "-");
        }
    }

    private void FilterPressed(int num)
    {
        Log.Main.Debug?.Log($"PRESSED [{num}]");

        if (_currentTab.Buttons.Length <= num)
            return;

        _currentButton = _currentTab.Buttons[num];

        if (MechLabFixStateTracker.GetInstance(_widget, out var state))
        {
            state.FilterChanged();
        }
    }

    private void TabPressed(TabInfo tabInfo)
    {
        Log.Main.Debug?.Log($"PRESSED [{tabInfo.Caption}]");
        foreach (var buttonInfo in _buttons)
        {
            buttonInfo.Go.SetActive(false);
        }

        _currentTab = tabInfo;

        if (tabInfo.Buttons.Length == 0)
            return;

        for (var i = 0; i < 14 && i < tabInfo.Buttons.Length; i++)
        {
            Log.Main.Debug?.Log($"- button {i}");

            var buttonInfo = tabInfo.Buttons[i];
            var customButtonInfo = _buttons[i];
            if (!string.IsNullOrEmpty(buttonInfo.Text))
            {
                Log.Main.Debug?.Log("-- set text");
                customButtonInfo.Text.text = buttonInfo.Text;
                customButtonInfo.GoText.SetActive(true);
            }
            else
            {
                customButtonInfo.GoText.SetActive(false);
            }


            if (!string.IsNullOrEmpty(buttonInfo.Icon))
            {
                Log.Main.Debug?.Log("-- set icon");
                customButtonInfo.Icon.vectorGraphics = _iconCache.GetAsset(buttonInfo.Icon);
                customButtonInfo.GoIcon.SetActive(true);
                if (customButtonInfo.Icon.vectorGraphics == null)
                {
                    Log.Main.Error?.Log($"Icon {buttonInfo.Icon} not found, replacing with ???");
                    customButtonInfo.Text.text = "???";
                    customButtonInfo.GoText.SetActive(true);
                }
            }
            else
            {
                customButtonInfo.GoIcon.SetActive(false);
            }

            if (!string.IsNullOrEmpty(buttonInfo.Tag))
            {
                Log.Main.Debug?.Log("- set tag");
                customButtonInfo.Tag.text = buttonInfo.Tag;
                customButtonInfo.GoTag.SetActive(true);
            }
            else
            {
                customButtonInfo.GoTag.SetActive(false);
            }

            if (!string.IsNullOrEmpty(buttonInfo.Tooltip))
            {
                var state = new HBSTooltipStateData();
                state.SetString(buttonInfo.Tooltip);

                customButtonInfo.Tooltip.SetDefaultStateData(state);

            }
            else
            {
                var state = new HBSTooltipStateData();
                state.SetDisabled();
                customButtonInfo.Tooltip.SetDefaultStateData(state);
            }

            customButtonInfo.Go.SetActive(!buttonInfo.Debug || _settings.ShowDebugButtons);
        }
        _widget.filterRadioSet.Reset();
        FilterPressed(0);
    }

    internal bool ApplyFilter(MechComponentDef? item)
    {
        // ReSharper disable once Unity.NoNullPropagation
        if (_mechLab.activeMechDef == null)
            return true;

        if (item == null)
        {
            Log.Main.Error?.Log("-- ITEM IS NULL!");
            return false;
        }

        Log.Main.Trace?.Log($"ApplyFilter def item={item.Description.Id}");

        if (CustomComponentsFlagsFilter != null && !CustomComponentsFlagsFilter(item))
        {
            Log.Main.Trace?.Log($"\tfiltered by CC flags");
            return false;
        }

        if (!ApplyFilter(item, _currentTab.Filter))
        {
            Log.Main.Trace?.Log($"\tfiltered by current tab {_currentTab}");
            return false;
        }

        if (!ApplyFilter(item, _currentButton.Filter))
        {
            Log.Main.Trace?.Log($"\tfiltered by current button {_currentButton} in tab {_currentTab}");
            return false;
        }

        if (CustomComponentsIMechLabFilter != null && !CustomComponentsIMechLabFilter(_mechLab, item))
        {
            Log.Main.Trace?.Log($"\tfiltered by IMechLabFilter");
            return false;
        }

        if (item.ComponentType == ComponentType.JumpJet && item is JumpJetDef jj)
        {
            var tonnage = _mechLab.activeMechDef.Chassis.Tonnage;
            if (tonnage < jj.MinTonnage || tonnage > jj.MaxTonnage)
            {
                Log.Main.Trace?.Log($"\tfiltered by JumpJet tonnage");
                return false;
            }
        }

        Log.Main.Trace?.Log($"\taccepted by filters and button {_currentButton} in tab {_currentTab}");
        return true;
    }

    private static bool ApplyFilter(MechComponentDef? item, FilterInfo? filter)
    {
        if (item == null)
        {
            Log.Main.Error?.Log("-- ITEM IS NULL!");
            return false;
        }

        if (filter == null)
        {
            // Logging.Trace?.Log("--- empty filter");
            return true;
        }

        Log.Main.Trace?.Log($"ApplyFilter def+filter item={item.Description.Id}");

        if (filter.ComponentTypes is { Length: > 0 } && !filter.ComponentTypes.Contains(item.ComponentType))
        {
            Log.Main.Trace?.Log($"\tfiltered by ComponentType");
            return false;
        }

        if (item.ComponentType == ComponentType.Weapon)
        {
            if (item is not WeaponDef weaponDef)
            {
                Log.Main.Warning?.Log($"{item.Description.Id} of type {item.ComponentType} is actually not of type {typeof(WeaponDef)}");
                return false;
            }

            if (filter.WeaponCategories is { Length: > 0 } && !filter.WeaponCategories.Contains(weaponDef.WeaponCategoryValue.Name))
            {
                Log.Main.Trace?.Log($"\tfiltered by WeaponCategory Name");
                return false;
            }

            if (filter.UILookAndColorIcons is { Length: > 0 } && !filter.UILookAndColorIcons.Contains(weaponDef.weaponCategoryValue.Icon))
            {
                Log.Main.Trace?.Log($"\tfiltered by WeaponCategory Icon");
                return false;
            }
        }

        if (item.ComponentType == ComponentType.AmmunitionBox)
        {
            if (item is not AmmunitionBoxDef boxDef)
            {
                Log.Main.Warning?.Log($"{item.Description.Id} of type {item.ComponentType} is actually not of type {typeof(AmmunitionBoxDef)}");
                return false;
            }

            if (filter.AmmoCategories is { Length: > 0 } && !filter.AmmoCategories.Contains(boxDef.Ammo.AmmoCategoryValue.Name))
            {
                Log.Main.Trace?.Log($"\tfiltered by AmmoCategoryValue Name");
                return false;
            }

            if (filter.UILookAndColorIcons is { Length: > 0 } && !filter.UILookAndColorIcons.Contains(boxDef.Ammo.AmmoCategoryValue.Icon))
            {
                Log.Main.Trace?.Log($"\tfiltered by AmmoCategoryValue Icon");
                return false;
            }
        }

        if (CustomComponentsCategoryFilter != null && !CustomComponentsCategoryFilter(filter, item))
        {
            Log.Main.Trace?.Log($"\tfiltered by Category");
            return false;
        }

        return true;
    }

    internal void RefreshJumpJetOptions(float tonnage)
    {
        // tonnage > 0 -> part of LoadMech, the only mech based filtering during LoadMech
        // tonnage == -1 -> part of ApplyFiltering
        if (tonnage < 0)
        {
            return;
        }

        // RefreshJumpJetOptions + RefreshInventorySelectability are the mech based filters of the base game
        // RefreshJumpJetOptions hides / filters out items
        // RefreshInventorySelectability just adds an overlay ontop of items
        // we only do filters for now
        // since we have elaborate filters to go through, call them all
        if (MechLabFixStateTracker.GetInstance(_widget, out var mechLabFixState))
        {
            mechLabFixState.FilterChanged();
        }
    }
}