#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch(typeof(SkirmishMechBayPanel), nameof(SkirmishMechBayPanel.RequestResources))]
public static class SkirmishMechBayPanel_RequestResources_Patch
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled;
    }

    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, SkirmishMechBayPanel __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        TagManagerFeature.Shared.RequestResources(__instance);
        __runOriginal = false;
    }
}