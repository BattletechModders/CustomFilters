using CustomFilters.MechBayScrolling;

namespace CustomFilters.ModCompatibility;

internal static class CustomUnityModCompatibility
{
    internal static void Setup()
    {
        CustomMechBayMechStorageWidgetTracker.CustomUnitsDisablesSupportForLanceConfiguratorPanelInSimGame = true;
    }
}
