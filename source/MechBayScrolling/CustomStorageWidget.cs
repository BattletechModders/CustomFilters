﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BattleTech;
using BattleTech.UI;
using CustomFilters.MechBaySorting;
using CustomFilters.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace CustomFilters.MechBayScrolling;

internal class CustomStorageWidget
{
    private readonly bool _mechUnitValidationEnabled = Control.MainSettings.MechBay.MechUnitValidationEnabled;
    private readonly MechValidationLevel? _mechUnitValidationLevel = Control.MainSettings.MechBay.MechUnitValidationLevel;
    private readonly MechValidationType[] _mechUnitValidationWarnings = Control.MainSettings.MechBay.MechUnitValidationWarnings;

    private readonly int _screenRowCount;
    private readonly int _rowCellCount;
    private const int RowCountToPreloadAsBuffer = 1;
    private int _rowCountBelowScreen;
    private int _rowIndexForLoadingData;
    private int _rowIndexMaxForLoadingData;

    internal ScrollRect GetScrollRect() => _scrollRect;
    private readonly ScrollRect _scrollRect;
    private readonly GridLayoutGroup _grid;
    private readonly MechBayMechStorageWidget _widget;

    internal CustomStorageWidget(MechBayMechStorageWidget widget)
    {
        _widget = widget;
        _scrollRect = _widget.GetComponentInChildren<ScrollRect>();
        _grid = _widget.itemListParent.GetComponent<GridLayoutGroup>();
        var rect = _scrollRect.GetComponent<RectTransform>();
        _rowCellCount = Mathf.FloorToInt(rect.sizeDelta.x / (_grid.cellSize.x + _grid.spacing.x));
        // Ceil for SimGame MechBay Storage, otherwise all others enclose much of the cells
        _screenRowCount = Mathf.CeilToInt(rect.sizeDelta.y / (_grid.cellSize.y + _grid.spacing.y));
        Log.Main.Debug?.Log($"Widget dimensions set to _rowCellCount={_rowCellCount} and _screenRowCount={_screenRowCount}");
    }

    private List<FakeItem> _inventory = null!;
    private List<FakeItem> _sortedAndFilteredInventory = null!;

    internal void ClearInventory()
    {
        ItemStateResetOnClear();
        _widget.listRadioSet.Reset();
        PoolInventory(0, _widget.inventory.Count - 1);
        _widget.listRadioSet.RadioButtons.Clear();
    }
    private void PoolInventory(int startIndex, int endIndex)
    {
        var inventory = _widget.inventory;
        Log.Main.Trace?.Log($"PoolInventory Count={_widget.inventory.Count} startIndex={startIndex} endIndex={endIndex}");
        for (var index = endIndex; index >= startIndex; index--)
        {
            var inventoryItem = inventory[index];
            inventory.RemoveAt(index);
            PoolItem(inventoryItem);
        }
    }
    private void PoolItem(IMechLabDraggableItem item)
    {
        ItemStateStoreOnPool(item);
        _widget.dataManager.PoolGameObject(_widget.itemPrefabName, item.GameObject);
    }

    private void ItemStateResetOnClear()
    {
        SetAvailableReset();
        SelectedItemReset();
    }
    private void ItemStateStoreOnPool(IMechLabDraggableItem inventoryItem)
    {
        SetAvailableStore(inventoryItem);
        SelectedItemStore(inventoryItem);
    }
    private void ItemStateRestoreOnInstantiate(IMechLabDraggableItem inventoryItem)
    {
        SetAvailableRestore(inventoryItem);
        SelectedItemRestore(inventoryItem);
    }

    internal void InitInventory(List<MechDef> mechDefs, bool resetFilters)
    {
        _inventory = mechDefs.Select(d => new FakeItem(d)).ToList();

        _widget.ClearInventory();
        if (resetFilters)
        {
            _widget.filtersRatioSet.Reset();
            _widget.filterEnabledStock = true;
            _widget.Filter_WeightAll();
        }
        else
        {
            _widget.ApplyFiltering();
        }
    }

    internal void InitInventory(List<ChassisDef> chassisDefs, bool resetFilters)
    {
        _inventory = chassisDefs.Select(d => new FakeItem(d)).ToList();

        _widget.ClearInventory();
        if (resetFilters)
        {
            _widget.filtersRatioSet.Reset();
            _widget.filterEnabledStock = true;
            _widget.Filter_WeightAll();
        }
        else
        {
            _widget.ApplyFiltering();
        }
    }

