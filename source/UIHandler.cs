using System;
using System.Collections.Generic;
using System.Linq;
using BattletechPerformanceFix;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using BattleTech.UI.Tooltips;
using CustomComponents;
using FluffyUnderware.DevTools.Extensions;
using Harmony;
using SVGImporter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomFilters
{
    public static class UIHandler
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

        public static MechLabInventoryWidget widget;
        public static MechLabInventoryWidget work_widget;

        private static SVGCache icon_cache;

        private static InventoryHelper inv_helper;
        private static TabInfo current_tab;
        private static ButtonInfo current_button;

        public static bool FilterInWork => widget == work_widget;



        public static void Init(MechLabPanel mechLabPanel, MechLabInventoryWidget mechLabInventoryWidget)
        {
            inv_helper.filterBtnAll.Value.SetActive(false);
            inv_helper.filterBtnEquipmentHeatsink.Value.SetActive(false);
            inv_helper.filterBtnEquipmentJumpjet.Value.SetActive(false);
            inv_helper.filterBtnEquipmentUpgrade.Value.SetActive(false);

            inv_helper.filterBtnWeaponEnergy.Value.SetActive(false);
            inv_helper.filterBtnWeaponMissile.Value.SetActive(false);
            inv_helper.filterBtnWeaponBallistic.Value.SetActive(false);
            inv_helper.filterBtnWeaponSmall.Value.SetActive(false);

            tab_radioset.defaultButton = tabs.FirstOrDefault();
            tab_radioset.Reset();

            TabPressed(Control.Settings.Tabs.FirstOrDefault());
        }

        public static void PreInit(MechLabPanel mechLabPanel, MechLabInventoryWidget mechLabInventoryWidget)
        {

            if (tabs == null)
            {
                Control.LogDebug("No tabs found - create new");
                tabs = new List<HBSDOTweenToggle>();
                buttons = new List<button_info>();
                InitTabs(mechLabPanel, mechLabInventoryWidget);
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
                InitTabs(mechLabPanel, mechLabInventoryWidget);
            }
            else
            {
                Control.LogDebug("already modified");
                widget = mechLabInventoryWidget;
            }
        }

        private static void InitTabs(MechLabPanel mechLabPanel, MechLabInventoryWidget mechLabInventoryWidget)
        {
            Control.LogDebug("- Init Tabs");

            mechlab = mechLabPanel;
            widget = mechLabInventoryWidget;
            icon_cache = new Traverse(mechlab.dataManager).Property<SVGCache>("SVGCache").Value;

            inv_helper = new InventoryHelper(widget);

            Control.LogDebug("-- hide old tabs");
            inv_helper.tabAllToggleObj.Value.gameObject.SetActive(false);
            inv_helper.tabEquipmentToggleObj.Value.gameObject.SetActive(false);
            inv_helper.tabMechPartToggleObj.Value.gameObject.SetActive(false);
            inv_helper.tabWeaponsToggleObj.Value.gameObject.SetActive(false);

            var go = inv_helper.tabWeaponsToggleObj.Value.gameObject;
            tab_radioset = go.transform.parent.GetComponent<HBSRadioSet>();
            tab_radioset.ClearRadioButtons();


            foreach (var settingsTab in Control.Settings.Tabs)
            {
                Control.LogDebug($"--- create tab [{settingsTab.Caption}]");

                var tab = GameObject.Instantiate(inv_helper.tabWeaponsToggleObj.Value.gameObject);
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

            go = inv_helper.filterBtnAll.Value;

            var grid = go.transform.parent.gameObject.GetComponent<GridLayoutGroup>();
            grid.spacing = new Vector2(8,8);

            ShowChilds(go, "");

            inv_helper.filterRadioSet.Value.ClearRadioButtons();
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
                    inv_helper.filterRadioSet.Value.AddButtonToRadioSet(info.toggle);
                }
                catch (Exception e)
                {
                    Control.LogError(e.Message + " " + e.StackTrace);
                }
            }

            inv_helper.filterRadioSet.Value.defaultButton = buttons.FirstOrDefault()?.toggle;
            inv_helper.filterRadioSet.Value.Reset();
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
            Control.LogDebug("Tab: ");
            Control.LogDebug(JsonUtility.ToJson(current_tab, true));
            Control.LogDebug(JsonUtility.ToJson(current_tab.Filter, true));

            Control.LogDebug("Button: ");
            Control.LogDebug(JsonUtility.ToJson(current_button, true));
            Control.LogDebug(JsonUtility.ToJson(current_button.Filter, true));

            if (Control.Settings.BTPerfFix)
                MechlabFix.state?.FilterChanged();
            else
                widget.ApplyFiltering(true);
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
                    button.text.RefreshText();
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
                        button.text.RefreshText();
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
                    button.tag.RefreshText();
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
            inv_helper.filterRadioSet.Value.Reset();
            FilterPressed(0);
        }

        public static bool ApplyFilter(MechComponentDef item)
        {
            if (item == null)
            {
                Control.LogError($"-- ITEM IS NULL!");
                return false;
            }
            //Control.LogDebug($"- {item.Description.Id}");

            if (item.Is<Flags>(out var f) && f.HideFromInventory)
            {
                //Control.LogDebug($"-- default");
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

            if (filter.ComponentTypes != null && filter.ComponentTypes.Length > 0 &&
                !filter.ComponentTypes.Contains(item.ComponentType))
            {
                //Control.LogDebug($"--- not component type");
                return false;
            }

            if (filter.WeaponCategories != null && filter.WeaponCategories.Length > 0)
                if (item.ComponentType == ComponentType.Weapon)
                {
                    if (!(item is WeaponDef weapon))
                    {
                        Control.LogError(
                            $"{item.ComponentType} have weapon type but not contain WeaponDef");
                        return false;
                    }

                    if (!filter.WeaponCategories.Contains(weapon.Category))
                    {
                        //Control.LogDebug($"--- weapon, wrong weapon type");

                        return false;
                    }
                }
                else if (item.ComponentType == ComponentType.AmmunitionBox)
                {
                    if (!(item is AmmunitionBoxDef ammo))
                    {
                        Control.LogError(
                            $"{item.Description.Id} have AmmunitionBox type but not contain AmmunitionBoxDef");
                        return false;
                    }

                    WeaponCategory wc;

                    AmmoCategory category = ammo.Ammo.Category;
                    if (category == AmmoCategory.AC2 || category == AmmoCategory.AC5 || category == AmmoCategory.AC10 ||
                        category == AmmoCategory.AC20 || category == AmmoCategory.GAUSS)
                    {
                        wc = WeaponCategory.Ballistic;
                    }
                    else if (category == AmmoCategory.Flamer)
                    {
                        wc = WeaponCategory.Energy;
                    }
                    else if (category == AmmoCategory.LRM || category == AmmoCategory.SRM)
                    {
                        wc = WeaponCategory.Missile;
                    }
                    else if (category == AmmoCategory.MG)
                    {
                        wc = WeaponCategory.AntiPersonnel;
                    }
                    else if (category == AmmoCategory.AMS)
                    {
                        wc = WeaponCategory.AMS;
                    }
                    else
                    {
                        Control.LogError($"{item.Description.Id} have wrong ammo type {category}");
                        return false;
                    }

                    if (!filter.WeaponCategories.Contains(wc))
                    {
                        //Control.LogDebug($"--- ammo, wrong weapon type");

                        return false;
                    }
                }

            if (filter.Categories != null && filter.Categories.Length > 0)
            {
                if (!filter.Categories.Any(item.IsCategory))
                {
                    //Control.LogDebug($"--- not found category");

                    return false;
                }
            }

            if (filter.NotCategories != null && filter.NotCategories.Length > 0)
            {
                if (filter.NotCategories.Any(item.IsCategory))
                {
                    //Control.LogDebug($"--- found forbidden category");

                    return false;
                }
            }

            return true;
        }
    }
}