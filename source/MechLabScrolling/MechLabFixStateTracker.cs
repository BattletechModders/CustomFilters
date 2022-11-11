using System.Collections.Generic;
using System.Linq;
using BattleTech.UI;
using UnityEngine.UI;

namespace CustomFilters.MechLabScrolling;

internal static class MechLabFixStateTracker
{
    internal static void SetInstance(MechLabPanel panel)
    {
        Cleanup();

        if (!Panels.TryGetValue(panel, out var state))
        {
            state = new(panel);
            Panels[panel] = state;
            Widgets[panel.inventoryWidget] = state;
            ScrollRects[panel.inventoryWidget.scrollbarArea] = state;
        }
    }

    private static readonly Dictionary<MechLabInventoryWidget, MechLabFixState> Widgets = new();
    internal static bool GetInstance(MechLabInventoryWidget widget, out MechLabFixState state)
    {
        return Widgets.TryGetValue(widget, out state);
    }

    private static readonly Dictionary<MechLabPanel, MechLabFixState> Panels = new();
    internal static bool GetInstance(MechLabPanel panel, out MechLabFixState state)
    {
        return Panels.TryGetValue(panel, out state);
    }

    private static readonly Dictionary<ScrollRect, MechLabFixState> ScrollRects = new();
    internal static bool GetInstance(ScrollRect panel, out MechLabFixState state)
    {
        return ScrollRects.TryGetValue(panel, out state);
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
        foreach (var scrollRect in ScrollRects.Keys.ToList())
        {
            if (scrollRect.gameObject == null)
            {
                ScrollRects.Remove(scrollRect);
            }
        }
    }
}