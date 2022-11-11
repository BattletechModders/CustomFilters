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
            Logging.Debug?.Log("FilterUsingHBSCode");
            var handler = UIHandlerTracker.Instance;
            return list.Where(i => handler.ApplyFilter(i.componentDef)).ToList();
        };

        UIHandler.BattleTechPerformanceFixFilterChanged = MechLabFixPublic.FilterChanged;
    }
}