    private void SetAvailableReset()
    {
        _unavailable.Clear();
    }
    private void SetAvailableStore(IMechLabDraggableItem item)
    {
        if (item is LanceLoadoutMechItem mechItem)
        {
            mechItem.SetAvailable(true);
        }
    }
    private void SetAvailableRestore(IMechLabDraggableItem item)
    {
        if (item is LanceLoadoutMechItem mechItem)
        {
            mechItem.SetAvailable(!_unavailable.Contains(SetAvailableGetId(mechItem)));
        }
    }
    private readonly HashSet<ObjectId> _unavailable = new();
    private void SetAvailable(FakeItem fakeItem, bool available)
    {
        var fakeId = SetAvailableGetId(fakeItem);
        var inventoryItem = _widget.inventory.FirstOrDefault(inventoryItem => fakeId == SetAvailableGetId(inventoryItem));
        if (inventoryItem is LanceLoadoutMechItem mechItem)
        {
            mechItem.SetAvailable(available);
        }
        if (available)
        {
            _unavailable.Remove(fakeId);
        }
        else
        {
            _unavailable.Add(fakeId);
        }
    }
    private bool SetAvailableIfFound(IMechLabDraggableItem item)
    {
        var id = SetAvailableGetId(item);
        var fakeItem = _inventory.FirstOrDefault(fakeItem => SetAvailableGetId(fakeItem) == id);
        if (fakeItem == null)
        {
            return false;
        }
        SetAvailable(fakeItem, true);
        return true;
    }
    private ObjectId SetAvailableGetId(IMechLabDraggableItem item)
    {
        return _widget.IsSimGame ? new(item.MechDef.GUID) : GetId(item);
    }

