using System.Collections.Generic;
using UnityEngine;

public class UIManager : ManagerBase {
    private List<UIBase> _instancedUIs = new();
    private List<UIBase> _registeredUIs = new();

    public void RegisterUI(UIBase ui) {
        _registeredUIs.Add(ui);

        ui.Close();
    }

    public void UnregisterUI(UIBase ui) {
        _registeredUIs.Remove(ui);
    }

    public T OpenUI<T>() where T : UIBase {
        foreach (UIBase ui in _registeredUIs) {
            if (ui.GetType() == typeof(T)) {
                _instancedUIs.Add(ui);
                ui.Open();
                return ui as T;
            }
        }

        return null;
    }

    public bool CloseUI<T>() {
        foreach (UIBase ui in _instancedUIs) {
            if (ui.GetType() == typeof(T)) {
                _instancedUIs.Remove(ui);
                ui.Close();
                return true;
            }
        }

        return false;
    }
}
