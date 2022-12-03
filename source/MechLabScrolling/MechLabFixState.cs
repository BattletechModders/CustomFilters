using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomFilters.MechLabFiltering;
using UnityEngine;
using UnityEngine.UI;

namespace CustomFilters.MechLabScrolling;

// heavily based o m22spencer' work in BattletechPerformanceFix
// heavily cleaned up and fixed by CptMoore
/* This patch fixes the slow inventory list creation within the mechlab. Without the fix, it manifests as a very long loadscreen where the indicator is frozen.

   The core of the problem is a lack of separation between Data & Visuals.
   Most of the logic requires operating on visual elements, which come from the asset pool (or a prefab if not in pool)
   additionally, the creation or modification of data causes preperation for re-render of the assets. (UpdateTooltips, UpdateDescription, Update....)

   Solution:
   Separate the data & visual elements entirely.
   Always process the data first, and then only create or re-use a couple of visual elements to display it.
   The user only sees 8 items at once, and they're expensive to create, so only make 8 of them.
*/
internal class MechLabFixState
{
    private const int ItemsOnScreen = 7;
    private const int RowBufferCount = 1;
    internal const int ItemLimit = ItemsOnScreen + RowBufferCount;

    internal List<ListElementController_BASE_NotListView> RawInventory => _rawInventory;

    private readonly MechLabPanel _mechLab;
    private readonly MechLabInventoryWidget _widget;

    private List<ListElementController_BASE_NotListView> _rawInventory = new();
    private bool _rawInventoryChanged = false;
    private List<ListElementController_BASE_NotListView> filteredInventory = new();
    private int _rowCountBelowScreen;
    private int _rowMaxToStartLoading;

    // Index of current item element at the top of scrollrect
    private int _rowToStartLoading;

    private readonly MechLabFixGameObjects _gameObjects;

    internal MechLabFixState(MechLabPanel mechLab)
    {
        _mechLab = mechLab;
        _widget = mechLab.inventoryWidget;
        _gameObjects = new(_widget);
    }

    internal void PopulateInventory()
    {
        var sw = new Stopwatch();
        sw.Start();
        _gameObjects.Refresh();

        Log.Main.Debug?.Log($"StorageInventory contains {_mechLab.storageInventory.Count}");

        if (_mechLab.IsSimGame) _mechLab.originalStorageInventory = _mechLab.storageInventory;

        Log.Main.Debug?.Log($"Mechbay Patch initialized :simGame? {_mechLab.IsSimGame}");

        _rawInventory = _mechLab.storageInventory.Select<MechComponentRef, ListElementController_BASE_NotListView>(
            componentRef =>
            {
                componentRef.DataManager = _mechLab.dataManager;
                componentRef.RefreshComponentDef();
                var count = !_mechLab.IsSimGame
                    ? int.MinValue
                    : _mechLab.sim.GetItemCount(componentRef.Def.Description, componentRef.Def.GetType(),
                        _mechLab.sim.GetItemCountDamageType(componentRef));

                if (componentRef.ComponentDefType == ComponentType.Weapon)
                {
                    var controller = new ListElementController_InventoryWeapon_NotListView();
                    controller.InitAndFillInSpecificWidget(componentRef, null, _mechLab.dataManager, null, count);
                    return controller;
                }
                else
                {
                    var controller = new ListElementController_InventoryGear_NotListView();
                    controller.InitAndFillInSpecificWidget(componentRef, null, _mechLab.dataManager, null, count);
                    return controller;
                }
            }).ToList();
        _rawInventoryChanged = true;

        // End
        Log.Main.Debug?.Log($"inventory cached in {sw.Elapsed.TotalMilliseconds} ms");

        FilterChanged();
    }

    private bool TryGet(MechComponentRef mcr, out ListElementController_BASE_NotListView lec)
    {
        var nullableLec = _rawInventory.FirstOrDefault(ri => ri.componentDef == mcr.Def && mcr.DamageLevel == GetRef(ri).DamageLevel);
        lec = nullableLec!;
        return nullableLec != null;
    }

