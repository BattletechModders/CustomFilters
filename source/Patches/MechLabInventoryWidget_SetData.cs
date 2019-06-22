using BattleTech.UI;
using Harmony;

namespace CustomFilters
{
    [HarmonyPatch(typeof(MechLabInventoryWidget))]
    [HarmonyPatch("SetData")]
    public static class MechLabInventoryWidget_SetData
    {

        public static void InitWidget(MechLabInventoryWidget __instance)
        {
            UIHandler.work_widget = __instance;
        }
    }
}