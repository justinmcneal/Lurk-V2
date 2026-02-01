using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public float MaxHealth => maxHealth;
    public float HealthPercent => maxHealth > 0 ? CurrentHealth / maxHealth : 0f;
    public bool IsDead { get; private set; }

    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount, string source = "Unknown")
    {
        if (IsDead) return;

        CurrentHealth -= amount;
        Debug.Log($"[HEALTH] Took {amount} damage from {source}. Health: {CurrentHealth:0}/{maxHealth}");

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            Kill(source);
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        Debug.Log($"[HEALTH] Healed {amount}. Health: {CurrentHealth:0}/{maxHealth}");
    }

    public void Kill(string reason = "Killed")
    {
        if (IsDead) return;
        IsDead = true;
        Debug.Log($"[RUN] Player died: {reason}");
        FindFirstObjectByType<RunStateManager>()?.OnPlayerDied();
    }
}
