using UnityEngine;

public class StaminaManager : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 20f;      // per second while sprinting
    [SerializeField] private float staminaRegenRate = 15f;      // per second while idle
    [SerializeField] private float staminaRegenDelay = 1f;      // delay before regen starts
    [SerializeField] private float minStaminaToSprint = 10f;    // minimum to start sprinting again

    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;
    public float StaminaPercent => maxStamina > 0 ? CurrentStamina / maxStamina : 0f;
    public bool IsExhausted { get; private set; }

    private float lastDrainTime;

    private void Start()
    {
        CurrentStamina = maxStamina;
    }

    private void Update()
    {
        HandleRegeneration();
    }

    /// <summary>
    /// Attempt to drain stamina. Returns true if stamina was available.
    /// </summary>
    public bool TryDrainStamina(float deltaTime)
    {
        if (CurrentStamina <= 0f && IsExhausted)
            return false;

        CurrentStamina -= staminaDrainRate * deltaTime;
        lastDrainTime = Time.time;

        if (CurrentStamina <= 0f)
        {
            CurrentStamina = 0f;
            IsExhausted = true;
            Debug.Log("[STAMINA] Exhausted!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Check if player has enough stamina to sprint.
    /// </summary>
    public bool CanSprint()
    {
        if (IsExhausted)
            return CurrentStamina >= minStaminaToSprint;

        return CurrentStamina > 0f;
    }

    private void HandleRegeneration()
    {
        // Only regenerate if enough time has passed since last drain
        if (Time.time - lastDrainTime >= staminaRegenDelay)
        {
            CurrentStamina += staminaRegenRate * Time.deltaTime;
            CurrentStamina = Mathf.Min(CurrentStamina, maxStamina);

            // Clear exhausted state when stamina is recovered enough
            if (IsExhausted && CurrentStamina >= minStaminaToSprint)
            {
                IsExhausted = false;
            }
        }
    }

    /// <summary>
    /// Force set stamina to a specific value (for debugging/testing).
    /// </summary>
    public void SetStamina(float value)
    {
        CurrentStamina = Mathf.Clamp(value, 0f, maxStamina);
        if (CurrentStamina > minStaminaToSprint)
            IsExhausted = false;
    }
}
