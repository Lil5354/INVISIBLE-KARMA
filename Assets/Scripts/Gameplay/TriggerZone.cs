using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    [Header("Cấu hình")]
    public StoryTransition storyManager; // Kéo cái Manager vào đây
    
    [Header("Tùy chọn")]
    [Tooltip("Tự động tìm StoryTransition nếu chưa gán")]
    public bool autoFindStoryManager = true;
    
    [Tooltip("Tắt player movement khi trigger")]
    public bool disablePlayerMovement = true;
    
    [Tooltip("Chỉ trigger một lần")]
    public bool triggerOnce = true;
    
    private bool hasTriggered = false;

    void Start()
    {
        // Tự động tìm StoryTransition nếu chưa gán
        if (storyManager == null && autoFindStoryManager)
        {
            storyManager = FindObjectOfType<StoryTransition>();
            if (storyManager != null)
            {
                Debug.Log($"[TriggerZone] Đã tự động tìm thấy StoryTransition: {storyManager.name}");
            }
            else
            {
                Debug.LogWarning("[TriggerZone] Không tìm thấy StoryTransition trong scene!");
            }
        }
        
        // Đảm bảo có Collider và là Trigger
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning("[TriggerZone] Không có Collider! Đang thêm BoxCollider...");
            col = gameObject.AddComponent<BoxCollider>();
        }
        
        if (!col.isTrigger)
        {
            Debug.LogWarning("[TriggerZone] Collider chưa được set là Trigger! Đang sửa...");
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra đã trigger chưa
        if (triggerOnce && hasTriggered)
        {
            return;
        }
        
        // Kiểm tra xem có phải Player đi vào không
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[TriggerZone] Player đã vào trigger zone: {gameObject.name}");
            
            if (storyManager == null)
            {
                Debug.LogError("[TriggerZone] StoryManager chưa được gán! Hãy kéo StoryTransition vào field 'Story Manager'.");
                return;
            }
            
            // Đánh dấu đã trigger
            hasTriggered = true;
            
            // Tắt script di chuyển của Player (Tùy chọn)
            if (disablePlayerMovement)
            {
                FirstPersonController fpsController = other.GetComponent<FirstPersonController>();
                if (fpsController != null)
                {
                    fpsController.enabled = false;
                    Debug.Log("[TriggerZone] Đã tắt FirstPersonController");
                }
                
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.enabled = false;
                    Debug.Log("[TriggerZone] Đã tắt PlayerController");
                }
            }
            
            // Bắt đầu story transition
            storyManager.BatDauChuyenCanh();
            
            // Hủy collider này để không bị kích hoạt lại
            if (triggerOnce)
            {
                Destroy(GetComponent<Collider>());
                Debug.Log("[TriggerZone] Đã hủy Collider để tránh trigger lại");
            }
        }
        else
        {
            Debug.Log($"[TriggerZone] Vật thể khác vào trigger: {other.name} (Tag: {other.tag})");
        }
    }
    
    /// <summary>
    /// Reset trigger để có thể kích hoạt lại (gọi từ script khác nếu cần)
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        Debug.Log("[TriggerZone] Đã reset trigger");
    }
    
    /// <summary>
    /// Kích hoạt story transition thủ công (không cần player vào trigger)
    /// </summary>
    public void ManualTrigger()
    {
        if (storyManager != null)
        {
            Debug.Log("[TriggerZone] Manual trigger activated");
            hasTriggered = true;
            storyManager.BatDauChuyenCanh();
        }
        else
        {
            Debug.LogError("[TriggerZone] Không thể manual trigger: StoryManager chưa được gán!");
        }
    }
    
    /// <summary>
    /// Vẽ gizmo để debug trong Scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = hasTriggered ? Color.red : Color.green;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            if (col is BoxCollider)
            {
                BoxCollider box = col as BoxCollider;
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphere = col as SphereCollider;
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
        }
    }
}