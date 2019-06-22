using BattleTech.UI;
using Harmony;

namespace CustomFilters.Patches
{
    [HarmonyPatch(typeof(MechLabInventoryWidget))]
    [HarmonyPatch("OnFilterButtonClicked")]
    public static class MechLabInventoryWidget_OnFilterButtonClicked
    {
        [HarmonyPrefix]
        public static bool OnFilterButtonClicked(MechLabInventoryWidget __instance)
        {
            return __instance != UIHandler.widget;
        }
    }

    [HarmonyPatch(typeof(MechLabInventoryWidget))]
    [HarmonyPatch("SetFiltersWeapons")]
    public static class MechLabInventoryWidget_SetFilterWeapons
    {
        [HarmonyPrefix]
        public static bool SetFilterWeapons(MechLabInventoryWidget __instance)
        {
            return __instance != UIHandler.widget;
        }
    }

    [HarmonyPatch(typeof(MechLabInventoryWidget))]
    [HarmonyPatch("ResetFilters")]
    public static class MechLabInventoryWidget_ResetFilters
    {
        [HarmonyPrefix]
        public static bool ResetFiltersPrefix(MechLabInventoryWidget __instance)
        {
            if (__instance != UIHandler.widget)
                return true;

           //__instance.FilterAll();

            return false;
        }
    }
}
