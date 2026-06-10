using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump")]
[SerializeField] private float jumpForce = 7f;
[SerializeField] private Transform groundCheck;
[SerializeField] private float groundCheckRadius = 0.2f;
[SerializeField] private LayerMask groundLayer;

[Header("Glide")]
[SerializeField] private float glideGravityMultiplier = 0.3f;
[SerializeField] private float normalGravityMultiplier = 1f;

private bool isGliding;
private bool isSprinting;
private bool jumpHeld;
private bool isGrounded;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;


    private Rigidbody rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
{
    if (context.performed)
    {
        jumpHeld = true;

        if (isGrounded)
        {
            Jump();
        }
    }

    if (context.canceled)
    {
        jumpHeld = false;
        isGliding = false;
    }
}

public void Sprint(InputAction.CallbackContext context)
{
    if (context.performed)
    {
        isSprinting = true;
    }

    if (context.canceled)
    {
        isSprinting = false;
    }
}
    private void FixedUpdate()
    {
        CheckGround();

        HandleGlide();
        animator.SetBool("IsGliding", isGliding);
        Move();
    }

private void HandleGlide()
{
    if (isGrounded)
    {
        isGliding = false;
        return;
    }

    // On ne plane que pendant la descente
    if (jumpHeld && rb.linearVelocity.y < 0f)
    {
        isGliding = true;
    }
    else
    {
        isGliding = false;
    }

    if (isGliding)
    {
        Vector3 velocity = rb.linearVelocity;

        // Réduit la vitesse de chute
        velocity.y += Physics.gravity.y *
                      (glideGravityMultiplier - 1f) *
                      Time.fixedDeltaTime;

        rb.linearVelocity = velocity;
        }
        else
        {
            Vector3 velocity = rb.linearVelocity;

        // Réduit la vitesse de chute
        velocity.y += Physics.gravity.y *
                      (normalGravityMultiplier - 1f) *
                      Time.fixedDeltaTime;

        rb.linearVelocity = velocity;
        }
}
    private void CheckGround()
{
    isGrounded = Physics.CheckSphere(
        groundCheck.position,
        groundCheckRadius,
        groundLayer
    );

    animator.SetBool("IsInAir", !isGrounded);
}

private void Jump()
{
    // Supprime la vitesse verticale actuelle
    Vector3 velocity = rb.linearVelocity;
    velocity.y = 0f;
    rb.linearVelocity = velocity;

    // Applique l'impulsion
    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    animator.SetTrigger("Jump");
}
    private void Move()
    {
        // Direction caméra
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Ignore la hauteur
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection =
            forward * moveInput.y +
            right * moveInput.x;

        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        Vector3 targetVelocity;
        if(isSprinting)
        targetVelocity = moveDirection * sprintSpeed;
        else targetVelocity = moveDirection * moveSpeed;

        if(moveDirection.magnitude <= 0.1f) animator.SetBool("IsMoving",false);
        else animator.SetBool("IsMoving",true);
        animator.SetBool("IsSprinting",isSprinting);

        rb.linearVelocity = new Vector3(
            targetVelocity.x,
            rb.linearVelocity.y,
            targetVelocity.z
        );

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
            Quaternion.LookRotation(moveDirection);

            rb.MoveRotation(
                 Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                 )
            );
        }
    }
}
