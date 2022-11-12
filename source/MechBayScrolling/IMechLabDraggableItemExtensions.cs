using System;
using BattleTech.UI;

namespace CustomFilters.MechBayScrolling;

static class IMechLabDraggableItemExtensions
{
    internal static HBSDOTweenToggle GetToggle(this IMechLabDraggableItem item)
    {
        switch (item)
        {
            case LanceLoadoutMechItem mechItem:
                return mechItem.toggleObj;
            case MechBayMechUnitElement mechUnitElement:
                return mechUnitElement.toggleObj;
            case MechBayChassisUnitElement chassisUnitElement:
                return chassisUnitElement.toggleObj;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}