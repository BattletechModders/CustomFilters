using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using SVGImporter;

namespace CustomFilters.MechLabInventory.Patches;

// ReSharper disable InconsistentNaming
[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.InitWidgets))]
internal static class MechLabPanel_InitWidgets
{
    [HarmonyPrefix]
    public static void Prefix(MechLabPanel __instance)
    {
        try
        {
            Logging.Debug?.Log("MechLab.InitWidgets - Prefix");
            UIHandler.PreInit(__instance);

            // TODO fix race condition (clash with custom components)
            var loadRequest = __instance.dataManager.CreateLoadRequest();
            foreach (var str in Control.Tabs.SelectMany(i => i.Buttons).Where(i => !string.IsNullOrEmpty(i.Icon)).Select(i => i.Icon))
            {
                loadRequest.AddLoadRequest<SVGAsset>(BattleTechResourceType.SVGAsset, str, null);
            }
            loadRequest.ProcessRequests();
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
    }

    [HarmonyPostfix]
    public static void Postfix()
    {
        try
        {
            Logging.Debug?.Log("MechLab.InitWidgets - Postfix");
            UIHandler.Init();
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
    }
}