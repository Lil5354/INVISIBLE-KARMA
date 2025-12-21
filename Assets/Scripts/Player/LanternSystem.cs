using UnityEngine;
using UnityEngine.UI; // Nếu bạn muốn làm thanh UI hiển thị dầu sau này
using System; // Cho event

/// <summary>
/// Hệ thống đèn lồng gắn trên người player
/// Quản lý việc Bật/Tắt đèn và tiêu hao dầu đèn
/// </summary>
public class LanternSystem : MonoBehaviour
{
    // Event để thông báo khi dầu thay đổi (UI có thể subscribe để cập nhật)
    public static event Action<float, float> OnOilChanged; // (currentOil, maxOil)
    [Header("Cài đặt Đèn")]
    public Light lanternLight; // Point Light gắn trên player
    public bool isLanternOn = false;

    [Header("Cài đặt Nhiên liệu")]
    public float maxOil = 100f;
    [SerializeField] private float _currentOil = 100f; // Dùng property để tránh Inspector override
    public float currentOil 
    { 
        get { return _currentOil; } 
        set 
        { 
            _currentOil = Mathf.Clamp(value, 0f, maxOil);
            Debug.Log($"[LanternSystem] currentOil được set: {value:F1} → {_currentOil:F1} (Instance: {GetInstanceID()})");
        } 
    }
    public float drainRate = 5f; // Tốn 5 dầu mỗi giây khi đèn bật

    [Header("Cài đặt Vùng An Toàn")]
    public float safeDistance = 6.0f; // Bán kính vùng sáng bảo vệ (mét)

    [Header("Hiệu ứng")]
    public float normalIntensity = 1.5f; // Cường độ đèn bình thường
    public float lowOilIntensityMin = 0.5f; // Cường độ tối thiểu khi sắp hết dầu
    public float lowOilIntensityMax = 1.5f; // Cường độ tối đa khi sắp hết dầu

    void Start()
    {
        Debug.Log($"[LanternSystem] Start() - Instance ID: {GetInstanceID()}, GameObject: {gameObject.name}");
        _currentOil = maxOil;
        Debug.Log($"[LanternSystem] Start() - currentOil được khởi tạo: {currentOil:F1}/{maxOil}");
        
        // Tự động tìm Light nếu chưa gán
        if (lanternLight == null)
        {
            FindLanternLight();
        }
        
        // Tắt đèn lúc đầu
        if (lanternLight != null)
        {
            lanternLight.enabled = false;
            // Đảm bảo Light là Point Light
            if (lanternLight.type != LightType.Point)
            {
                Debug.LogWarning("LanternSystem: Light không phải Point Light! Đang chuyển sang Point Light...");
                lanternLight.type = LightType.Point;
            }
            // Đảm bảo Range khớp với safeDistance
            if (Mathf.Abs(lanternLight.range - safeDistance) > 0.1f)
            {
                Debug.LogWarning($"LanternSystem: Range của Light ({lanternLight.range}) không khớp với Safe Distance ({safeDistance}). Đang cập nhật...");
                lanternLight.range = safeDistance;
            }
            Debug.Log($"LanternSystem: Đã tìm thấy và cấu hình đèn lồng! Range: {lanternLight.range}, Safe Distance: {safeDistance}");
        }
        else
        {
            Debug.LogError("LanternSystem: Chưa tìm thấy Light component! Hãy tạo Point Light và gán vào đây, hoặc đặt tên là 'LanternLight'.");
        }
    }
    