    private MechLabDraggableItemType ToDraggableType(MechComponentDef def)
    {
        switch (def.ComponentType)
        {
            case ComponentType.Weapon:
                return MechLabDraggableItemType.InventoryWeapon;
            case ComponentType.AmmunitionBox:
                return MechLabDraggableItemType.InventoryItem;
            case ComponentType.HeatSink:
            case ComponentType.JumpJet:
            case ComponentType.Upgrade:
            case ComponentType.Special:
            case ComponentType.MechPart:
                return MechLabDraggableItemType.InventoryGear;
            default:
                return MechLabDraggableItemType.NOT_SET;
        }
    }

    private void Sort(List<ListElementController_BASE_NotListView> items)
    {
        Log.Main.Trace?.Log($"Sorting: {string.Join(",",items.Select(item => GetRef(item).ComponentDefID))}");

        var sw = Stopwatch.StartNew();
        var cs = _widget.currentSort;
        Log.Main.Debug?.Log($"Sort using {cs.Method.DeclaringType?.FullName}::{cs.Method}");

        var iieA = _gameObjects.ElementTmpA;
        var iieB = _gameObjects.ElementTmpB;

        items.Sort((l, r) =>
        {
            iieA.ComponentRef = GetRef(l);
            iieA.controller = l;
            iieA.controller.ItemWidget = iieA;
            iieA.ItemType = ToDraggableType(l.componentDef);

            iieB.ComponentRef = GetRef(r);
            iieB.controller = r;
            iieB.controller.ItemWidget = iieB;
            iieB.ItemType = ToDraggableType(r.componentDef);

            var res = cs.Invoke(iieA, iieB);
            Log.Main.Trace?.Log(
                $"Compare {iieA.ComponentRef.ComponentDefID}({iieA.controller.ItemWidget != null}) & {iieB.ComponentRef.ComponentDefID}({iieB.controller.ItemWidget != null}) -> {res}");
            return res;
        });

        Log.Main.Debug?.Log($"Sorted in {sw.ElapsedMilliseconds} ms");
        Log.Main.Trace?.Log($"Sorting: {string.Join(",",items.Select(item => GetRef(item).ComponentDefID))}");
    }

    private MechComponentRef GetRef(ListElementController_BASE_NotListView lec)
    {
        switch (lec)
        {
            case ListElementController_InventoryWeapon_NotListView lecIw:
                return lecIw.componentRef;
            case ListElementController_InventoryGear_NotListView lecIg:
                return lecIg.componentRef;
            default:
                Log.Main.Error?.Log("lec is not gear or weapon: " + lec.GetId());
                throw new ArgumentOutOfRangeException(nameof(lec));
        }
    }

    /* The user has changed a filter, and we rebuild the item cache. */
    internal void FilterChanged(bool resetIndex = true)
    {
        if (resetIndex)
        {
            _widget.scrollbarArea.verticalNormalizedPosition = 1.0f;
            _rowToStartLoading = 0;
        }

        UIHandlerTracker.GetInstance(_widget, out var handler);

        if (_rawInventoryChanged)
        {
            var sw = new Stopwatch();
            sw.Start();

            Sort(_rawInventory);
            _rawInventoryChanged = false;

            sw.Stop();
            Log.Main.Debug?.Log($"sorting raw inventory took {sw.ElapsedMilliseconds} ms");
        }

        filteredInventory = _rawInventory.Where(i => handler.ApplyFilter(i.componentDef)).ToList();
        _rowCountBelowScreen = Mathf.Max(0, filteredInventory.Count - ItemsOnScreen);
        _rowMaxToStartLoading = Mathf.Max(0, _rowCountBelowScreen - RowBufferCount);
        _rowToStartLoading = Mathf.Clamp(_rowToStartLoading, 0, _rowMaxToStartLoading);
        Refresh();
    }

