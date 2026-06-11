using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Distance")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float height = 2f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 120f;

    [Header("Follow")]
    [SerializeField] private float smoothTime = 0.15f;

    private Vector2 lookInput;
    private Vector3 velocity;

    private float yaw;

    private void Start()
    {
        yaw = target.eulerAngles.y;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        HandleRotation();
        FollowTarget();
    }

    private void HandleRotation()
    {
        yaw += lookInput.x * rotationSpeed * Time.deltaTime;
    }

    private void FollowTarget()
    {
        Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);

        Vector3 desiredPosition =
            target.position
            - rotation * Vector3.forward * distance
            + Vector3.up * height;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime
        );

        transform.LookAt(
            target.position + Vector3.up * 1.5f
        );
    }
}
