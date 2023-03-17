using System.Collections.Generic;
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling;

internal static class CustomStorageWidgetTracker
{
    private static readonly Dictionary<MechBayMechStorageWidget, CustomStorageWidget> Widgets = new();
    private static readonly Dictionary<UnityEngine.UI.ScrollRect, CustomStorageWidget> ScrollRects = new();

    internal static bool TryGet(MechBayMechStorageWidget widget, out CustomStorageWidget customWidget)
    {
        if (widget.ParentDropTarget == null)
        {
            customWidget = default!;
            return false;
        }

        // SimGame MechBayPanel "uixPrfPanl_storageMechUnit-Element"
        // SimGame&Skirmish LanceConfigurationPanel "uixPrfPanl_LC_MechSlot"
        // Skirmish SkirmishMechBay "uixPrfPanl_LC_mechUnit-Element"

        // SimGame + LanceConfigurationPanel is being skipped
        // has more custom logic and other mods interfere (CustomUnits and maybe TisButAScratch)
        // while there are no performance benefits yet unless a mod would introduce many more mech bays for SimGame
        if (widget is { IsSimGame: true, ParentDropTarget: LanceConfiguratorPanel })
        {
            customWidget = default!;
            return false;
        }

        if (!Widgets.TryGetValue(widget, out customWidget))
        {
            customWidget = new(widget);
            Widgets[widget] = customWidget;
            ScrollRects[customWidget.GetScrollRect()] = customWidget;
        }
        return true;
    }
    internal static CustomStorageWidget? Get(UnityEngine.UI.ScrollRect scrollRect)
    {
        return ScrollRects.TryGetValue(scrollRect, out var customWidget) ? customWidget : null;
    }
}