#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.SetData))]
public static class MechBayMechStorageWidget_SetData
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        Logging.Trace?.Log("MechBayMechStorageWidget.SetData");
        return true;
    }
}