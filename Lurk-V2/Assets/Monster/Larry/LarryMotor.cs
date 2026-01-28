using UnityEngine;

public class LarryMotor : MonoBehaviour
{
    [Header("Speeds")]
    public float investigateSpeed = 1.6f;
    public float huntSpeed = 3.4f;

    [Header("Turn")]
    public float turnSpeed = 360f; // degrees/sec

    // speedMultiplier lets Charge move faster without changing huntSpeed permanently
    public void MoveTowards(Vector3 target, bool hunting, float speedMultiplier = 1f)
    {
        Vector3 to = target - transform.position;
        to.y = 0f;

        if (to.sqrMagnitude < 0.01f)
            return;

        // Rotate toward target
        Quaternion targetRot = Quaternion.LookRotation(to.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);

        // Move forward
        float baseSpeed = hunting ? huntSpeed : investigateSpeed;
        float speed = baseSpeed * speedMultiplier;

        transform.position += transform.forward * (speed * Time.deltaTime);
    }
}