    private string LecToString(ListElementController_BASE_NotListView lec)
    {
        return $"[id:{GetRef(lec).ComponentDefID},damage:{GetRef(lec).DamageLevel},quantity:{lec.quantity},id:{lec.GetId()}]";
    }

    private void Refresh()
    {
        Log.Main.Trace?.Log($"Refresh row={_rowToStartLoading} filteredInventory.Count={filteredInventory.Count} vnp={_widget.scrollbarArea.verticalNormalizedPosition}");
        Log.Main.Trace?.Log($"Refresh inventoryCount={_widget.localInventory.Count}");

        // display elements missing -> not yet initialized
        if (_widget.localInventory.Count == 0)
        {
            return;
        }

        var toShow = filteredInventory.Skip(_rowToStartLoading).Take(ItemLimit).ToList();
        var icc = _widget.localInventory.ToList();

        Log.Main.Trace?.Log(
            toShow
                .Select(LecToString)
                .Aggregate("Showing: ", (prev, next) => $"{prev}\n{next}")
        );

        var details = new List<string>();

        foreach (var lec in toShow)
        {
            var iw = icc[0];
            icc.RemoveAt(0);
            var cref = GetRef(lec);
            iw.ClearEverything();
            iw.ComponentRef = cref;
            lec.ItemWidget = iw;
            iw.SetData(lec, _widget, lec.quantity);
            lec.SetupLook(iw);
            iw.gameObject.SetActive(true);
            details.Insert(
                0,
                $"enabled {iw.ComponentRef.ComponentDefID} {iw.GetComponent<RectTransform>().anchoredPosition}"
            );
        }

        foreach (var unused in icc)
        {
            unused.gameObject.SetActive(false);
        }

        var elementHeight = 64;
        var spacingY = 16;
        var halfSpacingY = spacingY / 2;
        var paddingY = elementHeight + spacingY;

        var topCount = _rowToStartLoading;
        var bottomCount = filteredInventory.Count -
                          (_rowToStartLoading + _widget.localInventory.Count(ii => ii.gameObject.activeSelf));

        var vlg = _widget.listParent.GetComponent<VerticalLayoutGroup>();
        var padding = vlg.padding;
        padding.top = 12 + (topCount > 0 ? paddingY * topCount - halfSpacingY : 0);
        padding.bottom = 12 + (bottomCount > 0 ? paddingY * bottomCount - halfSpacingY : 0);
        vlg.padding = padding;

        LayoutRebuilder.MarkLayoutForRebuild(vlg.GetComponent<RectTransform>());

        _mechLab.RefreshInventorySelectability();
        Log.Main.Trace?.Log(
            $"Refresh padding={padding}" +
            $" vnp={_widget.scrollbarArea.verticalNormalizedPosition}" +
            $" lli={"(" + string.Join(", ", details) + ")"}"
        );
    }

    internal void LateUpdate()
    {
        var scrollRect = _widget.scrollbarArea;
        var newIndexCandidate = (int)(_rowCountBelowScreen *
                                      (1.0f - scrollRect.verticalNormalizedPosition));
        newIndexCandidate = Mathf.Clamp(newIndexCandidate, 0, _rowMaxToStartLoading);
        if (_rowToStartLoading != newIndexCandidate)
        {
            _rowToStartLoading = newIndexCandidate;
            Log.Main.Debug?.Log(
                $"Refresh with: {newIndexCandidate} {scrollRect.verticalNormalizedPosition}");
            Refresh();
        }
    }

    internal void OnRemoveItem(IMechLabDraggableItem item)
    {
        var nlv = (InventoryItemElement_NotListView)item;
        if (!TryGet(item.ComponentRef, out var lec))
        {
            Log.Main.Error?.Log("Existing not found");
            return;
        }

        var quantity = nlv.controller.quantity;
        if (quantity == 0 || lec.quantity == int.MinValue)
        {
            Log.Main.Error?.Log("Existing has invalid quantity");
            return;
        }

        const int change = -1;
        Log.Main.Debug?.Log($"OnRemoveItem id={item.ComponentRef.ComponentDefID} quantity={lec.quantity} change={change}");
        lec.ModifyQuantity(change);
        if (lec.quantity < 1)
        {
            _rawInventory.Remove(lec);
            _rawInventoryChanged = true;
        }

        FilterChanged(false);
        Refresh();
    }

