using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private Dictionary<int, List<SOItem>> _items = new();

    public void AddItem(SOItem item) {
        if (!_items.ContainsKey(item.id)) _items.Add(item.id, new());
        _items[item.id].Add(item);
    }

    public void RemoveItem(SOItem item) {
        _items[item.id].Remove(item);
        if (_items[item.id].Count == 0) _items.Remove(item.id);
    }

    // Debug
    public void CheckInventory() {
        for (int i = 0; i < _items.Count; i++) {
            Debug.Log(_items[i].Count);
        }
    }
}
