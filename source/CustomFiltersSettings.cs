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
        public bool Debug = false;

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
        public bool ShowDebugButtons = false;
        public bool ShowBlacklistedSkirmish = false;
        public bool ShowBlacklistedSimGame = true;
        public bool BTPerfFix = false;

        public void Complete()
        {
            BTPerfFix = BattletechPerformanceFix.Main.settings.features.ContainsKey("MechlabFix") &&
                        BattletechPerformanceFix.Main.settings.features["MechlabFix"];

            Control.LogDebug("Using MechLabFix: " + BTPerfFix.ToString());
        }
    }
}
