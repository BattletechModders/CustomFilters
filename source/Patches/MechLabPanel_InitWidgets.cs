using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using RootMotion.FinalIK;
using SVGImporter;

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

            var loadRequest = __instance.dataManager.CreateLoadRequest();
            foreach (var str in Control.Settings.Tabs.SelectMany(i => i.Buttons).Where(i => !string.IsNullOrEmpty(i.Icon)).Select(i => i.Icon))
            {
                loadRequest.AddLoadRequest<SVGAsset>(BattleTechResourceType.SVGAsset, str, null);
            }
            loadRequest.ProcessRequests();
        }

        [HarmonyPostfix]
        public static void Init(MechLabPanel __instance, MechLabInventoryWidget ___inventoryWidget)
        {
            Control.LogDebug("MechLab.InitWidgets - Postfix");
            UIHandler.Init(__instance, ___inventoryWidget);
        }
    }
}