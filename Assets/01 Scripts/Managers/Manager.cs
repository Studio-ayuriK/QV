using UnityEngine;

public class Manager : MonoBehaviour {
    private static Manager Instance;

    // Instances
    private static SceneManager SceneManagerInstance;
    private static InteractManager InteractManagerInstance;
    private static UIManager UIManagerInstance;

    // Getter
    public static SceneManager Scene { get { return SceneManagerInstance; } }
    public static InteractManager Interact { get { return InteractManagerInstance; } }
    public static UIManager UI { get { return UIManagerInstance; } }


    void Start() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        SceneManagerInstance = new SceneManager();
        InteractManagerInstance = new InteractManager();
        UIManagerInstance = new UIManager();
    }

    void Update() {
        
    }
}
