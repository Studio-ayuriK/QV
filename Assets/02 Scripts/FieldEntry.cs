using UnityEngine;

public class FieldEntry : MonoBehaviour, IInteractable
{
    public string sceneToLoad;

    void Start() {
        
    }

    void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Manager.Interact.AddInteractable(this);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            Manager.Interact.RemoveInteractable(this);
        }
    }

    public void Interact() {
        Manager.Scene.LoadScene(sceneToLoad);
    }
}
