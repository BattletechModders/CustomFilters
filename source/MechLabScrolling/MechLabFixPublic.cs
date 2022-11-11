#nullable disable
using System.Collections.Generic;
using BattleTech.UI;

namespace CustomFilters.MechLabScrolling;

// CU uses this, but not sure why
public static class MechLabFixPublic
{
    public static List<ListElementController_BASE_NotListView> GetRawInventory(this MechLabInventoryWidget widget)
    {
        if (MechLabFixStateTracker.GetInstance(widget, out var state))
        {
            return state.RawInventory;
        }
        return null;
    }
}