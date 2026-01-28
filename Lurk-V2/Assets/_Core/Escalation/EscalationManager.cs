using System;
using System.Collections.Generic;
using UnityEngine;

public class EscalationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OxygenManager oxygen;

    [Header("Phases (order matters: Scout -> Escalate -> Chaos)")]
    [SerializeField] private List<EscalationPhaseSO> phases = new();

    public EscalationPhaseSO CurrentPhase { get; private set; }

    public event Action<EscalationPhaseSO> OnPhaseChanged;

    public event Action OnHunterAware;

    public event Action OnChaseStart;

    private float elapsedSeconds;
    
    private bool hunterAwareFired;
    
    private bool chaseStartFired;


    private void Awake()
    {
        if (oxygen == null)
            oxygen = FindFirstObjectByType<OxygenManager>();
    }

    private void Start()
    {
        if (oxygen == null)
        {
            Debug.LogError("EscalationManager: OxygenManager not found in scene.");
            enabled = false;
            return;
        }

        if (phases == null || phases.Count == 0)
        {
            Debug.LogError("EscalationManager: No phases assigned.");
            enabled = false;
            return;
        }

        // Reset state temporary for now
        hunterAwareFired = false;
        chaseStartFired = false;
        elapsedSeconds = 0f;

        // Start in first phase immediately
        SetPhase(phases[0]);
    }

    private void Update()
    {
        elapsedSeconds += Time.deltaTime;

        // Determine which phase should be active
        EscalationPhaseSO desired = DetermineDesiredPhase();

        if (desired != null && desired != CurrentPhase)
            SetPhase(desired);
    }

    private EscalationPhaseSO DetermineDesiredPhase()
    {
        // We want the "latest" phase that qualifies.
        EscalationPhaseSO best = phases[0];

        foreach (var phase in phases)
        {
            if (phase == null) continue;

            bool qualifies = phase.triggerType switch
            {
                EscalationTriggerType.OxygenPercentRemaining
                    => oxygen.OxygenPercentRemaining <= phase.activateAtOrBelow,

                EscalationTriggerType.TimeElapsedSeconds
                    => elapsedSeconds >= phase.activateAfterSeconds,

                _ => false
            };

            if (qualifies)
                best = phase;
        }

        return best;
    }

    private void SetPhase(EscalationPhaseSO newPhase)
    {
        CurrentPhase = newPhase;

        Debug.Log(
            $"[ESCALATION] Phase -> {CurrentPhase.phaseName} | " +
            $"audio={CurrentPhase.audioIntensity:0.00}, " +
            $"aggro={CurrentPhase.monsterAggression:0.00}, " +
            $"env={CurrentPhase.environmentReaction:0.00}"
        );

        OnPhaseChanged?.Invoke(CurrentPhase);
        
        // Fire one-time events based on flags
        if (!hunterAwareFired && CurrentPhase.hunterAware)
        {
            hunterAwareFired = true;
            Debug.Log("[ESCALATION] Hunter is aware!");
            OnHunterAware?.Invoke();
        }

        if (!chaseStartFired && CurrentPhase.chaseActive)
        {
            chaseStartFired = true;
            Debug.Log("[ESCALATION] Chase started!");
            OnChaseStart?.Invoke();
        }
    }
}
