using UnityEngine;

[CreateAssetMenu(fileName = "OxygenConfig", menuName = "ScriptableObjects/OxygenConfig")]
public class OxygenConfig : ScriptableObject
{
    [Header("Start / Limits")]
    public float startOxygenSeconds = 300f;   // 5 minutes
    public float maxOxygenSeconds = 600f;     // 10 minutes cap

    [Header("Drain")]
    public float drainPerSecond = 1f;         // 1 oxygen per second

    [Header("Quota")]
    public int quotaTanksRequired = 5;

    [Header("Tank Value")]
    public float tankAddSeconds = 30f;        // each tank adds 30 seconds
}
