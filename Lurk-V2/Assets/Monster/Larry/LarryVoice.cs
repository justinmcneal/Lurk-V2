using UnityEngine;

public class LarryVoice : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioSource breathSource; // Separate source for breathing loop

    [Header("Clips - Phase 2 (Investigate)")]
    [SerializeField] private AudioClip[] awareTaunts;     // Rare eerie sounds
    [SerializeField] private AudioClip[] investigateSounds; // Grunts, moans (slower, creepy)

    [Header("Clips - Phase 3 (Hunt/Chase)")]
    [SerializeField] private AudioClip[] chaseTaunts;     // Aggressive taunts
    [SerializeField] private AudioClip[] chargeScreams;   // When charge starts
    [SerializeField] private AudioClip[] closeGrowls;     // When very close

    [Header("Clips - Ambient")]
    [SerializeField] private AudioClip closeBreathLoop;   // Breathing when nearby

    [Header("Volumes")]
    [Range(0f, 1f)] [SerializeField] private float tauntVolume = 0.7f;
    [Range(0f, 1f)] [SerializeField] private float screamVolume = 0.9f;
    [Range(0f, 1f)] [SerializeField] private float growlVolume = 0.8f;
    [Range(0f, 1f)] [SerializeField] private float investigateVolume = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float breathVolume = 0.4f;

    [Header("Proximity Settings")]
    [SerializeField] private float breathStartDistance = 8f;
    [SerializeField] private float breathMaxDistance = 15f;
    [SerializeField] private Transform player;

    [Header("Pitch Variation")]
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;

    private bool isBreathingActive = false;

    private void Awake()
    {
        if (voiceSource == null)
            voiceSource = GetComponent<AudioSource>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        SetupBreathSource();
    }

    private void SetupBreathSource()
    {
        if (breathSource == null)
        {
            breathSource = gameObject.AddComponent<AudioSource>();
            breathSource.spatialBlend = 1f; // 3D sound
            breathSource.rolloffMode = AudioRolloffMode.Linear;
            breathSource.minDistance = 2f;
            breathSource.maxDistance = breathMaxDistance;
            breathSource.loop = true;
            breathSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        UpdateProximityBreathing();
    }

    private void UpdateProximityBreathing()
    {
        if (player == null || closeBreathLoop == null || breathSource == null)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= breathStartDistance)
        {
            if (!isBreathingActive)
            {
                StartBreathing();
            }

            // Fade in volume based on distance (closer = louder)
            float t = 1f - Mathf.Clamp01((dist - 2f) / (breathStartDistance - 2f));
            breathSource.volume = Mathf.Lerp(0.1f, breathVolume, t);
        }
        else if (isBreathingActive)
        {
            StopBreathing();
        }
    }

    private void StartBreathing()
    {
        if (breathSource == null || closeBreathLoop == null) return;

        breathSource.clip = closeBreathLoop;
        breathSource.Play();
        isBreathingActive = true;
    }

    private void StopBreathing()
    {
        if (breathSource == null) return;

        breathSource.Stop();
        isBreathingActive = false;
    }

    // Phase 2: Rare eerie aware sound
    public void PlayHunterAwareOnce()
    {
        PlayRandom(awareTaunts, tauntVolume, randomPitch: true);
    }

    // Phase 2: Occasional investigation sounds (grunts, moans)
    public void PlayInvestigateSound()
    {
        PlayRandom(investigateSounds, investigateVolume, randomPitch: true);
    }

    // Phase 3: Aggressive taunts
    public void PlayChaseTaunt()
    {
        PlayRandom(chaseTaunts, tauntVolume, randomPitch: false);
    }

    // Phase 3: Charge scream
    public void PlayChargeScream()
    {
        PlayRandom(chargeScreams, screamVolume, randomPitch: false);
    }

    // When very close
    public void PlayCloseGrowl()
    {
        PlayRandom(closeGrowls, growlVolume, randomPitch: true);
    }

    private void PlayRandom(AudioClip[] clips, float volume, bool randomPitch = false)
    {
        if (voiceSource == null || clips == null || clips.Length == 0) return;

        var clip = clips[Random.Range(0, clips.Length)];
        if (clip == null) return;

        if (randomPitch)
        {
            voiceSource.pitch = Random.Range(minPitch, maxPitch);
        }
        else
        {
            voiceSource.pitch = 1f;
        }

        voiceSource.PlayOneShot(clip, volume);
    }

    public void StopAllSounds()
    {
        voiceSource?.Stop();
        StopBreathing();
    }
}
