using UnityEngine;

[CreateAssetMenu(fileName = "ItemTest", menuName = "Scriptable Objects/ItemData")]
public class SOItem : ScriptableObject {
    public int id;
    public string name;
    public string description;
    public Sprite icon;
}
