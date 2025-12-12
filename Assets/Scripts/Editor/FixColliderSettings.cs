using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor script để kiểm tra và sửa Collider settings cho ground
/// </summary>
public class FixColliderSettings : EditorWindow
{
    [MenuItem("Tools/Fix Ground Collider Settings")]
    public static void FixGroundColliders()
    {
        int fixedCount = 0;
        
        // Tìm tất cả GameObjects có Collider
        Collider[] allColliders = FindObjectsOfType<Collider>();
        
        foreach (Collider col in allColliders)
        {
            // Kiểm tra Mesh Collider
            if (col is MeshCollider)
            {
                MeshCollider meshCollider = col as MeshCollider;
                
                // Kiểm tra nếu không có Convex
                if (!meshCollider.convex)
                {
                    // CharacterController không hoạt động với non-convex Mesh Collider
                    // Cần enable Convex hoặc đổi sang Box Collider
                    Debug.LogWarning($"Mesh Collider on '{col.gameObject.name}' is not Convex. CharacterController may not work properly!");
                    
                    // Hỏi user có muốn enable Convex không
                    if (EditorUtility.DisplayDialog(
                        "Fix Mesh Collider",
                        $"Mesh Collider on '{col.gameObject.name}' is not Convex.\n\n" +
                        "CharacterController requires Convex Mesh Collider or Box/Capsule Collider.\n\n" +
                        "Do you want to enable Convex? (This may change the collision shape)",
                        "Enable Convex",
                        "Skip"))
                    {
                        meshCollider.convex = true;
                        fixedCount++;
                        Debug.Log($"Enabled Convex on '{col.gameObject.name}'");
                    }
                }
            }
            
            // Kiểm tra Is Trigger
            if (col.isTrigger)
            {
                Debug.LogWarning($"Collider on '{col.gameObject.name}' is set as Trigger. CharacterController will pass through it!");
                
                if (EditorUtility.DisplayDialog(
                    "Fix Trigger Collider",
                    $"Collider on '{col.gameObject.name}' is set as Trigger.\n\n" +
                    "CharacterController cannot interact with Trigger colliders.\n\n" +
                    "Do you want to disable Trigger?",
                    "Disable Trigger",
                    "Skip"))
                {
                    col.isTrigger = false;
                    fixedCount++;
                    Debug.Log($"Disabled Trigger on '{col.gameObject.name}'");
                }
            }
        }
        
        if (fixedCount > 0)
        {
            EditorUtility.DisplayDialog("Fix Complete", $"Fixed {fixedCount} collider(s)!", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("No Issues", "All colliders are properly configured!", "OK");
        }
    }
    
    [MenuItem("Tools/Check Player Position")]
    public static void CheckPlayerPosition()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            EditorUtility.DisplayDialog("No Player", "No PlayerController found in scene!", "OK");
            return;
        }
        
        CharacterController charController = player.GetComponent<CharacterController>();
        if (charController == null)
        {
            EditorUtility.DisplayDialog("No CharacterController", "PlayerController doesn't have CharacterController component!", "OK");
            return;
        }
        
        // Raycast xuống để tìm ground
        RaycastHit hit;
        bool hasGround = Physics.Raycast(
            player.transform.position,
            Vector3.down,
            out hit,
            charController.height + 5f
        );
        
        if (hasGround)
        {
            float distanceToGround = hit.distance;
            float playerBottom = player.transform.position.y - charController.height / 2f;
            float groundY = hit.point.y;
            float distance = playerBottom - groundY;
            
            string message = $"Player Position: {player.transform.position}\n" +
                           $"Ground Position: {hit.point}\n" +
                           $"Distance to Ground: {distance:F2}\n" +
                           $"Ground Object: {hit.collider.gameObject.name}\n" +
                           $"Ground Collider: {hit.collider.GetType().Name}";
            
            if (hit.collider is MeshCollider)
            {
                MeshCollider meshCol = hit.collider as MeshCollider;
                message += $"\nMesh Collider Convex: {meshCol.convex}";
                message += $"\nMesh Collider Is Trigger: {meshCol.isTrigger}";
            }
            
            EditorUtility.DisplayDialog("Player Position Check", message, "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("No Ground", "No ground detected below player! Check if ground has Collider.", "OK");
        }
    }
}

