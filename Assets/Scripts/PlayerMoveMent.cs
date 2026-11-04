using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Handles 3D player movement (forward, backward, left, right) and updates
/// walking and idle animations via Unity's Animator component.
/// Works with both the New Input System and the Legacy Input Manager.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMoveMent : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed in meters per second.")]
    [SerializeField] private float moveSpeed = 4f;

    [Tooltip("How fast the character rotates to face the moving direction.")]
    [SerializeField] private float rotationSpeed = 12f;

    [Tooltip("Gravity acceleration applied when in air.")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Camera Relative Settings")]
    [Tooltip("If enabled, W/S/A/D moves relative to the camera view direction.")]
    [SerializeField] private bool useCameraDirection = true;

    [Tooltip("Reference to the main camera transform. Automatically assigned to Camera.main if left empty.")]
    [SerializeField] private Transform cameraTransform;

    [Header("Animation Settings")]
    [Tooltip("Reference to the Animator. Automatically found on this object or its children if left empty.")]
    [SerializeField] private Animator animator;

    [Tooltip("Name of the boolean parameter in the Animator Controller for walking (true = walking, false = idle).")]
    [SerializeField] private string isWalkingParameter = "isWalking";

    [Tooltip("Name of the float parameter in the Animator Controller for movement speed (0 = idle, 1 = full speed).")]
    [SerializeField] private string speedParameter = "Speed";

    private CharacterController characterController;
    private float verticalVelocity;

    // Animator parameter hashes
    private int isWalkingHash;
    private int speedHash;
    private bool hasIsWalkingParam;
    private bool hasSpeedParam;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        // Auto-detect Animator on this GameObject or child models
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
        }

        // Auto-assign camera if needed
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        CacheAnimatorParameters();
    }

    private void Start()
    {
        // Re-check camera in Start if not available during Awake
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Re-cache parameters in case AnimatorController was assigned dynamically
        CacheAnimatorParameters();
    }

    private void Update()
    {
        Vector2 input = GetMovementInput();
        Vector3 moveDirection = CalculateMoveDirection(input);

        ApplyMovement(moveDirection);
        ApplyRotation(moveDirection);
        ApplyGravity();
        UpdateAnimations(input.magnitude);
    }

    /// <summary>
    /// Reads movement input from keyboard (WASD / Arrows) and Gamepad.
    /// Supports New Input System and Legacy Input Manager.
    /// </summary>
    private Vector2 GetMovementInput()
    {
        Vector2 input = Vector2.zero;

#if ENABLE_INPUT_SYSTEM
        // New Input System
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
        }

        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.leftStick.ReadValue();
            if (stick.sqrMagnitude > 0.01f)
            {
                input = stick;
            }
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        // Legacy Input Manager fallback
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
#else
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
#endif

        return Vector2.ClampMagnitude(input, 1f);
    }

    /// <summary>
    /// Calculates horizontal movement direction in world or camera-relative space.
    /// </summary>
    private Vector3 CalculateMoveDirection(Vector2 input)
    {
        if (input.sqrMagnitude < 0.001f)
        {
            return Vector3.zero;
        }

        if (useCameraDirection && cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            // Project onto the horizontal plane (ignore vertical tilt)
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            return (camRight * input.x + camForward * input.y).normalized;
        }

        // Direct world space: X = left/right (A/D), Z = forward/backward (W/S)
        return new Vector3(input.x, 0f, input.y).normalized;
    }

    /// <summary>
    /// Moves the character horizontally using CharacterController.
    /// </summary>
    private void ApplyMovement(Vector3 moveDirection)
    {
        Vector3 velocity = moveDirection * moveSpeed;
        velocity.y = verticalVelocity;

        characterController.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Smoothly rotates the character to face the direction of movement.
    /// </summary>
    private void ApplyRotation(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Handles downward gravity acceleration and grounded states.
    /// </summary>
    private void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            // Small downward velocity to keep grounded against uneven slopes
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// Updates walking and idle states in the Animator.
    /// </summary>
    private void UpdateAnimations(float inputMagnitude)
    {
        if (animator == null) return;

        bool isWalking = inputMagnitude > 0.05f;

        if (hasIsWalkingParam)
        {
            animator.SetBool(isWalkingHash, isWalking);
        }

        if (hasSpeedParam)
        {
            animator.SetFloat(speedHash, inputMagnitude, 0.1f, Time.deltaTime);
        }
    }

    /// <summary>
    /// Caches animator parameter hashes and checks which parameters exist on the controller.
    /// </summary>
    private void CacheAnimatorParameters()
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;

        isWalkingHash = Animator.StringToHash(isWalkingParameter);
        speedHash = Animator.StringToHash(speedParameter);

        hasIsWalkingParam = HasParameter(animator, isWalkingParameter);
        hasSpeedParam = HasParameter(animator, speedParameter);
    }

    private bool HasParameter(Animator anim, string paramName)
    {
        if (string.IsNullOrEmpty(paramName) || anim.runtimeAnimatorController == null)
            return false;

        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName)
            {
                return true;
            }
        }

        return false;
    }
}
