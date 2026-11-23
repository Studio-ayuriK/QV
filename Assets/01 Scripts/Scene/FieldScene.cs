using UnityEngine;

public class FieldScene : SceneBase {
    private Transform _startPoint;

    void Start() {
        Init();

        cameraPitchYaw = new Vector2(40.0f, 45.0f);
        camera.SetPitchYaw(cameraPitchYaw.x, cameraPitchYaw.y);

        _startPoint = GameObject.FindWithTag("StartPoint").transform;
        GameObject.FindWithTag("Player").transform.position = _startPoint.position;
    }

    void Update() {
     
    }
}
