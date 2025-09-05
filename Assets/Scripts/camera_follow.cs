using UnityEngine;

[RequireComponent(typeof(Camera))]
public class camera_follow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Transform of the player to follow")]
    public Transform target;

    [Header("Offset Settings")]
    [Tooltip("Local-space camera offset relative to the target (X=right, Y=up, Z=forward)")]
    public Vector3 offset = new Vector3(-5f, 1.5f, 0f);

    [Header("Tilt Settings")]
    [Tooltip("Downward tilt angle in degrees")]
    public float tiltAngle = 20f;

    private float fixedY;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraFollow: No target assigned. Please assign the player transform.");
            enabled = false;
            return;
        }

        // Lock the camera's initial Y position (world Y) to prevent bobbing
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        // Calculate desired position by applying local offset in the target's space
        Vector3 desiredPos = target.position + target.rotation * offset;
        desiredPos.y = fixedY;
        transform.position = desiredPos;

        // Compute flat direction from camera to target for yaw
        Vector3 flatDir = target.position - transform.position;
        flatDir.y = 0f;

        if (flatDir.sqrMagnitude > 0.001f)
        {
            // Determine yaw so camera faces the player
            float yaw = Quaternion.LookRotation(flatDir, Vector3.up).eulerAngles.y;
            transform.rotation = Quaternion.Euler(tiltAngle, yaw, 0f);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            Debug.Log("Camera target set successfully");
        }
        else
        {
            Debug.LogWarning("Camera target set to null");
        }
    }
}
