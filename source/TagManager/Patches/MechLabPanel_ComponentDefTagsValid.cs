#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.ComponentDefTagsValid))]
public static class MechLabPanel_ComponentDefTagsValid
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled;
    }

    [HarmonyPrefix]
    public static bool Prefix(MechLabPanel __instance, MechComponentDef def, bool ___isDebugLab, ref bool __result)
    {
        try
        {
            __result = __instance.IsSimGame || TagManagerFeature.Shared.ComponentIsValidForSkirmish(def, ___isDebugLab);
            return false;
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }

        return true;
    }
}