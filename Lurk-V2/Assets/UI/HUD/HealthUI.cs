using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image healthFill;

    [Header("Settings")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 0.3f;

    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogWarning("[HealthUI] PlayerHealth not found!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (playerHealth == null || healthFill == null) return;

        float healthPercent = playerHealth.HealthPercent;

        // Update fill amount
        healthFill.fillAmount = healthPercent;

        // Change color based on health
        healthFill.color = healthPercent <= lowHealthThreshold ? lowHealthColor : normalColor;
    }
}
