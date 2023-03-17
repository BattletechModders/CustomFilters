#nullable disable
// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BattleTech.UI;

namespace CustomFilters.MechLabShopPooling;

[HarmonyBefore(Mods.BattleTechPerformanceFix)]
[HarmonyPatch(typeof(MechLabPanel), nameof(MechLabPanel.InitWidgets))]
internal static class MechLabPanel_InitWidgets
{
    public static bool Prepare()
    {
        return Control.MainSettings.MechLab.ShopPoolingFixEnabled;
    }

    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var to = AccessTools.Method(typeof(MechLabPanel_InitWidgets), nameof(CreateUIModule));

        foreach (var instruction in instructions)
        {
            if (instruction.operand is MethodBase method && method.Name == nameof(CreateUIModule))
            {
                instruction.opcode = OpCodes.Call;
                instruction.operand = to;
            }

            yield return instruction;
        }
    }

    public static SG_Shop_Screen CreateUIModule(UIManager uiManager, string prefabOverride = "", bool resort = true)
    {
        Log.Main.Trace?.Log(nameof(MechLabPanel_InitWidgets) + "." + nameof(Transpiler) + "." + nameof(CreateUIModule));
        try
        {
            return uiManager.GetOrCreateUIModule<SG_Shop_Screen>(prefabOverride, resort);
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
            throw;
        }
    }

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechLabPanel __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        Log.Main.Trace?.Log(nameof(MechLabPanel_InitWidgets));

        if (__instance.Shop != null) __instance.Shop.Pool();
    }
}