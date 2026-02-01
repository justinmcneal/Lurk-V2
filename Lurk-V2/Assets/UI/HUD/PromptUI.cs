using UnityEngine;
using TMPro;

public class PromptUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float fadeSpeed = 5f;

    [Header("Messages")]
    [SerializeField] private string submitPrompt = "Press E to Submit";
    [SerializeField] private string quotaNotMetMessage = "Quota not met!";

    private OxygenManager oxygen;
    private bool isNearTerminal;
    private float targetAlpha;

    private void Start()
    {
        oxygen = FindFirstObjectByType<OxygenManager>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        targetAlpha = 0f;
    }

    private void Update()
    {
        UpdatePromptText();
        UpdateFade();
    }

    private void UpdatePromptText()
    {
        if (promptText == null) return;

        if (isNearTerminal)
        {
            if (oxygen != null && oxygen.QuotaMet)
            {
                promptText.text = submitPrompt;
                promptText.color = Color.green;
            }
            else
            {
                promptText.text = quotaNotMetMessage;
                promptText.color = Color.red;
            }
            targetAlpha = 1f;
        }
        else
        {
            targetAlpha = 0f;
        }
    }

    private void UpdateFade()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
    }

    // Called by TerminalInteract or a trigger zone
    public void ShowPrompt(bool show)
    {
        isNearTerminal = show;
    }

    // Static helper for easy access
    public static void SetNearTerminal(bool near)
    {
        var instance = FindFirstObjectByType<PromptUI>();
        if (instance != null)
            instance.ShowPrompt(near);
    }
}
