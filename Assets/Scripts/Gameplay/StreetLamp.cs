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
    
    [Header("Sạc Dầu cho Đèn Lồng")]
    public bool canRefillOil = true; // Cho phép sạc dầu khi đèn đã bật
    public float oilRefillAmount = 50f; // Số dầu được sạc mỗi lần (0-100)
    public float refillCooldown = 5f; // Thời gian chờ giữa các lần sạc (giây)
    private float lastRefillTime = -999f; // Thời gian sạc lần cuối
    
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
        
        // Tự động tìm Particle System nếu chưa gán
        if (fireParticle == null)
        {
            ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            if (particles.Length > 0)
            {
                fireParticle = particles[0].gameObject;
                Debug.Log($"[StreetLamp] Đã tự động tìm thấy Particle System: {fireParticle.name}");
            }
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
    /// Chỉ bật/tắt đèn, không sạc dầu
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
    /// Kiểm tra xem có thể sạc dầu không
    /// </summary>
    bool CanRefillOil()
    {
        // Kiểm tra cooldown
        if (Time.time - lastRefillTime < refillCooldown)
        {
            float remainingTime = refillCooldown - (Time.time - lastRefillTime);
            Debug.Log($"[StreetLamp] CanRefillOil: Đang trong cooldown! Còn {remainingTime:F1} giây");
            return false;
        }
        
        // Tìm Player và LanternSystem
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[StreetLamp] CanRefillOil: Không tìm thấy Player! Hãy đảm bảo Player có Tag 'Player'.");
            return false;
        }
        
        LanternSystem lantern = player.GetComponent<LanternSystem>();
        if (lantern == null)
        {
            Debug.LogWarning("[StreetLamp] CanRefillOil: Player không có LanternSystem component!");
            return false;
        }
        
        // Kiểm tra xem dầu đã đầy chưa
        if (lantern.currentOil >= lantern.maxOil)
        {
            Debug.Log($"[StreetLamp] CanRefillOil: Dầu đã đầy! ({lantern.currentOil:F1}/{lantern.maxOil})");
            return false;
        }
        
        Debug.Log($"[StreetLamp] CanRefillOil: ✅ CÓ THỂ SẠC! Dầu hiện tại: {lantern.currentOil:F1}/{lantern.maxOil}");
        return true;
    }
    
    /// <summary>
    /// Sạc dầu cho đèn lồng của player
    /// </summary>
    public void RefillPlayerOil()
    {
        // Kiểm tra cooldown
        if (Time.time - lastRefillTime < refillCooldown)
        {
            float remainingTime = refillCooldown - (Time.time - lastRefillTime);
            Debug.LogWarning($"[StreetLamp] RefillPlayerOil: Phải đợi {remainingTime:F1} giây nữa mới sạc được!");
            return;
        }
        
        // Tìm Player và LanternSystem
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[StreetLamp] RefillPlayerOil: Không tìm thấy Player! Hãy đảm bảo Player có Tag 'Player'.");
            return;
        }
        
        // Kiểm tra xem có bao nhiêu LanternSystem component
        LanternSystem[] allLanterns = player.GetComponents<LanternSystem>();
        if (allLanterns.Length > 1)
        {
            Debug.LogWarning($"[StreetLamp] RefillPlayerOil: CẢNH BÁO! Có {allLanterns.Length} LanternSystem component trên Player! Sẽ dùng component đầu tiên.");
            for (int i = 0; i < allLanterns.Length; i++)
            {
                Debug.LogWarning($"[StreetLamp] LanternSystem #{i}: Instance ID = {allLanterns[i].GetInstanceID()}, currentOil = {allLanterns[i].currentOil:F1}");
            }
        }
        
        LanternSystem lantern = player.GetComponent<LanternSystem>();
        if (lantern == null)
        {
            Debug.LogError("[StreetLamp] RefillPlayerOil: Player không có LanternSystem component!");
            return;
        }
        
        Debug.Log($"[StreetLamp] RefillPlayerOil: Đã tìm thấy LanternSystem - Instance ID: {lantern.GetInstanceID()}, GameObject: {lantern.gameObject.name}");
        
        // Kiểm tra xem dầu đã đầy chưa
        if (lantern.currentOil >= lantern.maxOil)
        {
            Debug.Log($"[StreetLamp] RefillPlayerOil: Đèn lồng đã đầy dầu rồi! ({lantern.currentOil:F1}/{lantern.maxOil})");
            return;
        }
        
        // Sạc dầu
        float oldOil = lantern.currentOil;
        lantern.AddOil(oilRefillAmount);
        lastRefillTime = Time.time;
        
        // ĐỌC LẠI GIÁ TRỊ SAU KHI SẠC để đảm bảo đã cập nhật
        float newOil = lantern.currentOil;
        
        Debug.Log($"[StreetLamp] ✅ ĐÃ SẠC DẦU! {oldOil:F1} → {newOil:F1}/{lantern.maxOil} (+{oilRefillAmount})");
        
        // Kiểm tra xem có cập nhật đúng không
        float expectedOil = Mathf.Clamp(oldOil + oilRefillAmount, 0f, lantern.maxOil);
        if (Mathf.Abs(newOil - expectedOil) > 0.1f)
        {
            Debug.LogWarning($"[StreetLamp] ⚠️ CẢNH BÁO: Dầu không được cập nhật đúng! Mong đợi: {expectedOil:F1}, Thực tế: {newOil:F1}");
            Debug.LogWarning($"[StreetLamp] Đang thử cập nhật lại...");
            // Thử cập nhật lại
            lantern.currentOil = expectedOil;
            Debug.Log($"[StreetLamp] Đã cập nhật lại: {lantern.currentOil:F1}/{lantern.maxOil}");
        }
        else
        {
            Debug.Log($"[StreetLamp] ✅ Dầu đã được cập nhật đúng!");
        }
        
        // Phát âm thanh nếu có
        if (audioSource != null && lightOnSFX != null)
        {
            audioSource.PlayOneShot(lightOnSFX);
        }
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
        
        // Tự động tìm Particle System nếu chưa gán
        if (fireParticle == null)
        {
            ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            if (particles.Length > 0)
            {
                fireParticle = particles[0].gameObject;
                Debug.Log($"[StreetLamp] Đã tự động tìm thấy Particle System: {fireParticle.name}");
            }
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


