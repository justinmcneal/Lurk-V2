using UnityEngine;

public class LarryKillTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var hp = other.GetComponentInParent<PlayerHealth>();
        if (hp != null)
            hp.Kill("Larry");
    }
}