    private void SelectedItemReset()
    {
        _selectedItemObjectId = null;
    }
    private void SelectedItemRestore(IMechLabDraggableItem inventoryItem)
    {
        if (GetId(inventoryItem) == _selectedItemObjectId)
        {
            inventoryItem.GetToggle().SetToggled(true);
        }
    }
    private void SelectedItemStore(IMechLabDraggableItem inventoryItem)
    {
        if (GetId(inventoryItem) == _selectedItemObjectId)
        {
            _widget.listRadioSet.HaveInitialButtonSet = false;
            _widget.listRadioSet.Reset();
        }
        // TODO not good here alone (setup is in instantiate)
        var toggle = inventoryItem.GetToggle();
        toggle.SetParent(null);
        toggle.SetToggled(false);
    }
    private ObjectId? _selectedItemObjectId;
    private IMechLabDraggableItem? selectedCloneFixForMechBay;
    internal void OnButtonClicked(ref IMechLabDraggableItem item)
    {
        if (_widget.itemPrefabName == MechBayPanel.storageListPrefabName)
        {
            var previousSelection = selectedCloneFixForMechBay;
            selectedCloneFixForMechBay = PooledInstantiate(item, false, false);
            selectedCloneFixForMechBay.GameObject.transform.SetParent(SharedGameObjects.ContainerTransform);

            item = selectedCloneFixForMechBay;

            // pooling afterwards allows a vanilla == check to properly detect selection changes
            if (previousSelection != null)
            {
                _widget.dataManager.PoolGameObject(_widget.itemPrefabName, previousSelection.GameObject);
            }
        }
        _selectedItemObjectId = GetId(item);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal IMechLabDraggableItem? GetMechDefByGUID(string GUID)
    {
        if (string.IsNullOrEmpty(GUID))
        {
            return null;
        }

        return _inventory
            .Where(i => i.HasGUID(GUID))
            .Select(GetTempMechLabItem)
            .FirstOrDefault();
    }

    internal IMechLabDraggableItem? GetInventoryItem(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        return _inventory
            .Where(i => i.HasId(id))
            .Select(GetTempMechLabItem)
            .FirstOrDefault();
    }

    internal void FilterAndSort(bool reset)
    {
        _sortedAndFilteredInventory =_inventory
            .Where(item =>
            {
                if (_widget.useStockFilter && !_widget.filterEnabledStock && item.MechDef != null && !item.MechDef.MechTags.Contains(MechValidationRules.MechTag_Custom))
                {
                    return false;
                }

                switch (item.ChassisDef.weightClass)
                {
                    case WeightClass.LIGHT when !_widget.filterEnabledLight:
                    case WeightClass.MEDIUM when !_widget.filterEnabledMedium:
                    case WeightClass.HEAVY when !_widget.filterEnabledHeavy:
                    case WeightClass.ASSAULT when !_widget.filterEnabledAssault:
                        return false;
                }
                return true;
            })
            .ToList();

        MechBayDynamicSorting.Sort(_sortedAndFilteredInventory);

        Log.Main.Trace?.Log($"_inventory.Count={_inventory.Count}");
        Log.Main.Trace?.Log($"_sortedAndFilteredInventory.Count={_sortedAndFilteredInventory.Count}");

        if (reset)
        {
            _scrollRect.verticalNormalizedPosition = 1.0f;
            _rowIndexForLoadingData = 0;
        } // TODO how to handle small modifications such as add/remove? see HBSInventoryLoopingListView or HBSLoopScrollRect

        var rowCount = Mathf.CeilToInt(_sortedAndFilteredInventory.Count / (float)_rowCellCount);
        _rowCountBelowScreen = Mathf.Max(0,rowCount - _screenRowCount);
        _rowIndexMaxForLoadingData = Mathf.Max(0, _rowCountBelowScreen - RowCountToPreloadAsBuffer);
        _rowIndexForLoadingData = Mathf.Clamp(_rowIndexForLoadingData, 0, _rowIndexMaxForLoadingData);

        Render();
    }

    private float _lastVerticalNormalizedPosition;
    internal void ScrollRectLateUpdate()
    {
        if (Mathf.Approximately(_lastVerticalNormalizedPosition, _scrollRect.verticalNormalizedPosition))
        {
            return;
        }
        _lastVerticalNormalizedPosition = _scrollRect.verticalNormalizedPosition;

        var sanerScrollPosition = 1.0f - _scrollRect.verticalNormalizedPosition;
        var newRowForLoadingData = Mathf.Clamp(
            Mathf.FloorToInt(_rowCountBelowScreen * sanerScrollPosition),
            0,
            _rowIndexMaxForLoadingData
        );

        if (_rowIndexForLoadingData != newRowForLoadingData)
        {
            _rowIndexForLoadingData = newRowForLoadingData;
            Render();
        }
    }

    private void Render()
    {
        var slicedList = _sortedAndFilteredInventory
            .Skip(_rowIndexForLoadingData * _rowCellCount)
            .Take((_screenRowCount + RowCountToPreloadAsBuffer) * _rowCellCount)
            .ToList();

        // re-use existing items as is
        Dictionary<int, IMechLabDraggableItem> slicedIndexMappedToInventoryItem = new();
        {
            var inventoryItemsByObjectId = _widget.inventory.ToDictionary(GetId);
            var inventoryItemsBeingReused = new HashSet<ObjectId>();
            for (var index = 0; index < slicedList.Count; index++)
            {
                var fakeItem = slicedList[index];
                if (inventoryItemsByObjectId.TryGetValue(GetId(fakeItem), out var inventoryItem))
                {
                    slicedIndexMappedToInventoryItem[index] = inventoryItem;
                    inventoryItemsBeingReused.Add(GetId(inventoryItem));
                }
            }

            for (var index = _widget.inventory.Count - 1; index >= 0; index--)
            {
                var inventoryItem = _widget.inventory[index];
                if (!inventoryItemsBeingReused.Contains(GetId(inventoryItem)))
                {
                    PoolInventory(index, index);
                }
            }
        }

        for (var index=0; index<slicedList.Count; index++)
        {
            var fakeItem = slicedList[index];
            if (slicedIndexMappedToInventoryItem.TryGetValue(index, out var inventoryItem))
            {
                inventoryItem.GameObject.transform.SetAsLastSibling();
            }
            else
            {
                PooledInstantiate(fakeItem, true, true);
            }
        }

        var gridSizeY = Mathf.RoundToInt(_grid.cellSize.y + _grid.spacing.y);
        const int fixedPaddingY = 18;

        var padding = _grid.padding;
        padding.top = fixedPaddingY + gridSizeY * _rowIndexForLoadingData;
        padding.bottom = fixedPaddingY + gridSizeY * (_rowIndexMaxForLoadingData - _rowIndexForLoadingData);
        _grid.padding = padding;

        LayoutRebuilder.MarkLayoutForRebuild(_grid.GetComponent<RectTransform>());

        Log.Main.Trace?.Log($"Render {nameof(_rowIndexMaxForLoadingData)}={_rowIndexMaxForLoadingData} {nameof(_rowIndexForLoadingData)}={_rowIndexForLoadingData} padding={padding}");
    }

    private IMechLabDraggableItem PooledInstantiate(IMechLabDraggableItem fakeItem, bool addToItemListParent, bool addItemToRadioSet)
    {
        var gameObject = _widget.dataManager.PooledInstantiate(_widget.itemPrefabName, BattleTechResourceType.UIModulePrefabs);

        var inventoryItem = GetInventoryItemComponent(gameObject, fakeItem.MechDef == null);
        SetInventoryItemData(inventoryItem, fakeItem);

        if (addItemToRadioSet)
        {
            inventoryItem.GetToggle().SetParent(_widget.listRadioSet);
        }

        if (addToItemListParent)
        {
            gameObject.transform.SetParent(_widget.itemListParent, false);
            _widget.inventory.Add(inventoryItem);
            ItemStateRestoreOnInstantiate(inventoryItem);
        }

        gameObject.SetActive(true);
        return inventoryItem;
    }

    private static IMechLabDraggableItem GetInventoryItemComponent(GameObject gameObject, bool isChassis)
    {
        if (isChassis)
        {
            return gameObject.GetComponent<MechBayChassisUnitElement>();
        }
        return (IMechLabDraggableItem)gameObject.GetComponent<LanceLoadoutMechItem>() ?? gameObject.GetComponent<MechBayMechUnitElement>();
    }

    private void SetInventoryItemData(IMechLabDraggableItem inventoryItem, IMechLabDraggableItem fakeItem)
    {
        if (inventoryItem is LanceLoadoutMechItem mechItem)
        {
            mechItem.SetData(_widget, _widget.dataManager, fakeItem.MechDef, true);
        }
        else if (inventoryItem is MechBayMechUnitElement mechUnitElement)
        {
            SetDataForMechUnitElement(mechUnitElement, fakeItem);
        }
        else if (inventoryItem is MechBayChassisUnitElement chassisUnitElement)
        {
            var chassisDef = fakeItem.ChassisDef;
            var chassisQuantity = _widget.Sim?.GetItemCount(chassisDef.Description.Id, typeof(MechDef), SimGameState.ItemCountType.UNDAMAGED_ONLY) ?? 0;
            chassisUnitElement.SetData(_widget, chassisDef, _widget.dataManager, chassisDef.MechPartCount, chassisDef.MechPartMax, chassisQuantity);
        }
        else
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    private void SetDataForMechUnitElement(MechBayMechUnitElement mechUnitElement, IMechLabDraggableItem fakeItem)
    {
        bool hasFieldableWarnings, isMaintenance, isFieldable;
        if (_mechUnitValidationEnabled)
        {
            isMaintenance = !MechValidationRules.ValidateSimGameMechNotInMaintenance(_widget.Sim, fakeItem.MechDef);

            if (_mechUnitValidationLevel == null)
            {
                // partially from MechBayMechInfoWidget.SetDescriptions
                hasFieldableWarnings = MechValidationRules.GetMechFieldableWarnings(_widget.dataManager, fakeItem.MechDef).Count > 0;
                isFieldable = MechValidationRules.ValidateMechCanBeFielded(_widget.Sim, fakeItem.MechDef);
            }
            else
            {
                var errors = MechValidationRules.ValidateMechDef(_mechUnitValidationLevel.Value, _widget.dataManager, fakeItem.MechDef, null);
                var validations = errors
                    .Where(kv => kv.Value.Count > 0)
                    .Select(kv => kv.Key)
                    .ToList();
                hasFieldableWarnings = validations.Any(errorType => _mechUnitValidationWarnings.Contains(errorType));
                isFieldable = validations.All(errorType => _mechUnitValidationWarnings.Contains(errorType));
                if (hasFieldableWarnings || !isFieldable)
                {
                    Log.Main.Debug?.Log(
                        $"mech={fakeItem.MechDef.Description.Id} hasFieldableWarnings={hasFieldableWarnings} isFieldable={isFieldable} errors:"
                        + errors.Aggregate("", (prev, kv) => $"{prev}\n{kv.Key}: {kv.Value.JoinAsString()}")
                    );
                }
            }
        }
        else
        {
            hasFieldableWarnings = false;
            isMaintenance = false;
            isFieldable = true;
        }

        mechUnitElement.SetData(_widget, _widget.dataManager, 0, fakeItem.MechDef, isMaintenance, isFieldable, hasFieldableWarnings, true, true);

        // from SkirmishMechBayPanel.RefreshMechList
        var shouldShow = !fakeItem.MechDef.MechTags.Contains(MechValidationRules.MechTag_Custom);
        mechUnitElement.SetFrameColor(shouldShow ? UIColor.StockMech : UIColor.White);
        mechUnitElement.ShowStockIcon(shouldShow);
    }

    // (?) used by MechBayPanel SwapMechUnitElements -> OnRemoveItem + OnAddItem | this could be another OnAddItem
    // used by LanceConfigurationPanel
    internal bool OnAddItem(IMechLabDraggableItem item, bool validate)
    {
        if (item.ItemType != MechLabDraggableItemType.Mech && item.ItemType != MechLabDraggableItemType.Chassis)
        {
            return false;
        }

        if (item is not LanceLoadoutMechItem)
        {
            throw new ArgumentException();
        }

        if (SetAvailableIfFound(item))
        {
            return false;
        }

        Log.Main.Trace?.Log($"Adding new item {item.MechDef.Description.Id}");
        _inventory.Add(new(item.MechDef));
        PoolItem(item);
        _widget.SetSorting();
        return true;
    }

    // (?) used by MechBayPanel SwapMechUnitElements -> OnRemoveItem + OnAddItem | this could be another OnRemoteItem
    // (?) used by MechBayPanel OnItemGrab -> RemoveFromParent -> OnRemoveItem | this could be another OnRemoteItem
    // (/) used by LanceConfigurationPanelOnItemGrab -> RemoveFromParent -> OnRemoveItem
    private readonly bool _testDuplicationInSkirmish = false;
    internal bool OnRemoveItem(IMechLabDraggableItem item)
    {
        if (_widget.allowRemovingItems)
        {
            for (var index=0; index<_widget.inventory.Count; index++)
            {
                var inventoryItem = _widget.inventory[index];
                if (GetId(inventoryItem) == GetId(item))
                {
                    PoolInventory(index, index);
                    break;
                }
            }
            _widget.SetSorting();
            return true;
        }

        if (item is LanceLoadoutMechItem mechItem
            && _widget.ParentDropTarget is LanceConfiguratorPanel panel
            && (!panel.allowDuplicateMechs || _testDuplicationInSkirmish)
        ) {
            SetAvailable(new(mechItem.mechDef), false);
        }
        return false;
    }

    internal LanceLoadoutMechItem CreateLanceItem(MechDef mechDef)
    {
        return (LanceLoadoutMechItem)GetTempMechLabItem(new FakeItem(mechDef));
    }

    internal void OnItemGrab(ref IMechLabDraggableItem item)
    {
        item = GetTempMechLabItem(item);
    }

    private IMechLabDraggableItem? _tmpItem;
    private IMechLabDraggableItem GetTempMechLabItem(IMechLabDraggableItem baseItem)
    {
        if (_tmpItem == null || _tmpItem.GameObject.transform.parent != null)
        {
            Log.Main.Trace?.Log($"Pooling new tmpItem based on {baseItem}");
            _tmpItem = PooledInstantiate(baseItem, false, true);
        }
        SetInventoryItemData(_tmpItem, baseItem);
        return _tmpItem;
    }

    // if vanilla used MechDef.GUID -> use MechDef.GUID
    // if vanilla used direct references as in "item" ->
    //   due to cloning/pooling, workaround is to use MechDef/ChassisDef references instead
    //   ObjectId is a wrapper in case the MechDef/ChassisDef reference solution does not pan out
    private ObjectId GetId(IMechLabDraggableItem item)
    {
        if (item.MechDef != null)
        {
            return new(item.MechDef);
        }
        return new(item.ChassisDef);
    }
}