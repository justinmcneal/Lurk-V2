using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNoiseEmitter : MonoBehaviour
{
    [Header("Step Noise")]
    [Tooltip("How often footstep noise can emit while moving (seconds).")]
    [SerializeField] private float stepIntervalWalk = 0.55f;

    [Tooltip("How often footstep noise can emit while sprinting (seconds).")]
    [SerializeField] private float stepIntervalSprint = 0.35f;

    [Header("Noise Values")]
    [SerializeField] private float walkRadius = 12f;
    [SerializeField] private float sprintRadius = 25f;
    [SerializeField] private float jumpRadius = 18f;
    [SerializeField] private float grabRadius = 10f;

    [SerializeField] private float walkIntensity = 0.4f;
    [SerializeField] private float sprintIntensity = 0.9f;
    [SerializeField] private float jumpIntensity = 0.7f;
    [SerializeField] private float grabIntensity = 0.35f;

    [Header("Detection")]
    [Tooltip("Minimum movement speed required to count as 'moving' for steps.")]
    [SerializeField] private float minMoveSpeedForSteps = 0.2f;

    private CharacterController cc;
    private Rigidbody rb;

    private float stepTimer;

    // Sprint + Jump input (new Input System)
    private bool isSprinting;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 1) Determine player speed (works for either CharacterController or Rigidbody)
        float speed = GetHorizontalSpeed();

        // 2) Footstep noise ticking
        if (speed >= minMoveSpeedForSteps)
        {
            stepTimer += Time.deltaTime;

            float interval = isSprinting ? stepIntervalSprint : stepIntervalWalk;

            if (stepTimer >= interval)
            {
                stepTimer = 0f;

                if (isSprinting)
                    EmitSprintStep();
                else
                    EmitWalkStep();
            }
        }
        else
        {
            // not moving -> reset timer so it feels responsive next time
            stepTimer = 0f;
        }

        // 3) Jump noise (simple MVP: Space pressed)
        // If your controller already has jump logic, you’ll later call EmitJump() from that script instead.
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            EmitJump();
        }

        // 4) Grab noise (simple MVP: E pressed)
        // Later you'll call EmitGrab() from your grab / physics pickup script.
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            EmitGrab();
        }

        // 5) Sprint toggle (simple MVP: LeftShift held)
        if (Keyboard.current != null)
        {
            isSprinting = Keyboard.current.leftShiftKey.isPressed;
        }
    }

    private float GetHorizontalSpeed()
    {
        if (cc != null)
        {
            Vector3 v = cc.velocity;
            v.y = 0f;
            return v.magnitude;
        }

        if (rb != null)
        {
            Vector3 v = rb.linearVelocity;
            v.y = 0f;
            return v.magnitude;
        }

        return 0f;
    }

    private void EmitWalkStep()
    {
        NoiseBus.Emit(transform.position, walkRadius, walkIntensity, "Walk");
    }

    private void EmitSprintStep()
    {
        NoiseBus.Emit(transform.position, sprintRadius, sprintIntensity, "Sprint");
    }

    public void EmitJump()
    {
        NoiseBus.Emit(transform.position, jumpRadius, jumpIntensity, "Jump");
    }

    public void EmitGrab()
    {
        NoiseBus.Emit(transform.position, grabRadius, grabIntensity, "Grab");
    }
}
