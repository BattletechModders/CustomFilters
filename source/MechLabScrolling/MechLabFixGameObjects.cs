using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomFilters.Shared;
using UnityEngine;

namespace CustomFilters.MechLabScrolling;

internal class MechLabFixGameObjects
{
    private static DataManager DataManager => UnityGameInstance.BattleTechGame.DataManager;

    private readonly MechLabInventoryWidget _widget;

    // temporary elements
    internal InventoryItemElement_NotListView ElementTmpA { get; }
    internal InventoryItemElement_NotListView ElementTmpB { get; }
    internal InventoryItemElement_NotListView ElementTmpG { get; }

    internal MechLabFixGameObjects(MechLabInventoryWidget widget)
    {
        _widget = widget;

        ElementTmpA = CreateElement("A"); // for sorting
        ElementTmpB = CreateElement("B"); // for sorting
        ElementTmpG = CreateElement("G"); // for grabbing
    }

    internal void Refresh()
    {
        Log.Main.Trace?.Log($"{nameof(MechLabFixGameObjects)}.{nameof(Refresh)} inventoryCount={_widget.localInventory.Count}");

        /* Allocate very few visual elements, as this is extremely slow for both allocation and deallocation.
                   It's the difference between a couple of milliseconds and several seconds for many unique items in inventory
                   This is the core of the fix, the rest is just to make it work within HBS's existing code.
                */
        // this should ether be 0 or the itemLimit
        var start = _widget.localInventory.Count;
        var end = MechLabFixState.ItemLimit - 1;
        for (var i = start; i <= end; i++)
        {
            var nlv = CreateElement();
            nlv.SetRadioParent(_widget.inventoryRadioSet);
            var transform = nlv.gameObject.transform;
            transform.SetParent(_widget.listParent, false);
            transform.localScale = Vector3.one;
            _widget.localInventory.Add(nlv);
        }
        //not sure if needed, better safe than sorry
        for (var i = 0; i < start; i++)
        {
            _widget.localInventory[i].gameObject.SetActive(false);
        }
    }

    private InventoryItemElement_NotListView CreateElement(string? id = null)
    {
        var iieGo = DataManager.PooledInstantiate(
            ListElementController_BASE_NotListView.INVENTORY_ELEMENT_PREFAB_NotListView,
            BattleTechResourceType.UIModulePrefabs);
        if (id != null)
            iieGo.name = $"{ListElementController_BASE_NotListView.INVENTORY_ELEMENT_PREFAB_NotListView} [{id}]";
        iieGo.transform.SetParent(SharedGameObjects.ContainerTransform, false);
        iieGo.SetActive(false); // everything from pool should already be deactivated
        return iieGo.GetComponent<InventoryItemElement_NotListView>();
    }
}