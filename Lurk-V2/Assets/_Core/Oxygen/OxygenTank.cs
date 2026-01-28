using UnityEngine;
using UnityEngine.InputSystem;

public class OxygenTank : MonoBehaviour
{
    [SerializeField] private float interactRange = 2f;

    private OxygenManager oxygen;
    private Transform player;

    private void Start()
    {
        oxygen = FindFirstObjectByType<OxygenManager>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (oxygen == null) Debug.LogError("OxygenTank: OxygenManager not found in scene!");
        if (player == null) Debug.LogError("OxygenTank: Player tag not found. Tag your Player as 'Player'.");
    }

    private void Update()
    {
        if (oxygen == null || player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);

        if (dist <= interactRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            oxygen.AddTank();
            Destroy(gameObject);
        }
    }
}
