using UnityEngine;

public class LarryBrain : MonoBehaviour
{
    private enum LarryState
    {
        Disabled,       // Scout
        Investigate,    // Escalate (listening)
        Hunt,           // Chaos (aggressive)
        Charge          // Phase 3 commit burst
    }

    [Header("References")]
    [SerializeField] private EscalationManager escalation;
    [SerializeField] private LarryMotor motor;
    [SerializeField] private LarryVoice voice;
    [SerializeField] private Transform player;

    [Header("Hearing")]
    [Tooltip("Minimum intensity required for Larry to care.")]
    [SerializeField] private float minIntensityToReact = 0.2f;

    [Tooltip("If true, Larry only reacts if distance <= NoiseEvent.radius.")]
    [SerializeField] private bool useNoiseRadius = true;

    [Header("Behavior")]
    [Tooltip("How close Larry must get to the target point to consider it 'reached'.")]
    [SerializeField] private float arriveDistance = 1.2f;

    [Tooltip("How long Larry keeps investigating after the last noise (seconds).")]
    [SerializeField] private float investigateMemorySeconds = 6f;

    [Tooltip("How long Larry keeps hunting after last noise (seconds).")]
    [SerializeField] private float huntMemorySeconds = 12f;

    [Header("Charge (Phase 3)")]
    [SerializeField] private float chargeTriggerDistance = 10f;
    [SerializeField] private float chargeDuration = 2.0f;
    [SerializeField] private float chargeCooldown = 3.5f;
    [SerializeField] private float chargeSpeedMultiplier = 1.8f;

    [Header("Vocalization")]
    [SerializeField] private float tauntIntervalMin = 6f;
    [SerializeField] private float tauntIntervalMax = 11f;
    [SerializeField] private float closeGrowlDistance = 3.2f;
    [SerializeField] private float closeGrowlCooldown = 2.5f;

    [Header("Phase 2 - Investigate Behavior")]
    [SerializeField] private float investigateSoundIntervalMin = 8f;
    [SerializeField] private float investigateSoundIntervalMax = 15f;
    [SerializeField] private float investigatePauseChance = 0.3f; // 30% chance to pause
    [SerializeField] private float investigatePauseDuration = 2f;

    [Header("Phase 3 - Hunt Behavior")]
    [SerializeField] private float huntTauntIntervalMin = 4f;
    [SerializeField] private float huntTauntIntervalMax = 8f;
    [SerializeField] private float aggressiveGrowlDistance = 5f;

    private LarryState state = LarryState.Disabled;

    private Vector3 lastHeardPos;
    private float lastHeardTime;
    private bool hasHeardAnything;

    private float chargeEndTime;
    private float nextChargeAllowedTime;

    private float nextTauntTime;
    private float nextCloseGrowlTime;
    private float nextInvestigateSoundTime;
    private float investigatePauseEndTime;
    private bool isPaused;

    private void Awake()
    {
        if (escalation == null)
            escalation = FindFirstObjectByType<EscalationManager>();

        if (motor == null)
            motor = GetComponent<LarryMotor>();

        if (voice == null)
            voice = GetComponent<LarryVoice>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
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

        // CHARGE: commit to locked target for duration
        if (state == LarryState.Charge)
        {
            motor.MoveTowards(lastHeardPos, hunting: true, speedMultiplier: chargeSpeedMultiplier);

            if (Time.time >= chargeEndTime)
                SetState(LarryState.Hunt);

            return;
        }

        // If no noise yet, do nothing (Investigate/Hunt will just idle)
        if (!hasHeardAnything && state != LarryState.Hunt)
            return;

        // Forget after memory window (Investigate/Hunt)
        float memory = state == LarryState.Hunt ? huntMemorySeconds : investigateMemorySeconds;

        if (hasHeardAnything && Time.time - lastHeardTime > memory)
        {
            hasHeardAnything = false;
            Debug.Log("[LARRY] Lost the trail (silence).");
            return;
        }

        // ============ PHASE 2 - INVESTIGATE BEHAVIOR ============
        if (state == LarryState.Investigate)
        {
            UpdateInvestigateBehavior();
            return;
        }

        // ============ PHASE 3 - HUNT BEHAVIOR ============
        if (state == LarryState.Hunt && player != null)
        {
            UpdateHuntBehavior();
        }
    }

