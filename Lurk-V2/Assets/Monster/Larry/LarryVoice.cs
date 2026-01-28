using UnityEngine;

public class LarryVoice : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource voiceSource;

    [Header("Clips")]
    [SerializeField] private AudioClip[] awareTaunts;     // Phase 2: optional
    [SerializeField] private AudioClip[] chaseTaunts;     // Phase 3: taunts during hunt
    [SerializeField] private AudioClip[] chargeScreams;   // Phase 3: when charge starts
    [SerializeField] private AudioClip[] closeGrowls;     // Phase 3: when very close

    [Header("Volumes")]
    [Range(0f, 1f)] [SerializeField] private float tauntVolume = 0.7f;
    [Range(0f, 1f)] [SerializeField] private float screamVolume = 0.9f;
    [Range(0f, 1f)] [SerializeField] private float growlVolume = 0.7f;

    private void Awake()
    {
        if (voiceSource == null)
            voiceSource = GetComponent<AudioSource>();
    }

    public void PlayHunterAwareOnce()
    {
        PlayRandom(awareTaunts, tauntVolume);
    }

    public void PlayChaseTaunt()
    {
        PlayRandom(chaseTaunts, tauntVolume);
    }

    public void PlayChargeScream()
    {
        PlayRandom(chargeScreams, screamVolume);
    }

    public void PlayCloseGrowl()
    {
        PlayRandom(closeGrowls, growlVolume);
    }

    private void PlayRandom(AudioClip[] clips, float volume)
    {
        if (voiceSource == null || clips == null || clips.Length == 0) return;

        var clip = clips[Random.Range(0, clips.Length)];
        if (clip == null) return;

        voiceSource.PlayOneShot(clip, volume);
    }
}
