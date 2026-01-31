using UnityEngine;
using UnityEngine.InputSystem;

public class TerminalInteract : MonoBehaviour
{
    [SerializeField] private OxygenManager oxygen;

    private bool playerInside;

    private void Awake()
    {
        if (oxygen == null)
            oxygen = FindFirstObjectByType<OxygenManager>();
    }

    private void Update()
    {
        if (!playerInside) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (oxygen != null && oxygen.QuotaMet)
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
