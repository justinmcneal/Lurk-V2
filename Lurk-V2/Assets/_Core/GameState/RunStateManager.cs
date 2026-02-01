using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunStateManager : MonoBehaviour
{
    public enum RunState { Playing, Won, Dead }

    [SerializeField] private CanvasGroup fadeCanvas; // optional
    [SerializeField] private float fadeTime = 0.6f;
    [SerializeField] private float messageDisplayTime = 2.0f; // Time to show "You Died" / "Run Complete"

    public RunState State { get; private set; } = RunState.Playing;

    public void OnPlayerWon()
    {
        if (State != RunState.Playing) return;
        State = RunState.Won;
        Debug.Log("[RUN] WIN");
        StartCoroutine(EndRoutine());
    }

    public void OnPlayerDied()
    {
        if (State != RunState.Playing) return;
        State = RunState.Dead;
        Debug.Log("[RUN] DEAD");
        StartCoroutine(EndRoutine());
    }

    private IEnumerator EndRoutine()
    {
        // Fade to black
        yield return Fade(1f);
        
        // Wait for message to display
        yield return new WaitForSeconds(messageDisplayTime);

        // Reset to StartScene (index 0) or reload current scene
        SceneManager.LoadScene("StartScene");
    }

    private IEnumerator Fade(float target)
    {
        if (fadeCanvas == null) yield break;

        float start = fadeCanvas.alpha;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(start, target, t / fadeTime);
            yield return null;
        }

        fadeCanvas.alpha = target;
    }
}
