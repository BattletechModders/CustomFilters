using BattleTech.UI;
using Harmony;

namespace CustomFilters.Patches
{
    [HarmonyPatch(typeof(MechLabPanel))]
    [HarmonyPatch("LoadMech")]
    public static class MechLabPanel_LoadMech
    {
        [HarmonyPostfix]
        public static void ApplyFilter()
        {
            UIHandler.CallFilter();
        }

    }
}