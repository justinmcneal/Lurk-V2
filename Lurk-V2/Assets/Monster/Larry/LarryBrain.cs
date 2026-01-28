using UnityEngine;

public class LarryBrain : MonoBehaviour
{
    private enum LarryState
    {
        Disabled,       // Scout
        Investigate,    // Escalate (listening)
        Hunt            // Chaos (aggressive)
    }

    [Header("References")]
    [SerializeField] private EscalationManager escalation;
    [SerializeField] private LarryMotor motor;

    [Header("Hearing")]
    [Tooltip("Minimum intensity required for Larry to care.")]
    [SerializeField] private float minIntensityToReact = 0.2f;

    [Tooltip("If a noise is within this radius, Larry accepts it. (NoiseEvent.radius will be compared against distance.)")]
    [SerializeField] private bool useNoiseRadius = true;

    [Header("Behavior")]
    [Tooltip("How close Larry must get to the target point to consider it 'reached'.")]
    [SerializeField] private float arriveDistance = 1.2f;

    [Tooltip("How long Larry keeps investigating after the last noise (seconds).")]
    [SerializeField] private float investigateMemorySeconds = 6f;

    [Tooltip("How long Larry keeps hunting after last noise (seconds).")]
    [SerializeField] private float huntMemorySeconds = 12f;

    private LarryState state = LarryState.Disabled;

    private Vector3 lastHeardPos;
    private float lastHeardTime;
    private bool hasHeardAnything;

    private void Awake()
    {
        if (escalation == null)
            escalation = FindFirstObjectByType<EscalationManager>();

        if (motor == null)
            motor = GetComponent<LarryMotor>();
    }

    private void OnEnable()
    {
        NoiseBus.OnNoiseEmitted += OnNoiseHeard;

        if (escalation != null)
        {
            escalation.OnPhaseChanged += OnPhaseChanged;
            escalation.OnHunterAware += OnHunterAware;
            escalation.OnChaseStart += OnChaseStart;
        }
    }

    private void OnDisable()
    {
        NoiseBus.OnNoiseEmitted -= OnNoiseHeard;

        if (escalation != null)
        {
            escalation.OnPhaseChanged -= OnPhaseChanged;
            escalation.OnHunterAware -= OnHunterAware;
            escalation.OnChaseStart -= OnChaseStart;
        }
    }

    private void Start()
    {
        // Initialize from current phase
        if (escalation != null && escalation.CurrentPhase != null)
            ApplyPhase(escalation.CurrentPhase);
        else
            SetState(LarryState.Disabled);
    }

    private void Update()
    {
        if (state == LarryState.Disabled || motor == null)
            return;

        // If no noise yet, do nothing
        if (!hasHeardAnything)
            return;

        float memory = state == LarryState.Hunt ? huntMemorySeconds : investigateMemorySeconds;

        // Forget after memory window
        if (Time.time - lastHeardTime > memory)
        {
            hasHeardAnything = false;
            Debug.Log("[LARRY] Lost the trail (silence).");
            return;
        }

        // Move toward last heard position
        motor.MoveTowards(lastHeardPos, hunting: state == LarryState.Hunt);

        // Arrive check
        float dist = Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z),
                                      new Vector3(lastHeardPos.x, 0f, lastHeardPos.z));

        if (dist <= arriveDistance)
        {
            // Larry reached the sound source and "listens"
            // In future: idle animation, scanning, etc.
            // For now: just hold position.
            // (We keep hasHeardAnything true until memory expires, so he doesn't instantly stop.)
        }
    }

    private void OnNoiseHeard(NoiseBus.NoiseEvent e)
    {
        if (state == LarryState.Disabled)
            return;

        if (e.intensity < minIntensityToReact)
            return;

        // Optional: respect the event radius as hearing range
        if (useNoiseRadius)
        {
            float dist = Vector3.Distance(transform.position, e.position);
            if (dist > e.radius)
                return;
        }

        lastHeardPos = e.position;
        lastHeardTime = Time.time;
        hasHeardAnything = true;

        Debug.Log($"[LARRY] Heard noise: {e.source} (intensity={e.intensity:0.00}) -> moving to sound");
    }

    private void OnPhaseChanged(EscalationPhaseSO phase)
    {
        if (phase == null) return;
        ApplyPhase(phase);
    }

    private void OnHunterAware()
    {
        // Escalate moment
        // You can add a one-time audio bark later here.
        Debug.Log("[LARRY] Hunter aware signal received.");
    }

    private void OnChaseStart()
    {
        Debug.Log("[LARRY] Chase start signal received.");
    }

    private void ApplyPhase(EscalationPhaseSO phase)
    {
        if (phase.chaseActive)
        {
            SetState(LarryState.Hunt);
        }
        else if (phase.hunterAware)
        {
            SetState(LarryState.Investigate);
        }
        else
        {
            SetState(LarryState.Disabled);
        }
    }

    private void SetState(LarryState newState)
    {
        if (state == newState) return;

        state = newState;

        if (state == LarryState.Disabled)
        {
            hasHeardAnything = false;
        }

        Debug.Log($"[LARRY] State -> {state}");
    }
}
