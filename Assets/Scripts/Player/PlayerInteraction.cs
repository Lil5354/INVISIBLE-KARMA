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
    public KeyCode interactKey = KeyCode.E; // Nút bật/tắt đèn
    public KeyCode refillKey = KeyCode.R; // Nút sạc dầu
    
    [Header("Debug")]
    public bool showDebugRay = true; // Hiển thị tia ray trong Scene view
    public bool showDebugLogs = true; // Hiển thị debug logs trong Console
    
    private StreetLamp currentLamp; // Đèn đang nhìn vào
    private bool canInteract = false;
    
    void Update()
    {
        // Kiểm tra xem có đang nhìn vào đèn không
        CheckForInteractable();
        
        // Kiểm tra nút bấm E (bật/tắt đèn)
        if (Input.GetKeyDown(interactKey))
        {
            TryToggleLamp();
        }
        
        // Kiểm tra nút bấm R (sạc dầu)
        if (Input.GetKeyDown(refillKey))
        {
            TryRefillOil();
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
            
            // Tìm StreetLamp - ƯU TIÊN TÌM TRONG PARENT TRƯỚC (vì script thường ở object cha)
            StreetLamp lamp = null;
            
            // 1. Tìm trong parent trước (quan trọng nhất - script thường ở object cha như Latern1)
            lamp = hit.collider.GetComponentInParent<StreetLamp>();
            
            // 2. Nếu không tìm thấy trong parent, tìm trực tiếp trên object bị trúng
            if (lamp == null)
            {
                lamp = hit.collider.GetComponent<StreetLamp>();
            }
            
            // 3. Tìm trong children nếu không tìm thấy
            if (lamp == null)
            {
                lamp = hit.collider.GetComponentInChildren<StreetLamp>();
            }
            
            // 4. Tìm trong root của hit object
            if (lamp == null)
            {
                Transform root = hit.collider.transform.root;
                lamp = root.GetComponent<StreetLamp>();
            }
            
            // Tìm trong toàn bộ hierarchy (tìm tất cả StreetLamp trong scene và kiểm tra distance)
            if (lamp == null)
            {
                StreetLamp[] allLamps = FindObjectsOfType<StreetLamp>();
                float closestDistance = float.MaxValue;
                StreetLamp closestLamp = null;
                
                foreach (StreetLamp sceneLamp in allLamps)
                {
                    // Kiểm tra xem hit object có phải là child/parent của lamp không
                    Transform hitTransform = hit.collider.transform;
                    Transform lampTransform = sceneLamp.transform;
                    
                    // Kiểm tra nếu hit object là child của lamp
                    if (hitTransform.IsChildOf(lampTransform) || lampTransform.IsChildOf(hitTransform))
                    {
                        float distance = Vector3.Distance(hitTransform.position, lampTransform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestLamp = sceneLamp;
                        }
                    }
                }
                
                if (closestLamp != null)
                {
                    lamp = closestLamp;
                    if (showDebugLogs)
                    {
                        Debug.Log($"[PlayerInteraction] Tìm thấy StreetLamp '{lamp.name}' gần object '{hit.collider.name}' (khoảng cách: {closestDistance:F2}m)");
                    }
                }
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
                if (Time.frameCount % 60 == 0 && showDebugLogs) // Chỉ log mỗi 60 frame để không spam
                {
                    Debug.LogWarning($"[PlayerInteraction] Raycast trúng: '{hit.collider.name}' nhưng không có StreetLamp script!\n" +
                        $"Hãy gắn StreetLamp component vào object này hoặc parent object của nó.\n" +
                        $"Path: {GetFullPath(hit.collider.transform)}");
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
    /// Thử bật/tắt đèn (phím E)
    /// </summary>
    void TryToggleLamp()
    {
        if (!canInteract || currentLamp == null)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning("PlayerInteraction: Không thể tương tác! canInteract = " + canInteract + ", currentLamp = " + (currentLamp != null ? currentLamp.name : "null"));
            }
            return; // Không có gì để tương tác
        }
        
        Debug.Log($"PlayerInteraction: Đang bật/tắt đèn: {currentLamp.name}");
        
        // Gọi hàm bật/tắt đèn
        currentLamp.ToggleLamp();
        
        Debug.Log($"PlayerInteraction: Đã gọi ToggleLamp()! Đèn hiện tại: {(currentLamp.IsLit() ? "BẬT" : "TẮT")}");
    }
    
    /// <summary>
    /// Thử sạc dầu (phím R)
    /// </summary>
    void TryRefillOil()
    {
        if (!canInteract || currentLamp == null)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning("PlayerInteraction: Không thể sạc dầu! canInteract = " + canInteract + ", currentLamp = " + (currentLamp != null ? currentLamp.name : "null"));
            }
            return; // Không có đèn để sạc dầu
        }
        
        // Chỉ sạc dầu khi đèn đã bật
        if (!currentLamp.IsLit())
        {
            Debug.LogWarning("PlayerInteraction: Không thể sạc dầu! Đèn chưa bật. Hãy bật đèn trước (nhấn E).");
            return;
        }
        
        if (!currentLamp.canRefillOil)
        {
            Debug.LogWarning("PlayerInteraction: Đèn này không cho phép sạc dầu!");
            return;
        }
        
        Debug.Log($"PlayerInteraction: Đang sạc dầu từ đèn: {currentLamp.name}");
        
        // Gọi hàm sạc dầu
        currentLamp.RefillPlayerOil();
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
            
            float yOffset = Screen.height - 100;
            
            // Hiển thị prompt bật/tắt đèn
            string togglePrompt;
            if (!currentLamp.IsLit())
            {
                togglePrompt = $"Nhấn {interactKey} để bật đèn";
            }
            else
            {
                togglePrompt = $"Nhấn {interactKey} để tắt đèn";
            }
            
            GUI.Label(new Rect(Screen.width / 2 - 150, yOffset, 300, 30), togglePrompt, style);
            
            // Hiển thị prompt sạc dầu (chỉ khi đèn đã bật và cho phép sạc)
            if (currentLamp.IsLit() && currentLamp.canRefillOil)
            {
                yOffset += 35;
                string refillPrompt = $"Nhấn {refillKey} để sạc dầu";
                GUI.Label(new Rect(Screen.width / 2 - 150, yOffset, 300, 30), refillPrompt, style);
            }
        }
    }
    
    /// <summary>
    /// Lấy full path của transform (để debug)
    /// </summary>
    string GetFullPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}


