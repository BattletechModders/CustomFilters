#nullable disable
// ReSharper disable InconsistentNaming
using System.Reflection;
using BattleTech;
using HBS.Util;

namespace CustomFilters.TagManager.Patches;

[HarmonyPatch]
public static class JSONSerializationUtility_RehydrateObjectFromDictionary
{
    public static bool Prepare()
    {
        return TagManagerFeature.Settings.Enabled;
    }

    public static MethodBase TargetMethod()
    {
        return typeof(JSONSerializationUtility).GetMethod(nameof(JSONSerializationUtility.RehydrateObjectFromDictionary), BindingFlags.NonPublic | BindingFlags.Static)!;
    }

    [HarmonyPriority(Priority.High)]
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix(object target)
    {
        if (target is MechComponentDef componentDef)
        {
            TagManagerFeature.Shared.ManageComponentTags(componentDef);
        }
        else if (target is MechDef mechDef)
        {
            TagManagerFeature.Shared.ManageMechTags(mechDef);
        }
        else if (target is PilotDef pilotDef)
        {
            TagManagerFeature.Shared.ManagePilotTags(pilotDef);
        }
        else if (target is LanceDef lanceDef)
        {
            TagManagerFeature.Shared.ManageLanceTags(lanceDef);
        }
    }
}