using UnityEngine;

/// <summary>
/// Script tương tác của Player - Sử dụng Raycast từ Camera
/// Tương tác với đèn tĩnh bằng phím E
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Cài đặt Tương tác")]
    public float interactRange = 5.0f; // Khoảng cách có thể với tới đèn (5 mét - tăng lên để dễ hơn)
    public LayerMask interactableLayer = -1; // Layer có thể tương tác (mặc định: Everything)
    public bool ignoreLayerMask = true; // Bỏ qua layer mask (để dễ test)
    
    [Header("UI Hiển thị")]
    public bool showInteractPrompt = true; // Hiển thị prompt "Nhấn E để tương tác"
    public KeyCode interactKey = KeyCode.E;
    
    [Header("Debug")]
    public bool showDebugRay = true; // Hiển thị tia ray trong Scene view
    public bool showDebugLogs = true; // Hiển thị debug logs trong Console
    
    private StreetLamp currentLamp; // Đèn đang nhìn vào
    private bool canInteract = false;
    
    void Update()
    {
        // Kiểm tra xem có đang nhìn vào đèn không
        CheckForInteractable();
        
        // Kiểm tra nút bấm E
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }
    
    /// <summary>
    /// Kiểm tra xem có đang nhìn vào vật thể có thể tương tác không
    /// </summary>
    void CheckForInteractable()
    {
        // Tạo tia chiếu từ tâm Camera
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        // Bắn tia ra
        LayerMask layerToUse = ignoreLayerMask ? -1 : interactableLayer; // Bỏ qua layer mask nếu ignoreLayerMask = true
        if (Physics.Raycast(ray, out hit, interactRange, layerToUse))
        {
            // Debug: Vẽ tia raycast
            Debug.DrawRay(transform.position, transform.forward * interactRange, Color.yellow, 0.1f);
            
            // Kiểm tra layer mask (nếu có set)
            if (interactableLayer != -1 && ((1 << hit.collider.gameObject.layer) & interactableLayer) == 0)
            {
                // Vật thể không nằm trong layer được phép
                // Nhưng vẫn tiếp tục kiểm tra StreetLamp (có thể đèn ở layer khác)
            }
            
            // Nếu tia trúng vật thể có gắn script StreetLamp
            StreetLamp lamp = hit.collider.GetComponent<StreetLamp>();
            
            // Tìm trong parent nếu không tìm thấy trực tiếp
            if (lamp == null)
            {
                lamp = hit.collider.GetComponentInParent<StreetLamp>();
            }
            
            // Tìm trong children nếu không tìm thấy
            if (lamp == null)
            {
                lamp = hit.collider.GetComponentInChildren<StreetLamp>();
            }
            
            // Tìm trong root của hit object
            if (lamp == null)
            {
                Transform root = hit.collider.transform.root;
                lamp = root.GetComponent<StreetLamp>();
            }
            
            if (lamp != null)
            {
                currentLamp = lamp;
                canInteract = true;
                return;
            }
            else
            {
                // Debug: Log khi raycast trúng nhưng không tìm thấy StreetLamp
                if (Time.frameCount % 60 == 0) // Chỉ log mỗi 60 frame để không spam
                {
                    Debug.Log($"[PlayerInteraction] Raycast trúng: {hit.collider.name} nhưng không có StreetLamp script!");
                }
            }
        }
        else
        {
            // Debug: Log khi raycast không trúng gì
            if (Time.frameCount % 60 == 0) // Chỉ log mỗi 60 frame
            {
                Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red, 0.1f);
            }
        }
        
        // Không tìm thấy
        currentLamp = null;
        canInteract = false;
    }
    
    /// <summary>
    /// Thử tương tác với vật thể
    /// </summary>
    void TryInteract()
    {
        if (!canInteract || currentLamp == null)
        {
            Debug.LogWarning("PlayerInteraction: Không thể tương tác! canInteract = " + canInteract + ", currentLamp = " + (currentLamp != null ? currentLamp.name : "null"));
            return; // Không có gì để tương tác
        }
        
        Debug.Log($"PlayerInteraction: Đang tương tác với đèn: {currentLamp.name}");
        
        // Gọi hàm bật/tắt đèn
        currentLamp.ToggleLamp();
        
        Debug.Log($"PlayerInteraction: Đã gọi ToggleLamp()! Đèn hiện tại: {(currentLamp.IsLit() ? "BẬT" : "TẮT")}");
    }
    
    /// <summary>
    /// Vẽ tia màu đỏ trong Scene để dễ debug (chỉ hiện trong Unity Editor)
    /// </summary>
    void OnDrawGizmos()
    {
        if (!showDebugRay) return;
        
        Gizmos.color = canInteract ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * interactRange);
    }
    
    /// <summary>
    /// Hiển thị UI prompt (có thể mở rộng với Canvas sau)
    /// </summary>
    void OnGUI()
    {
        if (!showInteractPrompt) return;
        
        if (canInteract && currentLamp != null)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 20;
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            
            string prompt = $"Nhấn {interactKey} để {(currentLamp.IsLit() ? "tắt" : "bật")} đèn";
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height - 100, 300, 30), prompt, style);
        }
    }
}


