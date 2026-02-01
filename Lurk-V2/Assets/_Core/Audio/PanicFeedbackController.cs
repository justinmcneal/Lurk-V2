using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PanicFeedbackController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EscalationManager escalation;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform larry;
    [SerializeField] private AudioSource breathing;
    [SerializeField] private AudioSource heartbeat;
    [SerializeField] private Volume globalVolume;

    [Header("Distance Thresholds")]
    [Tooltip("Distance where panic starts (Chaos phase)")]
    [SerializeField] private float panicStartDistance = 12f;
    [Tooltip("Distance where panic is maximum")]
    [SerializeField] private float panicMaxDistance = 4f;

    [Header("Visual Effects")]
    [SerializeField] private float maxVignetteIntensity = 0.5f;
    [SerializeField] private float maxFOVReduction = 15f;
    [SerializeField] private float panicFadeSpeed = 2.5f;
    
    [Header("Audio Settings")]
    [SerializeField] private float breathingMinVolume = 0.2f;
    [SerializeField] private float breathingMaxVolume = 0.7f;
    [SerializeField] private float heartbeatMinVolume = 0.3f;
    [SerializeField] private float heartbeatMaxVolume = 0.9f;
    [SerializeField] private float heartbeatMinPitch = 0.9f;
    [SerializeField] private float heartbeatMaxPitch = 1.4f;

    [Header("Phase Multipliers")]
    [Tooltip("Panic intensity in Scout phase (should be 0 or very low)")]
    [SerializeField] private float scoutPanicMultiplier = 0f;
    [Tooltip("Panic intensity in Escalate phase (mild)")]
    [SerializeField] private float escalatePanicMultiplier = 0.3f;
    [Tooltip("Panic intensity in Chaos phase (full)")]
    [SerializeField] private float chaosPanicMultiplier = 1f;

    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private Camera cam;
    private float baseFOV;
    private float currentPanicLevel; // 0 → 1

    private void Awake()
    {
        if (escalation == null)
            escalation = FindFirstObjectByType<EscalationManager>();

        if (playerCamera == null)
            playerCamera = Camera.main?.transform;

        if (larry == null)
            larry = GameObject.FindGameObjectWithTag("Larry")?.transform;

        cam = playerCamera.GetComponent<Camera>();
        baseFOV = cam.fieldOfView;

        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out chromaticAberration);
        }
    }

    private void Update()
    {
        float targetPanic = CalculatePanicLevel();
        
        // Smooth transition
        currentPanicLevel = Mathf.MoveTowards(currentPanicLevel, targetPanic, panicFadeSpeed * Time.deltaTime);

        ApplyPanicEffects(currentPanicLevel);
    }

    private float CalculatePanicLevel()
    {
        if (escalation == null || escalation.CurrentPhase == null || larry == null || playerCamera == null)
            return 0f;

        // Get phase multiplier
        float phaseMultiplier = GetPhaseMultiplier();

        // No panic if phase multiplier is 0 (Scout)
        if (phaseMultiplier <= 0f)
            return 0f;

        // Calculate distance-based panic
        float dist = Vector3.Distance(playerCamera.position, larry.position);

        float distancePanic = 0f;
        if (dist <= panicStartDistance)
        {
            // Inverse lerp: closer = more panic
            distancePanic = 1f - Mathf.InverseLerp(panicMaxDistance, panicStartDistance, dist);
            distancePanic = Mathf.Clamp01(distancePanic);
        }

        // Combine phase and distance
        return distancePanic * phaseMultiplier;
    }

    private float GetPhaseMultiplier()
    {
        if (escalation.CurrentPhase.chaseActive)
        {
            return chaosPanicMultiplier; // Chaos - full panic
        }
        else if (escalation.CurrentPhase.hunterAware)
        {
            return escalatePanicMultiplier; // Escalate - mild panic
        }
        else
        {
            return scoutPanicMultiplier; // Scout - no panic
        }
    }

    private void ApplyPanicEffects(float panicLevel)
    {
        // Visual: Vignette
        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(0f, maxVignetteIntensity, panicLevel);
        }

        // Visual: Chromatic Aberration (subtle screen distortion)
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = Mathf.Lerp(0f, 0.3f, panicLevel);
        }

        // Visual: FOV narrowing (tunnel vision)
        if (cam != null)
        {
            cam.fieldOfView = Mathf.Lerp(baseFOV, baseFOV - maxFOVReduction, panicLevel);
        }

        // Audio: Breathing
        if (breathing != null)
        {
            if (panicLevel > 0.05f)
            {
                if (!breathing.isPlaying) breathing.Play();
                breathing.volume = Mathf.Lerp(breathingMinVolume, breathingMaxVolume, panicLevel);
            }
            else
            {
                if (breathing.isPlaying) breathing.Stop();
            }
        }

        // Audio: Heartbeat (ramps up pitch and volume)
        if (heartbeat != null)
        {
            if (panicLevel > 0.1f)
            {
                if (!heartbeat.isPlaying) heartbeat.Play();
                heartbeat.volume = Mathf.Lerp(heartbeatMinVolume, heartbeatMaxVolume, panicLevel);
                heartbeat.pitch = Mathf.Lerp(heartbeatMinPitch, heartbeatMaxPitch, panicLevel);
            }
            else
            {
                if (heartbeat.isPlaying) heartbeat.Stop();
            }
        }
    }

    private void OnDisable()
    {
        // Reset effects when disabled
        if (cam != null)
            cam.fieldOfView = baseFOV;

        if (breathing != null && breathing.isPlaying)
            breathing.Stop();

        if (heartbeat != null && heartbeat.isPlaying)
            heartbeat.Stop();
    }
}
