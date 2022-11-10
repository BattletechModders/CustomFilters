using System;
using BattleTech;
using HBS.Logging;
using UnityEngine;

namespace CustomFilters;

[Serializable]
public class FilterInfo
{
    public ComponentType[] componentTypes;
    public string[] weaponCategories;
    public string[] ammoCategories;
    public string[] uiLookAndColorIcons;
    public string[] categories;
    public string[] notCategories;

    public string[] tags;
    public string[] notTags;
}

[Serializable]
public class ButtonInfo
{
    public string text = null;
    public string tag = null;
    public string icon = null;
    public string tooltip = null;
    public bool debug = false;

    [SerializeField]
    public FilterInfo filter = null;
}

[Serializable]
public class TabInfo
{
    public string caption;
    public FilterInfo filter;

    [SerializeField]
    public ButtonInfo[] buttons;
}

public class CustomFiltersSettings
{
    public LogLevel LogLevel = LogLevel.Debug;

    public TabInfo[] Tabs = null;
    public bool ShowDebugButtons = false;
    public bool DumpSettings = true;
}