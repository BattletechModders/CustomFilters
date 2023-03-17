#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech;
using BattleTech.UI;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.ComponentDefTagsValid))]
public static class MechLabPanel_ComponentDefTagsValid
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabPanel __instance, MechComponentDef def, bool ___isDebugLab, ref bool __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        __result = __instance.IsSimGame || TagManagerFeature.Shared.ComponentIsValidForSkirmish(def, ___isDebugLab);
        __runOriginal = false;
    }
}