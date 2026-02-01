using UnityEngine;
using TMPro;

public class PhaseUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI phaseText;

    [Header("Phase Colors")]
    [SerializeField] private Color scoutColor = Color.green;
    [SerializeField] private Color escalateColor = Color.yellow;
    [SerializeField] private Color chaosColor = Color.red;

    private EscalationManager escalation;

    private void Start()
    {
        escalation = FindFirstObjectByType<EscalationManager>();

        if (escalation == null)
        {
            Debug.LogWarning("[PhaseUI] EscalationManager not found!");
            enabled = false;
            return;
        }

        // Subscribe to phase changes
        escalation.OnPhaseChanged += HandlePhaseChanged;

        // Initialize with current phase
        if (escalation.CurrentPhase != null)
            HandlePhaseChanged(escalation.CurrentPhase);
    }

    private void OnDestroy()
    {
        if (escalation != null)
            escalation.OnPhaseChanged -= HandlePhaseChanged;
    }

    private void HandlePhaseChanged(EscalationPhaseSO phase)
    {
        if (phaseText == null || phase == null) return;

        phaseText.text = phase.phaseName;

        // Color based on phase name
        string phaseLower = phase.phaseName.ToLower();
        if (phaseLower.Contains("scout"))
            phaseText.color = scoutColor;
        else if (phaseLower.Contains("escalat"))
            phaseText.color = escalateColor;
        else if (phaseLower.Contains("chaos"))
            phaseText.color = chaosColor;
        else
            phaseText.color = Color.white;
    }
}
