using UnityEngine;
using UnityEngine.UI;

public class OxygenUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image oxygenFill;

    [Header("Settings")]
    [SerializeField] private Color normalColor = Color.cyan;
    [SerializeField] private Color lowOxygenColor = Color.yellow;
    [SerializeField] private Color criticalColor = Color.red;
    [SerializeField] private float lowOxygenThreshold = 0.5f;
    [SerializeField] private float criticalThreshold = 0.25f;

    private OxygenManager oxygen;

    private void Start()
    {
        oxygen = FindFirstObjectByType<OxygenManager>();

        if (oxygen == null)
        {
            Debug.LogWarning("[OxygenUI] OxygenManager not found!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (oxygen == null || oxygenFill == null) return;

        float oxygenPercent = oxygen.OxygenPercentRemaining;

        // Update fill amount
        oxygenFill.fillAmount = oxygenPercent;

        // Change color based on oxygen level
        if (oxygenPercent <= criticalThreshold)
            oxygenFill.color = criticalColor;
        else if (oxygenPercent <= lowOxygenThreshold)
            oxygenFill.color = lowOxygenColor;
        else
            oxygenFill.color = normalColor;
    }
}
