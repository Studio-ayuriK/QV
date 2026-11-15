using Unity.Mathematics;
using UnityEngine;

public class TPVCamera : MonoBehaviour {
    public Transform target;
    public float distance = 30.0f;
    public float cameraMovementSmoothness = 0.5f;

    [SerializeField] private float _pitch = 40.0f;
    [SerializeField] private float _yaw = 45.0f;

    private Vector3 _destPos;
    private Vector3 _armVector;
    private FadeObstacle _currentObstacle;

    void Start() {
        _destPos = target.position - _armVector * distance;
        transform.position = _destPos;
    }

    void Update() {
        _destPos = target.position - _armVector * distance;
        if ((transform.position - _destPos).magnitude > 0.001f)
        transform.position = Vector3.Lerp(transform.position, _destPos, cameraMovementSmoothness);
    }

    void LateUpdate() {
        // 장애물 투명화 기능
        Vector3 dir = target.position - transform.position;

        if (Physics.Raycast(transform.position, dir.normalized,
            out RaycastHit hit, dir.magnitude, LayerMask.GetMask("Obstacles"))) {
            FadeObstacle fade = hit.collider.GetComponent<FadeObstacle>();
            if (fade == null)
                fade = hit.transform.GetComponentInParent<FadeObstacle>();

            if (fade != null && fade != _currentObstacle) {
                // New obstacle: fade old one in, new one out
                if (_currentObstacle != null)
                    _currentObstacle.FadeIn();

                fade.FadeOut();
                _currentObstacle = fade;
            }
        }
        else {
            // Nothing blocking: fade previous obstacle back in
            if (_currentObstacle != null) {
                _currentObstacle.FadeIn();
                _currentObstacle = null;
            }
        }
    }

    public void SetPitchYaw(float pitch, float yaw) {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        _armVector = transform.rotation * Vector3.forward;
    }
}
