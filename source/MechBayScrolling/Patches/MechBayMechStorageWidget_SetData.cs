#nullable disable
// ReSharper disable InconsistentNaming
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(MechBayMechStorageWidget), nameof(MechBayMechStorageWidget.SetData))]
public static class MechBayMechStorageWidget_SetData
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        Log.Main.Trace?.Log(nameof(MechBayMechStorageWidget_SetData));
        return true;
    }
}