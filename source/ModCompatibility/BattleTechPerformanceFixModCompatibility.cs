using System.Linq;
using BattletechPerformanceFix.MechLabFix;
using CustomFilters.MechLabInventory;

namespace CustomFilters.ModCompatibility;

internal static class BattleTechPerformanceFixModCompatibility
{
    internal static void Setup()
    {
        MechLabFixPublic.FilterFunc = list =>
        {
            Logging.LogDebug("FilterUsingHBSCode");
            return list.Where(i => UIHandler.ApplyFilter(i.componentDef)).ToList();
        };

        UIHandler.BattleTechPerformanceFixFilterChanged = MechLabFixPublic.FilterChanged;
    }
}