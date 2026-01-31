using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public bool IsDead { get; private set; }

    public void Kill(string reason = "Killed")
    {
        if (IsDead) return;
        IsDead = true;
        Debug.Log($"[RUN] Player died: {reason}");
        FindFirstObjectByType<RunStateManager>()?.OnPlayerDied();
    }
}
