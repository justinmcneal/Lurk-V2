using UnityEngine;

/// <summary>
/// Automatically adds MeshColliders to all child objects with MeshRenderers.
/// Use: Attach to FBX root, run once in Editor, then remove script.
/// </summary>
public class AutoColliderSetup : MonoBehaviour
{
    [ContextMenu("Add Mesh Colliders to Children")]
    private void AddCollidersToChildren()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        int count = 0;

        foreach (MeshRenderer renderer in renderers)
        {
            // Skip if already has collider
            if (renderer.GetComponent<Collider>() != null)
                continue;

            // Skip roof/ceiling objects
            if (renderer.name.ToLower().Contains("roof") || 
                renderer.name.ToLower().Contains("ceiling"))
            {
                Debug.Log($"[AutoCollider] Skipped roof: {renderer.name}");
                continue;
            }

            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                MeshCollider collider = renderer.gameObject.AddComponent<MeshCollider>();
                collider.sharedMesh = meshFilter.sharedMesh;
                collider.convex = false; // Static geometry
                count++;
            }
        }

        Debug.Log($"[AutoCollider] Added {count} mesh colliders!");
    }

    [ContextMenu("Remove All Colliders")]
    private void RemoveAllColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            DestroyImmediate(col);
        }
        Debug.Log($"[AutoCollider] Removed {colliders.Length} colliders");
    }
}
