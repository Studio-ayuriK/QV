using UnityEngine;

public class Manager : MonoBehaviour {
    private static Manager Instance;

    private static SceneManager SceneManagerInstance;
    private static InteractManager InteractManagerInstance;

    public static SceneManager Scene { get { return SceneManagerInstance; } }
    public static InteractManager Interact { get { return InteractManagerInstance; } }

    void Start() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        SceneManagerInstance = new SceneManager();
        InteractManagerInstance = new InteractManager();
    }

    void Update() {
        
    }
}
