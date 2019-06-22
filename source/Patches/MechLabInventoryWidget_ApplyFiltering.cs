using System;
using System.Collections.Generic;
using System.Linq;
using BattletechPerformanceFix;
using BattleTech.UI;
using CustomComponents;
using Harmony;

namespace CustomFilters.Patches
{
    [HarmonyPatch(typeof(MechLabInventoryWidget))]
    [HarmonyPatch("ApplyFiltering")]
    public static class MechLabInventoryWidget_ApplyFiltering
    {
        [HarmonyPrefix]
        public static bool ApplyFiltering(MechLabInventoryWidget __instance, bool refreshPositioning,
            List<InventoryItemElement_NotListView> ___localInventory)
        {
            if (__instance != UIHandler.widget || Control.Settings.BTPerfFix)
                return true;

            foreach (var item in ___localInventory)
                item.gameObject.SetActive(UIHandler.ApplyFilter(item.ComponentRef.Def));

            return false;
        }
    }

    [HarmonyPatch(typeof(PatchMechlabLimitItems))]
    [HarmonyPatch("FilterUsingHBSCode")]
    public static class PatchMechlabLimitItems_FilterUsingHBSCode
    {
        [HarmonyPrefix]
        public static bool ApplyFiltering(List<ListElementController_BASE_NotListView> items, ref List<ListElementController_BASE_NotListView> __result)
        {
            Control.LogDebug("FilterUsingHBSCode");

            if (!UIHandler.FilterInWork && !Control.Settings.BTPerfFix)
                return true;

            Control.LogDebug("Start to work with items");
            __result = items.Where(i => UIHandler.ApplyFilter(i.componentDef)).ToList();
            return false;
        }
    }


}