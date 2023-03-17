#nullable disable
// ReSharper disable InconsistentNaming
using System;

namespace CustomFilters.MechBayScrolling.Patches;

[HarmonyPatch(typeof(UnityEngine.UI.ScrollRect), nameof(UnityEngine.UI.ScrollRect.LateUpdate))]
public static class ScrollRect_LateUpdate
{
    [HarmonyPrefix]
    public static void Prefix(UnityEngine.UI.ScrollRect __instance)
    {
        try
        {
            CustomStorageWidgetTracker.Get(__instance)?.ScrollRectLateUpdate();
        }
        catch (Exception e)
        {
            Log.Main.Error?.Log(e);
        }
    }
}