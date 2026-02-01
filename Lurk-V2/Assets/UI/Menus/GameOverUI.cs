using UnityEngine;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Messages")]
    [SerializeField] private string deathMessage = "YOU DIED";
    [SerializeField] private string winMessage = "RUN COMPLETE";

    [Header("Colors")]
    [SerializeField] private Color deathColor = new Color(0.8f, 0.1f, 0.1f, 1f); // Dark red
    [SerializeField] private Color winColor = new Color(0.1f, 0.8f, 0.3f, 1f);   // Green

    private RunStateManager runState;

    private void Awake()
    {
        // Start hidden
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    private void Start()
    {
        runState = FindFirstObjectByType<RunStateManager>();
    }

    private void Update()
    {
        if (runState == null) return;

        // Show message when game ends
        if (runState.State == RunStateManager.RunState.Dead)
        {
            ShowMessage(deathMessage, deathColor);
        }
        else if (runState.State == RunStateManager.RunState.Won)
        {
            ShowMessage(winMessage, winColor);
        }
    }

    private void ShowMessage(string message, Color color)
    {
        if (messageText == null) return;

        if (!messageText.gameObject.activeSelf)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = message;
            messageText.color = color;
        }
    }
}
