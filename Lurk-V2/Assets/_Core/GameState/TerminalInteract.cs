using UnityEngine;

public class TerminalInteract : MonoBehaviour
{
    [SerializeField] private OxygenManager oxygen;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool playerInside;

    private void Awake()
    {
        if (oxygen == null)
            oxygen = FindFirstObjectByType<OxygenManager>();
    }

    private void Update()
    {
        if (!playerInside) return;
        if (Input.GetKeyDown(interactKey))
        {
            if (oxygen != null && oxygen.OxygenPercentRemaining > 0f && oxygen.QuotaMet)
            {
                Debug.Log("[RUN] Terminal used: QUOTA MET -> WIN");
                FindFirstObjectByType<RunStateManager>()?.OnPlayerWon();
            }
            else
            {
                Debug.Log("[RUN] Terminal used: quota NOT met");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}
