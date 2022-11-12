#nullable disable
// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using BattleTech.UI;
using Harmony;

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
    public static IEnumerable<CodeInstruction> InitWidgets_Transpile(IEnumerable<CodeInstruction> instructions)
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
        Logging.Debug?.Log("[ShopFix] CreateUIModule");
        try
        {
            return uiManager.GetOrCreateUIModule<SG_Shop_Screen>(prefabOverride, resort);
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
            throw;
        }
    }

    [HarmonyPrefix]
    public static void Prefix(MechLabPanel __instance)
    {
        Logging.Debug?.Log("[ShopFix] InitWidgets_Pre");
        try
        {
            if (__instance.Shop != null) __instance.Shop.Pool();
        }
        catch (Exception e)
        {
            Logging.Error?.Log(e);
        }
    }
}