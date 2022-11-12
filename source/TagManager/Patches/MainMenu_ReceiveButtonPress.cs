#nullable disable
// ReSharper disable InconsistentNaming
using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.ReceiveButtonPress))]
public static class MainMenu_ReceiveButtonPress
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled && TagManagerFeature.Settings.SkirmishOptionsShow;
    }

    [HarmonyPrefix]
    public static bool Prefix(MainMenu __instance, string button)
    {
        try
        {
            if (button == "MechBay")
            {
                TagManagerFeature.Shared.ShowOptions(__instance);
                return false;
            }
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
            return false;
        }
        return true;
    }
}