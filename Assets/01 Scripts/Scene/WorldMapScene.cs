using UnityEngine;

public class WorldMapScene : SceneBase {
    void Start() {
        Init();

        cameraPitchYaw = new Vector2(40.0f, 0.0f);
        camera.SetPitchYaw(cameraPitchYaw.x, cameraPitchYaw.y);
    }

    void Update() {

    }
}
