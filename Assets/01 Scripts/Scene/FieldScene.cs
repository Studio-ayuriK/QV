using UnityEngine;

public class FieldScene : SceneBase {
    void Start() {
        Init();

        cameraPitchYaw = new Vector2(40.0f, 45.0f);
        camera.SetPitchYaw(cameraPitchYaw.x, cameraPitchYaw.y);
    }

    void Update() {
     
    }
}
