using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunStateManager : MonoBehaviour
{
    public enum RunState { Playing, Won, Dead }

    [SerializeField] private CanvasGroup fadeCanvas; // optional
    [SerializeField] private float fadeTime = 0.6f;
    [SerializeField] private float resetDelay = 1.0f;

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
        yield return Fade(1f);
        yield return new WaitForSeconds(resetDelay);

        // Reset to Level 1 (same scene for now)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
