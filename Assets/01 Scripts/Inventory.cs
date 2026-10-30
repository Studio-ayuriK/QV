using System.Collections.Generic;
using UnityEngine;

public struct ItemSlot {
    public SOItem itemData;
    public int quantity;

    public ItemSlot(SOItem itemData) {
        this.itemData = itemData;
        this.quantity = 0;
    }

    public void IncreaseQuantity(int mount = 1) {
        this.quantity += mount;
    }

    public void DecreaseQuantity(int mount = 1) {
        this.quantity -= mount;
    }
}

public class Inventory
{
    // <id, ItemData>
    private Dictionary<int, ItemSlot> _items = new();

    private bool _isUIOpened = false;

    public void AddItem(SOItem item) {
        if (!_items.ContainsKey(item.id)) _items.Add(item.id, new ItemSlot(item));
        _items[item.id].IncreaseQuantity();
    }

    public void RemoveItem(SOItem item) {
        _items[item.id].DecreaseQuantity();
        if (_items[item.id].quantity == 0) _items.Remove(item.id);
    }

    public void ToggleUI() {
        _isUIOpened = !_isUIOpened;
        if (_isUIOpened) {
            UIInventory inventoryUI = Manager.UI.OpenUI<UIInventory>();
            inventoryUI.UpdateItemList(_items);
        }
        else {
            Manager.UI.CloseUI<UIInventory>();
        }
    }

    // Debug
    public void CheckInventory() {
        for (int i = 0; i < _items.Count; i++) {
            Debug.Log(_items[i].quantity);
        }
    }
}
