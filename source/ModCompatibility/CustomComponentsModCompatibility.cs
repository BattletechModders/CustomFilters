using System;
using System.Linq;
using CustomComponents;
using CustomFilters.MechLabInventory;

namespace CustomFilters.ModCompatibility;

internal static class CustomComponentsModCompatibility
{
    internal static void Setup()
    {
        UIHandler.CustomComponentsFlagsFilter = item => !item.Flags<CCFlags>().HideFromInv;

        UIHandler.CustomComponentsIMechLabFilter = (mechLab, item) =>
        {
            foreach (var ccFilter in item.GetComponents<IMechLabFilter>())
            {
                try
                {
                    if (!ccFilter.CheckFilter(mechLab))
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    Logging.Error?.Log("Error in filter", e);
                }
            }
            return true;
        };

        UIHandler.CustomComponentsCategoryFilter = (filter, item) =>
            (!filter.Categories.HasAny() || filter.Categories.Any(item.IsCategory))
            && (!filter.NotCategories.HasAny() || !filter.NotCategories.Any(item.IsCategory));
    }
}
