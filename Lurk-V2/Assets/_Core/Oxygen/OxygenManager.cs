using UnityEngine;

public class OxygenManager : MonoBehaviour
{
    [SerializeField] private OxygenConfig config;

    public float CurrentOxygen { get; private set; }
    public int TanksCollected { get; private set; }

    private void Start()
    {
        if (config == null)
        {
            Debug.LogError("OxygenManager: No OxygenConfig assigned!");
            enabled = false;
            return;
        }

        CurrentOxygen = config.startOxygenSeconds;
        TanksCollected = 0;

        Debug.Log($"[OXYGEN] Start: {CurrentOxygen:0}s | Quota: {config.quotaTanksRequired} tanks");
    }

    private void Update()
    {
        CurrentOxygen -= config.drainPerSecond * Time.deltaTime;
        if (CurrentOxygen <= 0f)
        {
            CurrentOxygen = 0f;
            Debug.Log("[OXYGEN] Depleted -> DEAD (for now just log)");
            enabled = false; // stop draining
        }
    }

    public void AddTank()
    {
        TanksCollected++;

        CurrentOxygen = Mathf.Min(CurrentOxygen + config.tankAddSeconds, config.maxOxygenSeconds);

        Debug.Log($"[OXYGEN] Tank picked! Tanks: {TanksCollected}/{config.quotaTanksRequired} | Oxygen: {CurrentOxygen:0}s");

        if (TanksCollected >= config.quotaTanksRequired)
        {
            Debug.Log("[OXYGEN] Quota met! (Later: terminal submit / win screen)");
        }
    }
}