    internal void OnItemGrab(ref IMechLabDraggableItem item)
    {
        var nlv = (InventoryItemElement_NotListView)item;
        var iw = _gameObjects.ElementTmpG;
        var lec = nlv.controller;
        var cref = GetRef(lec);
        iw.ClearEverything();
        iw.ComponentRef = cref;
        lec.ItemWidget = iw;
        iw.SetData(lec, _widget, lec.quantity);
        lec.SetupLook(iw);
        iw.gameObject.SetActive(true);
        item = iw;
    }

    internal void OnAddItem(IMechLabDraggableItem item)
    {
        var nlv = item as InventoryItemElement_NotListView;
        var quantity = nlv == null ? 1 : nlv.controller.quantity;
        if (TryGet(item.ComponentRef, out var existing))
        {
            Log.Main.Debug?.Log($"OnAddItem existing {quantity}");
            if (existing.quantity != int.MinValue) existing.ModifyQuantity(quantity);
            Refresh();
        }
        else
        {
            Log.Main.Debug?.Log($"OnAddItem new {quantity}");
            var controller = nlv == null ? null : nlv.controller;
            if (controller == null)
            {
                if (item.ComponentRef.ComponentDefType == ComponentType.Weapon)
                {
                    var ncontroller = new ListElementController_InventoryWeapon_NotListView();
                    ncontroller.InitAndCreate(item.ComponentRef, _mechLab.dataManager,
                        _widget, quantity);
                    controller = ncontroller;
                }
                else
                {
                    var ncontroller = new ListElementController_InventoryGear_NotListView();
                    ncontroller.InitAndCreate(item.ComponentRef, _mechLab.dataManager,
                        _widget, quantity);
                    controller = ncontroller;
                }
            }

            _rawInventory.Add(controller);
            _rawInventoryChanged = true;
        }
    }

    internal void ClearInventory()
    {
        if (_widget.localInventory == null)
        {
            return;
        }

        Log.Main.Debug?.Log($"ClearInventory with inventoryCount={_widget.localInventory.Count}");
        for (var i = _widget.localInventory.Count - 1; i >= 0; i--)
        {
            var iie = _widget.localInventory[i];
            iie.gameObject.transform.SetParent(null, false);
            if (iie.controller != null)
            {
                iie.controller = null;
                iie.SetClickable(true);
                iie.SetDraggable(true);
                _widget.dataManager.PoolGameObject(ListElementController_BASE_NotListView.INVENTORY_ELEMENT_PREFAB_NotListView, iie.gameObject);
            }
            else
            {
                _widget.dataManager.PoolGameObject(MechLabInventoryWidget.INVENTORY_ITEM_PREFAB, iie.gameObject);
            }
        }
        _widget.localInventory.Clear();
    }


    // TODO we ignore this for now as we filter out all items
    // see RefreshJumpJetOptions on the filtering option
    internal void RefreshInventorySelectability()
    {
        // TODO RefreshInventorySelectability sets an overlay
        // SetOutOfStockOverlay based on MechCanEquipItem response
        // uses MechCanUseAmmo and ValidateAdd and ValidateAddSimple
        // CC2 would need to extend this for Hardpoints and Validation

        // foreach (var iie in _widget.localInventory)
        // {
        //     iie.SetOutOfStockOverlay(!_mechLab.MechCanEquipItem(iie, false));
        // }
    }

    internal void ApplySorting()
    {
        // we ignore this, because ApplySorting is always called after ApplyFiltering
    }

    internal void ApplyFiltering(bool refreshPositioning)
    {
        FilterChanged(refreshPositioning);
    }
}