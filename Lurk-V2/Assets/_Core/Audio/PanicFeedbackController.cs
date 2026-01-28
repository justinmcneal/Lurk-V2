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

    [Header("Panic Settings")]
    [SerializeField] private float panicDistance = 8f;
    [SerializeField] private float fovPanicReduction = 12f;
    [SerializeField] private float panicFadeSpeed = 3f;

    private Vignette vignette;
    private Camera cam;
    private float baseFOV;
    private float panicLevel; // 0 → 1

    private void Awake()
    {
        cam = playerCamera.GetComponent<Camera>();
        baseFOV = cam.fieldOfView;

        globalVolume.profile.TryGet(out vignette);
    }

    private void Update()
    {
        bool chaos = escalation.CurrentPhase != null && escalation.CurrentPhase.chaseActive;
        float dist = Vector3.Distance(playerCamera.position, larry.position);

        bool shouldPanic = chaos && dist <= panicDistance;

        float target = shouldPanic ? 1f : 0f;
        panicLevel = Mathf.MoveTowards(panicLevel, target, panicFadeSpeed * Time.deltaTime);

        ApplyPanic(panicLevel);
    }

    private void ApplyPanic(float t)
    {
        // Vignette
        vignette.intensity.value = Mathf.Lerp(0f, 0.45f, t);

        // FOV
        cam.fieldOfView = Mathf.Lerp(baseFOV, baseFOV - fovPanicReduction, t);

        // Audio
        if (t > 0.1f)
        {
            if (!breathing.isPlaying) breathing.Play();
            if (!heartbeat.isPlaying) heartbeat.Play();

            breathing.volume = t;
            heartbeat.volume = t;
        }
        else
        {
            breathing.Stop();
            heartbeat.Stop();
        }
    }
}
