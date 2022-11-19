using System.Collections.Generic;
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling;

internal static class CustomMechBayMechStorageWidgetTracker
{
    private static readonly Dictionary<MechBayMechStorageWidget, CustomMechBayMechStorageWidget> Widgets = new();
    private static readonly Dictionary<UnityEngine.UI.ScrollRect, CustomMechBayMechStorageWidget> ScrollRects = new();
    internal static bool CustomUnitsDisablesSupportForLanceConfiguratorPanelInSimGame = false;

    internal static bool TryGet(MechBayMechStorageWidget widget, out CustomMechBayMechStorageWidget customWidget)
    {
        if (CustomUnitsDisablesSupportForLanceConfiguratorPanelInSimGame && widget.IsSimGame && widget.ParentDropTarget is LanceConfiguratorPanel)
        {
            customWidget = default!;
            return false;
        }

        // SimGame MechBayPanel "uixPrfPanl_storageMechUnit-Element"
        // SimGame&Skirmish LanceConfigurationPanel "uixPrfPanl_LC_MechSlot"
        // Skirmish SkirmishMechBay "uixPrfPanl_LC_mechUnit-Element"
        if (!Widgets.TryGetValue(widget, out customWidget))
        {
            customWidget = new(widget);
            Widgets[widget] = customWidget;
            ScrollRects[customWidget.GetScrollRect()] = customWidget;
        }
        return true;
    }
    internal static CustomMechBayMechStorageWidget Get(UnityEngine.UI.ScrollRect scrollRect)
    {
        return ScrollRects.TryGetValue(scrollRect, out var customWidget) ? customWidget : null!;
    }
}