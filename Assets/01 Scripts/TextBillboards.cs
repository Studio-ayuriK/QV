using UnityEngine;

public class TextBillboards : MonoBehaviour
{
    private Quaternion cameraRotation = Quaternion.identity;

    void LateUpdate() {
        if (Camera.main.transform.rotation != cameraRotation) {
            cameraRotation = Camera.main.transform.rotation;

            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
        }
    }
}
