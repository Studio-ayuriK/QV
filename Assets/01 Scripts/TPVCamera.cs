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

    void Start() {
        _destPos = target.position - _armVector * distance;
        transform.position = _destPos;
    }

    void Update() {
        _destPos = target.position - _armVector * distance;
        if ((transform.position - _destPos).magnitude > 0.001f)
        transform.position = Vector3.Lerp(transform.position, _destPos, cameraMovementSmoothness);
    }

    public void SetPitchYaw(float pitch, float yaw) {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        _armVector = transform.rotation * Vector3.forward;
    }
}
