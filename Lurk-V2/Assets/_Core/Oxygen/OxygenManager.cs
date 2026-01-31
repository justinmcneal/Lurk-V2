using UnityEngine;

public class OxygenManager : MonoBehaviour
{
    [SerializeField] private OxygenConfig config;

    public float CurrentOxygen { get; private set; }
    public float MaxOxygen { get; private set; }
    public float OxygenPercentRemaining => MaxOxygen <= 0f ? 0f : CurrentOxygen / MaxOxygen;

    public int TanksCollected { get; private set; }

    // ✅ ADD THESE
    public int QuotaTanksRequired => config != null ? config.quotaTanksRequired : 0;
    public bool QuotaMet => config != null && TanksCollected >= config.quotaTanksRequired;

    private void Start()
    {
        if (config == null)
        {
            Debug.LogError("OxygenManager: No OxygenConfig assigned!");
            enabled = false;
            return;
        }

        CurrentOxygen = config.startOxygenSeconds;
        MaxOxygen = config.startOxygenSeconds;
        TanksCollected = 0;

        Debug.Log($"[OXYGEN] Start: {CurrentOxygen:0}s | Quota: {config.quotaTanksRequired} tanks");
    }

    private void Update()
    {
        CurrentOxygen -= config.drainPerSecond * Time.deltaTime;
        if (CurrentOxygen <= 0f)
        {
            CurrentOxygen = 0f;

            Debug.Log("[OXYGEN] Depleted -> DEAD");
            FindFirstObjectByType<RunStateManager>()?.OnPlayerDied();

            enabled = false; // stop draining
        }
    }

    public void AddTank()
    {
        TanksCollected++;
        CurrentOxygen = Mathf.Min(CurrentOxygen + config.tankAddSeconds, config.maxOxygenSeconds);

        Debug.Log($"[OXYGEN] Tank picked! Tanks: {TanksCollected}/{config.quotaTanksRequired} | Oxygen: {CurrentOxygen:0}s");

        if (QuotaMet)
        {
            Debug.Log("[OXYGEN] Quota met! Go to Terminal to submit.");
        }
    }
}
