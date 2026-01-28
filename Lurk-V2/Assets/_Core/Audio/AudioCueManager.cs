using System.Collections.Generic;
using UnityEngine;

public class AudioCueManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OxygenManager oxygen;
    [SerializeField] private Transform player;

    [Header("Cues")]
    [SerializeField] private List<AudioCueSO> cues = new();

    private readonly HashSet<AudioCueSO> firedCues = new();

    private AudioSource oneShotSource;

    private void Awake()
    {
        if (oxygen == null)
            oxygen = FindFirstObjectByType<OxygenManager>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        oneShotSource = gameObject.AddComponent<AudioSource>();
        oneShotSource.spatialBlend = 1f; // 3D sound
        oneShotSource.rolloffMode = AudioRolloffMode.Linear;
        oneShotSource.minDistance = 1f;
        oneShotSource.maxDistance = 20f;
    }

    private void Update()
    {
        if (oxygen == null || player == null) return;

        float oxygenPercent = oxygen.OxygenPercentRemaining;

        foreach (var cue in cues)
        {
            if (cue == null) continue;

            if (cue.fireOnce && firedCues.Contains(cue))
                continue;

            if (oxygenPercent <= cue.triggerAtOxygenPercent)
            {
                PlayCue(cue);
                firedCues.Add(cue);
            }
        }
    }

    private void PlayCue(AudioCueSO cue)
    {
        Vector3 pos = player.position;

        if (!cue.playNearPlayer)
        {
            Vector2 offset2D = Random.insideUnitCircle * cue.randomOffsetRadius;
            pos += new Vector3(offset2D.x, 0f, offset2D.y);
        }

        oneShotSource.transform.position = pos;
        oneShotSource.PlayOneShot(cue.clip, cue.volume);

        Debug.Log($"[AUDIO CUE] {cue.cueName} at {pos}");
    }
}
