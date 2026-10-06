using UnityEngine;

public abstract class SceneBase : MonoBehaviour {
    protected TPVCamera camera;
    [SerializeField] protected Vector2 cameraPitchYaw;

    protected void Init() {
        camera = FindAnyObjectByType<TPVCamera>();
    }
}
