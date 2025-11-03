using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 0, -10);

    private void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = offset.z; // Giữ z cố định
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}
