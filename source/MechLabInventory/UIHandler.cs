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

internal class UIHandler
{
    // compatibility
    internal static Func<MechComponentDef, bool>? CustomComponentsFlagsFilter;
    internal static Func<MechLabPanel, MechComponentDef, bool>? CustomComponentsIMechLabFilter;
    internal static Func<FilterInfo, MechComponentDef, bool>? CustomComponentsCategoryFilter;
    internal static Action? BattleTechPerformanceFixFilterChanged;

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

        _currentTab = Control.Tabs.First();
        _currentButton = _currentTab.Buttons!.First();

        Logging.Debug?.Log("No tabs found - create new");

        Logging.Debug?.Log("-- hide old tabs");
        _widget.tabAllToggleObj.gameObject.SetActive(false);
        _widget.tabAmmoToggleObj.gameObject.SetActive(false);
        _widget.tabEquipmentToggleObj.gameObject.SetActive(false);
        _widget.tabMechPartToggleObj.gameObject.SetActive(false);
        _widget.tabWeaponsToggleObj.gameObject.SetActive(false);

        // ReSharper disable once Unity.InefficientPropertyAccess
        var go = _widget.tabWeaponsToggleObj.gameObject;
        _tabRadioSet = go.transform.parent.GetComponent<HBSRadioSet>();
        _tabRadioSet.ClearRadioButtons();

        foreach (var settingsTab in Control.Tabs)
        {
            Logging.Debug?.Log($"--- create tab [{settingsTab.Caption}]");

            var tab = Object.Instantiate(_widget.tabWeaponsToggleObj.gameObject, go.transform.parent);
            tab.transform.position = go.transform.position;
            tab.transform.localScale = Vector3.one;
            var radio = tab.GetComponent<HBSDOTweenToggle>();

            radio.OnClicked.RemoveAllListeners();
            radio.OnClicked.AddListener(() => TabPressed(settingsTab));

            tab.SetActive(true);
            _tabs.Add(radio);
            var text = tab.GetComponentInChildren<TextMeshProUGUI>(true);
            if (text != null)
                text.SetText(settingsTab.Caption);
            _tabRadioSet.RadioButtons.Add(radio);
        }

        Logging.Debug?.Log("-- create small buttons");

        go = _widget.filterBtnAll;

        var grid = go.transform.parent.gameObject.GetComponent<GridLayoutGroup>();
        grid.spacing = new(8,8);

        ShowChildren(go, "");

        _widget.filterRadioSet.ClearRadioButtons();
        for (var i = 0; i < 14; i++)
        {
            Logging.Debug?.Log($"--- Create Button #{i}");
            try
            {
                var info = new CustomButtonInfo(go, i, FilterPressed);
                _buttons.Add(info);
                _widget.filterRadioSet.AddButtonToRadioSet(info.Toggle);
            }
            catch (Exception e)
            {
                Logging.Error?.Log(e.Message + " " + e.StackTrace);
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

        TabPressed(Control.Tabs.First());
    }

    // this should never be called unless pooled objects get removed at some point
    internal void Destroy()
    {
        Logging.Debug?.Log("destroying modifications");
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
        Logging.Debug?.Log(prefix + " " + go.name);
        foreach (Transform child in go.transform)
        {
            ShowChildren(child.gameObject, prefix + "-");
        }
    }

    private void FilterPressed(int num)
    {
        Logging.Debug?.Log($"PRESSED [{num}]");

        if (_currentTab.Buttons == null || _currentTab.Buttons.Length <= num)
            return;

        _currentButton = _currentTab.Buttons[num];

        BattleTechPerformanceFixFilterChanged?.Invoke();
    }

    private void TabPressed(TabInfo settingsTab)
    {
        Logging.Debug?.Log($"PRESSED [{settingsTab.Caption}]");
        foreach (var buttonInfo in _buttons)
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
            var button = _buttons[i];
            if (!string.IsNullOrEmpty(buttonInfo.Text))
            {
                Logging.Debug?.Log("-- set text");
                button.Text.text = buttonInfo.Text;
                button.GoText.SetActive(true);
            }
            else
            {
                button.GoText.SetActive(false);
            }


            if (!string.IsNullOrEmpty(buttonInfo.Icon))
            {
                Logging.Debug?.Log("-- set icon");
                button.Icon.vectorGraphics = _iconCache.GetAsset(buttonInfo.Icon);
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
                Logging.Debug?.Log("- set tag");
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
            Logging.Error?.Log("-- ITEM IS NULL!");
            return false;
        }
        //Control.LogDebug($"- {item.Description.Id}");

        if (CustomComponentsFlagsFilter != null && !CustomComponentsFlagsFilter(item))
        {
            return false;
        }

        if (!ApplyFilter(item, _currentTab.Filter))
        {
            //Control.LogDebug($"-- tab filter miss");
            return false;
        }

        if (!ApplyFilter(item, _currentButton.Filter))
        {
            //Control.LogDebug($"-- button filter miss");
            return false;
        }

        if (CustomComponentsIMechLabFilter != null && !CustomComponentsIMechLabFilter(_mechLab, item))
        {
            return false;
        }

        if (item.ComponentType == ComponentType.JumpJet)
        {
            if (item is JumpJetDef jj)
            {
                if (_mechLab.activeMechDef.Chassis.Tonnage < jj.MinTonnage)
                    return false;
                if (_mechLab.activeMechDef.Chassis.Tonnage > jj.MaxTonnage)
                    return false;
            }
        }


        return true;
    }

    private bool ApplyFilter(MechComponentDef? item, FilterInfo? filter)
    {
        if (item == null)
        {
            Logging.Error?.Log("-- ITEM IS NULL!");
            return false;
        }

        if (filter == null)
        {
            Logging.Warn?.Log("--- empty filter");
            return true;
        }

        if (filter.ComponentTypes is { Length: > 0 } && !filter.ComponentTypes.Contains(item.ComponentType))
        {
            Logging.Warn?.Log("--- not component type");
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

    internal void ApplyFiltering()
    {
        _widget.ApplyFiltering();
    }
}