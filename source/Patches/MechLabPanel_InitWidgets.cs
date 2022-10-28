using System.Linq;
using BattleTech;
using BattleTech.UI;
using Harmony;
using SVGImporter;

namespace CustomFilters.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.InitWidgets))]
internal static class MechLabPanel_InitWidgets
{
    [HarmonyPrefix]
    public static void Prefix(MechLabPanel __instance)
    {
        Control.LogDebug("MechLab.InitWidgets - Prefix");
        UIHandler.PreInit(__instance);

        // TODO fix race condition (clash with custom components)
        var loadRequest = __instance.dataManager.CreateLoadRequest();
        foreach (var str in Control.Settings.Tabs.SelectMany(i => i.Buttons).Where(i => !string.IsNullOrEmpty(i.Icon)).Select(i => i.Icon))
        {
            loadRequest.AddLoadRequest<SVGAsset>(BattleTechResourceType.SVGAsset, str, null);
        }
        loadRequest.ProcessRequests();
    }

    [HarmonyPostfix]
    public static void Postfix()
    {
        Control.LogDebug("MechLab.InitWidgets - Postfix");
        UIHandler.Init();
    }
}