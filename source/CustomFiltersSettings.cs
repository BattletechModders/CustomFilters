using System;
using BattleTech;
using HBS.Logging;
using UnityEngine;

namespace CustomFilters;

[Serializable]
public class FilterInfo
{
    public ComponentType[] ComponentTypes;
    public string[] WeaponCategories;
    public string[] AmmoCategories;
    public string[] UILookAndColorIcons;
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
}