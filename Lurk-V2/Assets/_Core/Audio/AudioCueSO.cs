using UnityEngine;

[CreateAssetMenu(
    fileName = "AudioCue",
    menuName = "ScriptableObjects/Audio/Audio Cue"
)]
public class AudioCueSO : ScriptableObject
{
    [Header("Identity")]
    public string cueName;

    [TextArea]
    public string description;

    [Header("Trigger")]
    [Tooltip("Cue triggers when oxygen percent is AT or BELOW this value.")]
    [Range(0f, 1f)]
    public float triggerAtOxygenPercent = 0.75f;

    [Tooltip("Should this cue fire only once per run?")]
    public bool fireOnce = true;

    [Header("Audio")]
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.7f;

    [Tooltip("If true, plays near the player. If false, plays from a random nearby offset.")]
    public bool playNearPlayer = true;

    [Tooltip("Max distance offset if NOT playing near player.")]
    public float randomOffsetRadius = 6f;
}
