#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.ReceiveButtonPress))]
public static class MainMenu_ReceiveButtonPress
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled && TagManagerFeature.Settings.SkirmishOptionsShow;
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MainMenu __instance, string button)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (button == "MechBay")
        {
            __runOriginal = false; // if original code runs in RT it takes a lot of time, so better to die early
            TagManagerFeature.Shared.ShowOptions(__instance);
        }
    }
}