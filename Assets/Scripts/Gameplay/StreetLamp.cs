using UnityEngine;

/// <summary>
/// Script cho đèn tĩnh trên đường (3 đèn)
/// Player có thể tương tác bằng phím E để bật/tắt
/// Tích hợp với hệ thống bảo vệ khỏi enemy
/// </summary>
public class StreetLamp : MonoBehaviour
{
    [Header("Cài đặt Đèn")]
    public Light lampLight;          // Kéo Point Light của đèn vào đây
    public GameObject fireParticle;  // (Tùy chọn) Hiệu ứng lửa nếu có
    public bool isOn = false;        // Trạng thái ban đầu
    
    [Header("Vùng An Toàn")]
    public float protectionRadius = 8f; // Bán kính bảo vệ (mét)
    public SphereCollider safeZoneTrigger; // (Tùy chọn) Sphere Collider với Is Trigger = True
    
    [Header("Âm thanh")]
    public AudioSource audioSource;
    public AudioClip lightOnSFX;
    public AudioClip lightOffSFX;
    
    [Header("Tự động thắp sáng")]
    public bool autoLightOnStart = false; // Tự động sáng khi bắt đầu (nếu false, player phải thắp)
    
    private void Start()
    {
        // Đặt trạng thái ban đầu
        if (autoLightOnStart)
        {
            isOn = true;
        }
        UpdateLightState();
        
        // Tự động tìm Light nếu chưa gán
        if (lampLight == null)
        {
            lampLight = GetComponentInChildren<Light>();
        }
        
        // Tự động tạo Safe Zone Trigger nếu chưa có
        if (safeZoneTrigger == null)
        {
            CreateSafeZoneTrigger();
        }
    }
    
    /// <summary>
    /// Tạo vùng an toàn tự động
    /// </summary>
    void CreateSafeZoneTrigger()
    {
        // Tìm hoặc tạo Sphere Collider
        safeZoneTrigger = GetComponent<SphereCollider>();
        if (safeZoneTrigger == null)
        {
            safeZoneTrigger = gameObject.AddComponent<SphereCollider>();
        }
        
        safeZoneTrigger.isTrigger = true;
        safeZoneTrigger.radius = protectionRadius;
        safeZoneTrigger.enabled = isOn; // Chỉ bật khi đèn sáng
    }
    
    /// <summary>
    /// Hàm này sẽ được gọi từ Player khi nhấn E
    /// </summary>
    public void ToggleLamp()
    {
        Debug.Log($"[StreetLamp] ToggleLamp() được gọi! Trạng thái hiện tại: {(isOn ? "BẬT" : "TẮT")}");
        
        isOn = !isOn; // Đảo ngược trạng thái (Tắt -> Bật, Bật -> Tắt)
        
        Debug.Log($"[StreetLamp] Trạng thái mới: {(isOn ? "BẬT" : "TẮT")}");
        
        // Tìm lại Light nếu chưa có
        if (lampLight == null)
        {
            lampLight = GetComponentInChildren<Light>();
            if (lampLight == null)
            {
                Debug.LogError($"[StreetLamp] {gameObject.name}: Không tìm thấy Light component! Hãy gán Point Light vào field 'Lamp Light'.");
            }
            else
            {
                Debug.Log($"[StreetLamp] Đã tự động tìm thấy Light: {lampLight.name}");
            }
        }
        
        UpdateLightState();
        
        // Phát âm thanh
        if (audioSource != null)
        {
            if (isOn && lightOnSFX != null)
            {
                audioSource.PlayOneShot(lightOnSFX);
            }
            else if (!isOn && lightOffSFX != null)
            {
                audioSource.PlayOneShot(lightOffSFX);
            }
        }
        
        Debug.Log($"[StreetLamp] {gameObject.name}: Đèn {(isOn ? "đã được bật" : "đã được tắt")}!");
    }
    
    /// <summary>
    /// Bật đèn (có thể gọi từ script khác)
    /// </summary>
    public void LightUp()
    {
        if (isOn) return; // Đã sáng rồi
        
        isOn = true;
        UpdateLightState();
        
        if (audioSource != null && lightOnSFX != null)
        {
            audioSource.PlayOneShot(lightOnSFX);
        }
        
        Debug.Log("Đèn đã được thắp sáng!");
    }
    
    /// <summary>
    /// Tắt đèn (có thể gọi từ script khác)
    /// </summary>
    public void TurnOff()
    {
        if (!isOn) return; // Đã tắt rồi
        
        isOn = false;
        UpdateLightState();
        
        if (audioSource != null && lightOffSFX != null)
        {
            audioSource.PlayOneShot(lightOffSFX);
        }
        
        Debug.Log("Đèn đã được tắt!");
    }
    
    /// <summary>
    /// Cập nhật trạng thái đèn
    /// </summary>
    void UpdateLightState()
    {
        Debug.Log($"[StreetLamp] UpdateLightState() - isOn = {isOn}");
        
        if (lampLight != null) 
        {
            lampLight.enabled = isOn;
            Debug.Log($"[StreetLamp] Light component: enabled = {lampLight.enabled}, isOn = {isOn}");
            
            // Đảm bảo Light là Point Light
            if (lampLight.type != LightType.Point)
            {
                Debug.LogWarning($"[StreetLamp] Light không phải Point Light! Đang chuyển sang Point Light...");
                lampLight.type = LightType.Point;
            }
        }
        else
        {
            Debug.LogError($"[StreetLamp] {gameObject.name}: lampLight là NULL! Hãy gán Point Light vào field 'Lamp Light'.");
        }
        
        if (fireParticle != null) 
        {
            fireParticle.SetActive(isOn);
            Debug.Log($"[StreetLamp] Fire Particle: active = {fireParticle.activeSelf}");
        }
        
        // Bật/tắt vùng an toàn
        if (safeZoneTrigger != null)
        {
            safeZoneTrigger.enabled = isOn;
            Debug.Log($"[StreetLamp] Safe Zone Trigger: enabled = {safeZoneTrigger.enabled}");
        }
    }
    
    /// <summary>
    /// Kiểm tra xem một vị trí có nằm trong vùng bảo vệ không
    /// </summary>
    public bool IsInProtectionZone(Vector3 position)
    {
        if (!isOn) return false;
        
        float distance = Vector3.Distance(transform.position, position);
        return distance < protectionRadius;
    }
    
    /// <summary>
    /// Kiểm tra đèn có đang sáng không
    /// </summary>
    public bool IsLit()
    {
        return isOn;
    }
    
    /// <summary>
    /// Khi Player đi vào vùng an toàn (nếu dùng Trigger)
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (!isOn) return; // Đèn chưa sáng thì không có tác dụng
        
        if (other.CompareTag("Player"))
        {
            // Có thể giảm stress ở đây nếu muốn
            StressManager stressMgr = other.GetComponent<StressManager>();
            if (stressMgr != null)
            {
                stressMgr.AddStress(-0.1f); // Giảm stress khi vào vùng an toàn
            }
        }
    }
    
    /// <summary>
    /// Vẽ gizmo để debug vùng bảo vệ trong Scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isOn ? Color.yellow : Color.gray;
        Gizmos.DrawWireSphere(transform.position, protectionRadius);
    }
}


