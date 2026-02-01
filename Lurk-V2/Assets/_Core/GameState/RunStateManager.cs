using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunStateManager : MonoBehaviour
{
    public enum RunState { Playing, Won, Dead }

    [SerializeField] private CanvasGroup fadeCanvas; // optional
    [SerializeField] private float fadeTime = 0.6f;
    [SerializeField] private float deathDisplayTime = 2.0f;
    [SerializeField] private float winDisplayTime = 3.0f;

    public RunState State { get; private set; } = RunState.Playing;

    public void OnPlayerWon()
    {
        if (State != RunState.Playing) return;
        State = RunState.Won;
        Debug.Log("[RUN] WIN");
        StartCoroutine(EndRoutine(winDisplayTime));
    }

    public void OnPlayerDied()
    {
        if (State != RunState.Playing) return;
        State = RunState.Dead;
        Debug.Log("[RUN] DEAD");
        StartCoroutine(EndRoutine(deathDisplayTime));
    }

    private IEnumerator EndRoutine(float displayTime)
    {
        // Fade to black
        yield return Fade(1f);
        
        // Wait for message to display
        yield return new WaitForSeconds(displayTime);

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
