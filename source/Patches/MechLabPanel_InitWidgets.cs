using BattleTech.UI;
using Harmony;
using RootMotion.FinalIK;

namespace CustomFilters
{
    [HarmonyPatch(typeof(MechLabPanel))]
    [HarmonyPatch("InitWidgets")]
    public static class MechLabPanel_InitWidgets
    {
        [HarmonyPrefix]
        public static void PreInit(MechLabPanel __instance, MechLabInventoryWidget ___inventoryWidget)
        {
            Control.LogDebug("MechLab.InitWidgets - Prefix");
            UIHandler.PreInit(__instance, ___inventoryWidget);
        }

        [HarmonyPostfix]
        public static void Init(MechLabPanel __instance, MechLabInventoryWidget ___inventoryWidget)
        {
            Control.LogDebug("MechLab.InitWidgets - Postfix");
            UIHandler.Init(__instance, ___inventoryWidget);
        }
    }
}