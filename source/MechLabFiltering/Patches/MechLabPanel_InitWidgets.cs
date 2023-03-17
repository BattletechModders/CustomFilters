#nullable disable
// ReSharper disable InconsistentNaming
using System;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using SVGImporter;

namespace CustomFilters.MechLabFiltering.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.InitWidgets))]
internal static class MechLabPanel_InitWidgets
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabPanel __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechLabPanel_InitWidgets) + "." + nameof(Prefix));
        try
        {
            UIHandlerTracker.SetInstance(__instance);

            // TODO fix race condition (clash with custom components)
            var loadRequest = __instance.dataManager.CreateLoadRequest();
            var tabs = Control.MainSettings.MechLab.Tabs;
            foreach (var str in tabs.SelectMany(i => i.Buttons).Where(i => !string.IsNullOrEmpty(i.Icon)).Select(i => i.Icon))
            {
                loadRequest.AddLoadRequest<SVGAsset>(BattleTechResourceType.SVGAsset, str, null);
            }
            loadRequest.ProcessRequests();
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(MechLabPanel __instance)
    {
        Log.Main.Trace?.Log(nameof(MechLabPanel_InitWidgets) + "." + nameof(Postfix));
        try
        {
            if (UIHandlerTracker.GetInstance(__instance, out var handler))
            {
                handler.ResetFilters();
            }
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}