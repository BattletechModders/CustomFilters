using System.Collections.Generic;
using System.Linq;
using BattleTech.UI;

namespace CustomFilters.MechLabInventory;

internal static class UIHandlerTracker
{
    // TODO fix once BTPF uses the same tracker, so we don't need a static Instance for Filtering
    internal static UIHandler Instance = null!;

    private static readonly Dictionary<MechLabInventoryWidget, UIHandler> Widgets = new();
    private static readonly Dictionary<MechLabPanel, UIHandler> Panels = new();
    internal static void SetInstance(MechLabPanel panel)
    {
        Cleanup();

        if (!Panels.TryGetValue(panel, out var handler))
        {
            handler = new(panel);
            Panels[panel] = handler;
            Widgets[panel.inventoryWidget] = handler;
        }

        Instance = handler;
    }

    internal static bool GetInstance(MechLabInventoryWidget widget, out UIHandler handler)
    {
        return Widgets.TryGetValue(widget, out handler);
    }

    internal static bool GetInstance(MechLabPanel panel, out UIHandler handler)
    {
        return Panels.TryGetValue(panel, out handler);
    }

    private static void Cleanup()
    {
        foreach (var panel in Panels.Keys.ToList())
        {
            if (panel.gameObject == null)
            {
                Panels.Remove(panel);
            }
        }
        foreach (var widget in Widgets.Keys.ToList())
        {
            if (widget.gameObject == null)
            {
                Widgets.Remove(widget);
            }
        }
    }
}