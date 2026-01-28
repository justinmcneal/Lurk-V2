using UnityEngine;

public enum EscalationTriggerType
{
    OxygenPercentRemaining, // recommended for L.U.R.K (oxygen is timer)
    TimeElapsedSeconds
}

[CreateAssetMenu(fileName = "EscalationPhase", menuName = "ScriptableObjects/Escalation/Phase")]
public class EscalationPhaseSO : ScriptableObject
{
    [Header("Identity")]
    public string phaseName = "Scout"; // Scout / Escalate / Chaos
    [TextArea] public string description;

    [Header("When this phase should become active")]
    public EscalationTriggerType triggerType = EscalationTriggerType.OxygenPercentRemaining;

    [Tooltip("For OxygenPercentRemaining: phase activates when oxygenRemainingPercent <= this value.\nExample: 0.66 means switch when 66% oxygen remains.")]
    [Range(0f, 1f)] public float activateAtOrBelow = 0.66f;

    [Tooltip("For TimeElapsedSeconds: activates when elapsedSeconds >= this value.")]
    public float activateAfterSeconds = 0f;

    [Header("Tuning (systems read these)")]
    [Range(0f, 3f)] public float audioIntensity = 1f;
    [Range(0f, 3f)] public float monsterAggression = 1f;
    [Range(0f, 3f)] public float environmentReaction = 1f;
}
