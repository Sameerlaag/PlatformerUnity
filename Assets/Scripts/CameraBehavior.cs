using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform target; // The player
    public float followSpeed = 10f;
    public float rotationSpeed = 70f;

    public float minY = -20f;
    public float maxY = 60f;

    float mouseX;
    float mouseY;

    Transform cameraPivot;   // Empty GameObject pivot for vertical rotation
    Transform camTransform;  // Actual camera

    private void Awake()
    {
        camTransform = Camera.main.transform;
        cameraPivot = camTransform.parent; // assumes Camera is parented under pivot
    }

    public void HandleCameraMovement(float horizontalInput, float verticalInput, float zoomInput)
    {
        // Follow player
        Vector3 targetPos = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
        transform.position = targetPos;

        // Rotate with input
        mouseX += horizontalInput * rotationSpeed * Time.deltaTime;
        mouseY -= verticalInput * rotationSpeed * Time.deltaTime;
        mouseY = Mathf.Clamp(mouseY, minY, maxY);

        transform.rotation = Quaternion.Euler(0, mouseX, 0);
        cameraPivot.localRotation = Quaternion.Euler(mouseY, 0, 0);
    }
}
