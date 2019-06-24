using System;
using System.Collections.Generic;
using BattleTech;
using BattleTech.UI;
using HBS.Logging;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomFilters
{
    [Serializable]
    public class FilterInfo
    {
        public ComponentType[] ComponentTypes;
        public WeaponCategory[] WeaponCategories;
        public string[] Categories;
        public string[] NotCategories;

        public string[] Tags;
        public string[] NotTags;
    }

    [Serializable]
    public class ButtonInfo
    {
        public string Text = null;
        public string Tag = null;
        public string Icon = null;
        public string Tooltip = null;

        [SerializeField]
        public FilterInfo Filter = null;
    }
    [Serializable]

    public class TabInfo
    {
        public string Caption;
        public FilterInfo Filter;

        [SerializeField]
        public ButtonInfo[] Buttons;

    }

    public class CustomFiltersSettings
    {
        public LogLevel LogLevel = LogLevel.Debug;

        public TabInfo[] Tabs = null;

            /*
        {
            new TabInfo
            {
                Caption = "WEAPON",
                Filter = new FilterInfo {ComponentTypes = new[] {ComponentType.Weapon}},
                Buttons = new [] {
                    new ButtonInfo
                    {
                        Text = "all",
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_weapon_Ballistic",
                        Filter = new FilterInfo { WeaponCategories = new [] {WeaponCategory.Ballistic} }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_weapon_Energy",
                        Filter = new FilterInfo { WeaponCategories = new [] {WeaponCategory.Energy} }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_weapon_Missile",
                        Filter = new FilterInfo { WeaponCategories = new [] {WeaponCategory.Missile} }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_weapon_Support",
                        Filter = new FilterInfo { WeaponCategories = new [] {WeaponCategory.AntiPersonnel, WeaponCategory.AMS} }
                    }
                }
            },
            new TabInfo
            {
                Caption = "AMMO",
                Filter = new FilterInfo {ComponentTypes = new[] {ComponentType.AmmunitionBox}},
                Buttons = new [] {
                    new ButtonInfo
                    {
                        Text = "all",
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_weapon_Ballistic",
                        Tag = "AB",
                        Filter = new FilterInfo { WeaponCategories = new [] {WeaponCategory.Ballistic} }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_weapon_Energy",
                        Tag = "AB",
                        Filter = new FilterInfo { WeaponCategories = new [] {WeaponCategory.Energy} }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_weapon_Missile",
                        Tag = "AB",
                        Filter = new FilterInfo { WeaponCategories = new [] {WeaponCategory.Missile} }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_weapon_Support",
                        Tag = "AB",
                        Filter = new FilterInfo { WeaponCategories = new [] {WeaponCategory.AntiPersonnel, WeaponCategory.AMS } }
                    }
                }
            },
            new TabInfo
            {
                Caption = "Engine/HS",
                Filter = new FilterInfo
                {
                    ComponentTypes = new[] {ComponentType.HeatSink}
                },
                Buttons = new [] {
                    new ButtonInfo
                    {
                        Text = "all",
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_equipment_HeatBank",
                        Tag = "cor",
                        Filter = new FilterInfo { Categories = new [] { "EngineCore" } }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_equipment_Heatsink",
                        Tag = "kit",
                        Filter = new FilterInfo { Categories = new [] { "Cooling", "EngineHeatBlock" } }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_equipment_ThermalExchanger",
                        Tag = "eng",
                        Filter = new FilterInfo { Categories = new [] { "EngineShield" } }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_equipment_Heatsink",
                        Filter = new FilterInfo { NotCategories = new [] { "EnginePart", "Cooling" } }
                    }
                }
            },
            new TabInfo
            {
                Caption = "EQUIP",
                Filter = new FilterInfo
                {
                    ComponentTypes = new[] {ComponentType.JumpJet, ComponentType.Upgrade}
                },
                Buttons = new [] {
                    new ButtonInfo
                    {
                        Text = "all",
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_action_jump",
                        Filter = new FilterInfo { ComponentTypes = new[] {ComponentType.JumpJet} }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_equipment_Cockpit",
                        Filter = new FilterInfo
                        {
                            ComponentTypes = new[] {ComponentType.Upgrade},
                            Categories = new [] { "LifeSupportA", "LifeSupportB", "Cockpit", "SensorsA", "SensorsB" }
                        }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_quantity",
                        Filter = new FilterInfo
                        {
                            ComponentTypes = new[] {ComponentType.Upgrade},
                            Categories = new [] { "Armor", "Structure", "Gyro"}
                        }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_equipment_ActuatorLeg",
                        Filter = new FilterInfo
                        {
                            ComponentTypes = new[] {ComponentType.Upgrade},
                            Categories = new [] { "LegHip", "LegUpperActuator", "LegLowerActuator", "LegFootActuator" }
                        }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_equipment_ActuatorArm",
                        Filter = new FilterInfo
                        {
                            ComponentTypes = new[] {ComponentType.Upgrade},
                            Categories = new [] { "ArmShoulder", "ArmUpperActuator", "ArmLowerActuator", "ArmHandActuator" }
                        }
                    },
                    new ButtonInfo
                    {
                        Icon =  "uixSvgIcon_equipment_Generic",
                        Filter = new FilterInfo
                        {
                            ComponentTypes = new[] {ComponentType.Upgrade},
                            NotCategories = new [] { "Armor", "Structure", "Gyro",
                                "LifeSupportA", "LifeSupportB", "Cockpit", "SensorsA", "SensorsB",
                                "LegHip", "LegUpperActuator", "LegLowerActuator", "LegFootActuator",
                                "ArmShoulder", "ArmUpperActuator", "ArmLowerActuator", "ArmHandActuator"
                            }
                        }
                    }

                }
            },
        };
 */
        public bool BTPerfFix = false;

        public void Complete()
        {
            BTPerfFix = BattletechPerformanceFix.Main.settings.features.ContainsKey("MechlabFix") &&
                        BattletechPerformanceFix.Main.settings.features["MechlabFix"];

            Control.LogDebug("Using MechLabFix: " + BTPerfFix.ToString());
        }
    }
}
