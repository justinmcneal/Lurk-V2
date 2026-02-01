using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuotaUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI quotaText;
    [SerializeField] private Image tankIcon; // Optional: tank outline sprite
    [SerializeField] private Image tankFill; // Optional: tank fill sprite that changes with progress

    [Header("Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color completedColor = Color.green;
    [SerializeField] private Color emptyTankColor = new Color(1f, 1f, 1f, 0.3f); // Dimmed for uncollected

    private OxygenManager oxygen;

    private void Start()
    {
        oxygen = FindFirstObjectByType<OxygenManager>();

        if (oxygen == null)
        {
            Debug.LogWarning("[QuotaUI] OxygenManager not found!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (oxygen == null) return;

        int collected = oxygen.TanksCollected;
        int required = oxygen.QuotaTanksRequired;
        bool quotaMet = oxygen.QuotaMet;

        // Update text
        if (quotaText != null)
        {
            quotaText.text = $"Tanks: {collected}/{required}";
            quotaText.color = quotaMet ? completedColor : normalColor;
        }

        // Update tank outline - full opacity when quota met, partial otherwise
        if (tankIcon != null)
        {
            tankIcon.color = quotaMet ? completedColor : normalColor;
        }

        // Update tank fill - fill amount based on collection progress
        if (tankFill != null)
        {
            float fillPercent = required > 0 ? (float)collected / required : 0f;
            tankFill.fillAmount = fillPercent;
            tankFill.color = quotaMet ? completedColor : normalColor;
        }
    }
}
