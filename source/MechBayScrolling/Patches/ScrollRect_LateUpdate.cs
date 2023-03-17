#nullable disable
// ReSharper disable InconsistentNaming
namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(UnityEngine.UI.ScrollRect), nameof(UnityEngine.UI.ScrollRect.LateUpdate))]
public static class ScrollRect_LateUpdate
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, UnityEngine.UI.ScrollRect __instance)
    {

        CustomStorageWidgetTracker.Get(__instance)?.ScrollRectLateUpdate();
    }
}