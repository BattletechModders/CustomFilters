using System;
using BattleTech.UI;
using Harmony;

namespace CustomFilters.MechLabInventory.Patches;

[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.LoadMech))]
internal static class MechLabPanel_LoadMech
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        try
        {
            UIHandler.CallFilter();
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
    }
}