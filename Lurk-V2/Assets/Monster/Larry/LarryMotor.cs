using UnityEngine;

public class LarryMotor : MonoBehaviour
{
    [Header("Speeds")]
    [Tooltip("Phase 2: Slow, methodical movement")]
    public float investigateSpeed = 1.4f;
    
    [Tooltip("Phase 3: Aggressive pursuit speed")]
    public float huntSpeed = 3.8f;

    [Header("Turn")]
    [Tooltip("Phase 2 turn speed (slower, creepier)")]
    public float investigateTurnSpeed = 180f;
    
    [Tooltip("Phase 3 turn speed (snappy, aggressive)")]
    public float huntTurnSpeed = 400f;

    [Header("Movement Feel")]
    [SerializeField] private float accelerationTime = 0.3f;
    
    private float currentSpeed = 0f;

    // speedMultiplier lets Charge move faster without changing huntSpeed permanently
    public void MoveTowards(Vector3 target, bool hunting, float speedMultiplier = 1f)
    {
        Vector3 to = target - transform.position;
        to.y = 0f;

        if (to.sqrMagnitude < 0.01f)
            return;

        // Choose turn speed based on mode
        float turnSpeed = hunting ? huntTurnSpeed : investigateTurnSpeed;

        // Rotate toward target
        Quaternion targetRot = Quaternion.LookRotation(to.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        // Calculate target speed
        float baseSpeed = hunting ? huntSpeed : investigateSpeed;
        float targetSpeed = baseSpeed * speedMultiplier;

        // Smooth acceleration for more natural movement
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, (targetSpeed / accelerationTime) * Time.deltaTime);

        // Move forward
        transform.position += transform.forward * (currentSpeed * Time.deltaTime);
    }

    public void Stop()
    {
        currentSpeed = 0f;
    }
}