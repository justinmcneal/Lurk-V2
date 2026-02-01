using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image staminaFill;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float fadeDelay = 2f;      // hide bar after full for this long
    [SerializeField] private float fadeSpeed = 3f;
    [SerializeField] private Color normalColor = Color.yellow;
    [SerializeField] private Color exhaustedColor = Color.red;

    private StaminaManager stamina;
    private float fullTimer;
    private bool wasExhausted;

    private void Start()
    {
        stamina = FindFirstObjectByType<StaminaManager>();

        if (stamina == null)
        {
            Debug.LogWarning("[StaminaUI] StaminaManager not found!");
            enabled = false;
            return;
        }

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (stamina == null) return;

        float percent = stamina.StaminaPercent;

        // Update fill amount
        if (staminaFill != null)
        {
            staminaFill.fillAmount = percent;
            staminaFill.color = stamina.IsExhausted ? exhaustedColor : normalColor;
        }

        // Flash on exhaustion
        if (stamina.IsExhausted && !wasExhausted)
        {
            // Could trigger a flash effect here
        }
        wasExhausted = stamina.IsExhausted;

        // Fade logic: hide when full, show when not
        if (canvasGroup != null)
        {
            if (percent >= 1f)
            {
                fullTimer += Time.deltaTime;
                if (fullTimer >= fadeDelay)
                {
                    canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, fadeSpeed * Time.deltaTime);
                }
            }
            else
            {
                fullTimer = 0f;
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);
            }
        }
    }
}