    /// <summary>
    /// Tự động tìm Light component trong Player và children
    /// </summary>
    void FindLanternLight()
    {
        // 1. Tìm trong children trực tiếp
        lanternLight = GetComponentInChildren<Light>();
        
        // 2. Tìm trong CameraHolder
        if (lanternLight == null)
        {
            Transform cameraHolder = transform.Find("CameraHolder");
            if (cameraHolder != null)
            {
                lanternLight = cameraHolder.GetComponentInChildren<Light>();
            }
        }
        
        // 3. Tìm bằng tên "LanternLight"
        if (lanternLight == null)
        {
            Transform lanternObj = transform.Find("LanternLight");
            if (lanternObj != null)
            {
                lanternLight = lanternObj.GetComponent<Light>();
            }
        }
        
        // 4. Tìm trong CameraHolder với tên "LanternLight"
        if (lanternLight == null)
        {
            Transform cameraHolder = transform.Find("CameraHolder");
            if (cameraHolder != null)
            {
                Transform lanternObj = cameraHolder.Find("LanternLight");
                if (lanternObj != null)
                {
                    lanternLight = lanternObj.GetComponent<Light>();
                }
            }
        }
        
        // 5. Tìm tất cả Light trong scene và chọn cái gần nhất
        if (lanternLight == null)
        {
            Light[] allLights = FindObjectsOfType<Light>();
            float closestDistance = float.MaxValue;
            Light closestLight = null;
            
            foreach (Light light in allLights)
            {
                if (light.type == LightType.Point)
                {
                    float dist = Vector3.Distance(transform.position, light.transform.position);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestLight = light;
                    }
                }
            }
            
            if (closestLight != null && closestDistance < 5f) // Chỉ lấy nếu gần hơn 5m
            {
                lanternLight = closestLight;
                Debug.LogWarning($"LanternSystem: Tìm thấy Point Light gần nhất ({closestLight.name}) và tự động gán. Hãy kiểm tra lại setup!");
            }
        }
    }

    void Update()
    {
        // Tìm lại Light nếu bị mất (trong trường hợp bị disable/enable)
        if (lanternLight == null)
        {
            FindLanternLight();
        }
        
        // Đảm bảo Light component và isLanternOn đồng bộ
        if (lanternLight != null)
        {
            // Nếu isLanternOn = true nhưng Light bị tắt → Bật lại
            if (isLanternOn && !lanternLight.enabled)
            {
                lanternLight.enabled = true;
                Debug.LogWarning("LanternSystem: Light component bị tắt nhưng isLanternOn = true. Đã tự động bật lại!");
            }
            // Nếu isLanternOn = false nhưng Light đang bật → Tắt
            else if (!isLanternOn && lanternLight.enabled)
            {
                lanternLight.enabled = false;
            }
        }
        
        // Phím F để Bật/Tắt đèn
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleLantern();
        }

        // Logic tiêu hao dầu
        if (isLanternOn && _currentOil > 0)
        {
            float oldOil = _currentOil;
            _currentOil -= drainRate * Time.deltaTime;
            
            // Đảm bảo currentOil không bị âm
            if (_currentOil < 0)
            {
                _currentOil = 0;
            }
            
            // Thông báo UI khi dầu giảm (mỗi 0.5 giây để tránh spam)
            if (Time.frameCount % 30 == 0 && Mathf.Abs(oldOil - _currentOil) > 0.1f)
            {
                NotifyOilChanged();
            }
            
            // Hiệu ứng đèn nhấp nháy khi sắp hết dầu (Dưới 20%)
            if (lanternLight != null && lanternLight.enabled)
            {
                if (currentOil < 20)
                {
                    lanternLight.intensity = UnityEngine.Random.Range(lowOilIntensityMin, lowOilIntensityMax); // Nhấp nháy
                }
                else
                {
                    lanternLight.intensity = normalIntensity; // Cường độ bình thường
                }
            }
        }
        
        // Kiểm tra hết dầu (chỉ khi đèn đang bật)
        if (_currentOil <= 0 && isLanternOn)
        {
            // Hết dầu thì tắt ngóm
            _currentOil = 0;
            isLanternOn = false;
            if (lanternLight != null)
            {
                lanternLight.enabled = false;
            }
            Debug.Log("[LanternSystem] Đèn đã hết dầu và tự động tắt!");
        }
    }

    /// <summary>
    /// Bật/Tắt đèn lồng
    /// </summary>
    public void ToggleLantern()
    {
        // Tìm lại Light nếu bị mất
        if (lanternLight == null)
        {
            FindLanternLight();
            if (lanternLight == null)
            {
                Debug.LogError("LanternSystem: Không thể bật/tắt đèn vì không tìm thấy Light component!");
                return;
            }
        }
        
        // Kiểm tra dầu với debug log chi tiết
        Debug.Log($"[LanternSystem] ToggleLantern() được gọi - Instance ID: {GetInstanceID()}, GameObject: {gameObject.name}");
        Debug.Log($"[LanternSystem] ToggleLantern: currentOil = {_currentOil:F1}, maxOil = {maxOil}, isLanternOn = {isLanternOn}");
        
        if (_currentOil <= 0)
        {
            Debug.LogWarning($"[LanternSystem] Không thể bật đèn: Hết dầu! (currentOil = {_currentOil:F1}, Instance: {GetInstanceID()})");
            return;
        }

        isLanternOn = !isLanternOn;
        
        if (lanternLight != null)
        {
            lanternLight.enabled = isLanternOn;
            
            if (isLanternOn)
            {
                lanternLight.intensity = normalIntensity;
                Debug.Log($"[LanternSystem] ✅ Đèn đã được bật! Dầu còn lại: {_currentOil:F1}/{maxOil} (Instance: {GetInstanceID()})");
            }
            else
            {
                Debug.Log("[LanternSystem] Đèn đã được tắt!");
            }
        }
        else
        {
            Debug.LogError("LanternSystem: Không thể bật/tắt đèn vì Light component là null!");
        }
    }

    /// <summary>
    /// Sạc đầy dầu (wrapper method - gọi từ StreetLamp)
    /// </summary>
    public void RefillOil()
    {
        Debug.Log($"[LanternSystem] RefillOil() được gọi - Instance ID: {GetInstanceID()}");
        float oldOil = _currentOil;
        _currentOil = maxOil; // Đổ đầy 100%
        
        Debug.Log($"[LanternSystem] ✅ ĐÃ SẠC ĐẦY DẦU! {oldOil:F1} → {_currentOil:F1}/{maxOil} (Instance: {GetInstanceID()})");
        
        // Force update UI bằng cách trigger property setter
        currentOil = _currentOil; // Gán lại để trigger setter và đảm bảo UI được cập nhật
        
        // --- CẬP NHẬT UI NGAY LẬP TỨC (BẤT KỂ ĐÈN ĐANG TẮT HAY BẬT) ---
        UpdateDebugUI(); // Gọi hàm này để force update UI ngay lập tức
        // -------------------------------------------------------------------
        
        // KHÔNG tự động bật đèn - để player tự quyết định khi nào bật
    }
    
    /// <summary>
    /// Thêm dầu vào đèn (khi nhặt được item dầu)
    /// </summary>
    public void AddOil(float amount)
    {
        Debug.Log($"[LanternSystem] AddOil() được gọi - Instance ID: {GetInstanceID()}, GameObject: {gameObject.name}");
        float oldOil = _currentOil;
        
        // Tính toán giá trị mới
        float newOilValue = _currentOil + amount;
        _currentOil = Mathf.Clamp(newOilValue, 0f, maxOil);
        
        Debug.Log($"[LanternSystem] AddOil: {oldOil:F1} + {amount} = {_currentOil:F1}/{maxOil} (Instance: {GetInstanceID()})");
        
        // Đảm bảo currentOil không bị âm hoặc vượt quá maxOil (double check)
        if (_currentOil < 0)
        {
            Debug.LogError($"[LanternSystem] AddOil: currentOil bị âm! ({_currentOil}) - Đang reset về 0");
            _currentOil = 0;
        }
        if (_currentOil > maxOil)
        {
            Debug.LogWarning($"[LanternSystem] AddOil: currentOil vượt quá maxOil! ({_currentOil} > {maxOil}) - Đang clamp về {maxOil}");
            _currentOil = maxOil;
        }
        
        // ĐỌC LẠI GIÁ TRỊ ĐỂ XÁC NHẬN
        float confirmedOil = _currentOil;
        Debug.Log($"[LanternSystem] ✅ Dầu đã được cập nhật: {confirmedOil:F1}/{maxOil} (Instance: {GetInstanceID()})");
        Debug.Log($"[LanternSystem] 🔍 XÁC NHẬN: Property currentOil trả về: {currentOil:F1} (phải khớp với {confirmedOil:F1})");
        
        // Force update UI bằng cách trigger property setter (nếu có UI đang listen)
        // Điều này đảm bảo UI được thông báo về thay đổi
        float temp = currentOil; // Đọc property để trigger getter
        currentOil = _currentOil; // Gán lại để trigger setter và log (nếu có)
        
        // --- CẬP NHẬT UI NGAY LẬP TỨC (BẤT KỂ ĐÈN ĐANG TẮT HAY BẬT) ---
        UpdateDebugUI(); // Gọi hàm này để force update UI ngay lập tức
        // -------------------------------------------------------------------
    }
    
    /// <summary>
    /// Thông báo cho UI khi dầu thay đổi (gọi event)
    /// </summary>
    void NotifyOilChanged()
    {
        // Trigger event để UI có thể cập nhật ngay lập tức
        OnOilChanged?.Invoke(_currentOil, maxOil);
        Debug.Log($"[LanternSystem] 🔔 Đã thông báo UI: Dầu = {_currentOil:F1}/{maxOil}");
    }
    
    /// <summary>
    /// Cập nhật UI ngay lập tức (có thể gọi từ bên ngoài)
    /// Hàm này đảm bảo UI được vẽ lại ngay sau khi sạc dầu
    /// </summary>
    public void UpdateDebugUI()
    {
        // Gọi event để thông báo UI cập nhật
        NotifyOilChanged();
        
        // Force update property để trigger setter (nếu có logic trong setter)
        currentOil = _currentOil;
        
        Debug.Log($"[LanternSystem] ✅ UpdateDebugUI() được gọi - Dầu: {_currentOil:F1}/{maxOil}");
    }

    /// <summary>
    /// Kiểm tra xem một vị trí có nằm trong vùng sáng bảo vệ không
    /// </summary>
    public bool IsInSafeZone(Vector3 position)
    {
        // Đèn phải đang bật
        if (!isLanternOn)
        {
            return false; // Đèn tắt = không có vùng an toàn
        }
        
        // Light component phải được bật
        if (lanternLight == null || !lanternLight.enabled)
        {
            return false; // Light component tắt = không có vùng an toàn
        }
        
        // Tính khoảng cách từ vị trí của Light (không phải từ Player position)
        // Vì đèn lồng có thể ở vị trí khác (trong CameraHolder)
        Vector3 lightPosition = lanternLight.transform.position;
        float distance = Vector3.Distance(lightPosition, position);
        
        // Kiểm tra xem có nằm trong vùng bảo vệ không
        bool inZone = distance < safeDistance;
        
        return inZone;
    }

    /// <summary>
    /// Lấy tỷ lệ dầu còn lại (0-1)
    /// </summary>
    public float GetOilPercentage()
    {
        return _currentOil / maxOil;
    }
    
    /// <summary>
    /// Debug method: Kiểm tra và log giá trị dầu hiện tại
    /// </summary>
    public void DebugOilStatus()
    {
        Debug.Log($"[LanternSystem] 🔍 DEBUG OIL STATUS - Instance ID: {GetInstanceID()}");
        Debug.Log($"[LanternSystem] _currentOil (private): {_currentOil:F1}");
        Debug.Log($"[LanternSystem] currentOil (property): {currentOil:F1}");
        Debug.Log($"[LanternSystem] maxOil: {maxOil:F1}");
        Debug.Log($"[LanternSystem] Percentage: {GetOilPercentage() * 100:F1}%");
    }

    /// <summary>
    /// Kiểm tra xem đèn có đang bật không
    /// </summary>
    public bool IsLanternOn()
    {
        return isLanternOn;
    }
    
    /// <summary>
    /// Vẽ gizmo để debug vùng an toàn trong Scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (lanternLight != null && isLanternOn)
        {
            // Vẽ sphere màu vàng để hiển thị vùng an toàn
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lanternLight.transform.position, safeDistance);
        }
        else if (lanternLight != null)
        {
            // Vẽ sphere màu xám khi đèn tắt
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(lanternLight.transform.position, safeDistance);
        }
        else
        {
            // Vẽ từ Player position nếu chưa có Light
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, safeDistance);
        }
    }
}



