using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 1.8f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 150f;

    private CharacterController controller;
    private Camera playerCamera;
    private StaminaManager stamina;

    private float yVelocity;
    private float xRotation;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        stamina = FindFirstObjectByType<StaminaManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (stamina == null)
            Debug.LogWarning("[PlayerController] StaminaManager not found in scene!");
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMovement()
    {
        // WASD (Input System)
        Vector2 moveInput = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        }

        bool wantsToSprint = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        // Check if player can sprint using StaminaManager
        bool canSprint = stamina != null && stamina.CanSprint();
        bool isSprinting = wantsToSprint && isMoving && canSprint;

        // Drain stamina if sprinting
        if (isSprinting && stamina != null)
        {
            stamina.TryDrainStamina(Time.deltaTime);
        }

        float speed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        move = Vector3.ClampMagnitude(move, 1f);

        // Ground / Jump
        if (controller.isGrounded)
        {
            if (yVelocity < 0) yVelocity = -2f;

            bool jumpPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
            if (jumpPressed)
                yVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        yVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * speed + Vector3.up * yVelocity;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        if (Mouse.current == null) return;

        Vector2 delta = Mouse.current.delta.ReadValue();
        float mouseX = delta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = delta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