    private void UpdateInvestigateBehavior()
    {
        // Check for random pause (creepy hesitation)
        if (isPaused)
        {
            if (Time.time >= investigatePauseEndTime)
            {
                isPaused = false;
            }
            else
            {
                // Just stand there menacingly
                return;
            }
        }

        // Random chance to pause mid-movement (creepy)
        if (hasHeardAnything && Random.value < investigatePauseChance * Time.deltaTime)
        {
            isPaused = true;
            investigatePauseEndTime = Time.time + investigatePauseDuration;
            Debug.Log("[LARRY] *pauses and listens*");
            return;
        }

        // Occasional eerie sounds during investigation
        if (Time.time >= nextInvestigateSoundTime)
        {
            ScheduleNextInvestigateSound();
            voice?.PlayInvestigateSound();
        }

        // Move toward last heard position (slower, methodical)
        if (hasHeardAnything)
        {
            motor.MoveTowards(lastHeardPos, hunting: false);

            float dist = Vector3.Distance(
                new Vector3(transform.position.x, 0f, transform.position.z),
                new Vector3(lastHeardPos.x, 0f, lastHeardPos.z)
            );

            if (dist <= arriveDistance)
            {
                // Reached sound point -> wait and listen
            }
        }
    }

    private void UpdateHuntBehavior()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // Close growl - more frequent in Phase 3
        if (distToPlayer <= closeGrowlDistance && Time.time >= nextCloseGrowlTime)
        {
            nextCloseGrowlTime = Time.time + closeGrowlCooldown;
            voice?.PlayCloseGrowl();
        }
        // Aggressive growl at medium range
        else if (distToPlayer <= aggressiveGrowlDistance && distToPlayer > closeGrowlDistance)
        {
            if (Time.time >= nextCloseGrowlTime)
            {
                nextCloseGrowlTime = Time.time + closeGrowlCooldown * 1.5f; // Slightly longer cooldown
                voice?.PlayCloseGrowl();
            }
        }

        // More frequent taunts during hunt
        if (Time.time >= nextTauntTime)
        {
            ScheduleNextTaunt();
            voice?.PlayChaseTaunt();
        }

        // Charge trigger
        if (Time.time >= nextChargeAllowedTime && distToPlayer <= chargeTriggerDistance)
        {
            StartCharge(player.position);
            return;
        }

        // Move toward last heard position
        if (hasHeardAnything)
        {
            motor.MoveTowards(lastHeardPos, hunting: true);

            float dist = Vector3.Distance(
                new Vector3(transform.position.x, 0f, transform.position.z),
                new Vector3(lastHeardPos.x, 0f, lastHeardPos.z)
            );

            if (dist <= arriveDistance)
            {
                // Reached sound point -> hold
            }
        }
    }

    private void OnNoiseHeard(NoiseBus.NoiseEvent e)
    {
        if (state == LarryState.Disabled)
            return;

        if (e.intensity < minIntensityToReact)
            return;

        if (useNoiseRadius)
        {
            float dist = Vector3.Distance(transform.position, e.position);
            if (dist > e.radius)
                return;
        }

        // In Hunt/Charge, we still use noise as "last known position"
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
        Debug.Log("[LARRY] Hunter aware signal received.");
        voice?.PlayHunterAwareOnce();
    }

    private void OnChaseStart()
    {
        Debug.Log("[LARRY] Chase start signal received.");
        voice?.PlayChaseTaunt();
        ScheduleNextTaunt();
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
            nextTauntTime = 0f;
            nextCloseGrowlTime = 0f;
            nextInvestigateSoundTime = 0f;
            isPaused = false;
            voice?.StopAllSounds();
        }

        if (state == LarryState.Investigate)
        {
            ScheduleNextInvestigateSound();
            isPaused = false;
        }

        if (state == LarryState.Hunt)
        {
            ScheduleNextTaunt();
            isPaused = false;
        }

        Debug.Log($"[LARRY] State -> {state}");
    }

    private void StartCharge(Vector3 targetPos)
    {
        // Lock-on for charge duration
        lastHeardPos = targetPos;
        lastHeardTime = Time.time;
        hasHeardAnything = true;

        SetState(LarryState.Charge);

        chargeEndTime = Time.time + chargeDuration;
        nextChargeAllowedTime = Time.time + chargeCooldown;

        voice?.PlayChargeScream();

        Debug.Log("[LARRY] CHARGE!");
    }

    private void ScheduleNextTaunt()
    {
        // Use shorter intervals in Hunt mode
        if (state == LarryState.Hunt)
        {
            nextTauntTime = Time.time + Random.Range(huntTauntIntervalMin, huntTauntIntervalMax);
        }
        else
        {
            nextTauntTime = Time.time + Random.Range(tauntIntervalMin, tauntIntervalMax);
        }
    }

    private void ScheduleNextInvestigateSound()
    {
        nextInvestigateSoundTime = Time.time + Random.Range(investigateSoundIntervalMin, investigateSoundIntervalMax);
    }
}
