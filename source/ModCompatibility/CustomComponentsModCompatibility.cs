using System;
using System.Linq;
using CustomComponents;
using CustomFilters.MechLabFiltering;

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
            (filter.Categories is not { Length: > 0 } || filter.Categories.Any(item.IsCategory))
            && (filter.NotCategories is not { Length: > 0 } || !filter.NotCategories.Any(item.IsCategory));
    }
}
