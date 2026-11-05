using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : UIBase {
    private GameObject _itemObj;
    private Transform _inventoryFrame;

    private List<GameObject> _instancedItemObj = new();

    void Start() {
        _itemObj = Resources.Load("Prefabs/UI/Item") as GameObject;
        _inventoryFrame = transform.Find("Frame");

        Manager.UI.RegisterUI(this);
    }

    public override void Close() {
        base.Close();

        for (int i = 0; i < _instancedItemObj.Count; ++i) {
            Destroy(_instancedItemObj[i]);
        }
        _instancedItemObj.Clear();
    }

    public void UpdateItemList(Dictionary<int, ItemSlot> items) {
        foreach (var item in items.Values) {
            GameObject itemObj = Instantiate(_itemObj, _inventoryFrame);
            itemObj.GetComponent<Image>().sprite = item.itemData.icon;
            _instancedItemObj.Add(itemObj);
        }
    }
}
