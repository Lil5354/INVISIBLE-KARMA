using UnityEngine;
using UnityEngine.UI; // Nếu bạn muốn làm thanh UI hiển thị dầu sau này

/// <summary>
/// Hệ thống đèn lồng gắn trên người player
/// Quản lý việc Bật/Tắt đèn và tiêu hao dầu đèn
/// </summary>
public class LanternSystem : MonoBehaviour
{
    [Header("Cài đặt Đèn")]
    public Light lanternLight; // Point Light gắn trên player
    public bool isLanternOn = false;

    [Header("Cài đặt Nhiên liệu")]
    public float maxOil = 100f;
    public float currentOil;
    public float drainRate = 5f; // Tốn 5 dầu mỗi giây khi đèn bật

    [Header("Cài đặt Vùng An Toàn")]
    public float safeDistance = 6.0f; // Bán kính vùng sáng bảo vệ (mét)

    [Header("Hiệu ứng")]
    public float normalIntensity = 1.5f; // Cường độ đèn bình thường
    public float lowOilIntensityMin = 0.5f; // Cường độ tối thiểu khi sắp hết dầu
    public float lowOilIntensityMax = 1.5f; // Cường độ tối đa khi sắp hết dầu

    void Start()
    {
        currentOil = maxOil;
        
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
        if (isLanternOn && currentOil > 0)
        {
            currentOil -= drainRate * Time.deltaTime;
            
            // Hiệu ứng đèn nhấp nháy khi sắp hết dầu (Dưới 20%)
            if (lanternLight != null && lanternLight.enabled)
            {
                if (currentOil < 20)
                {
                    lanternLight.intensity = Random.Range(lowOilIntensityMin, lowOilIntensityMax); // Nhấp nháy
                }
                else
                {
                    lanternLight.intensity = normalIntensity; // Cường độ bình thường
                }
            }
        }
        else if (currentOil <= 0)
        {
            // Hết dầu thì tắt ngóm
            currentOil = 0;
            if (isLanternOn)
            {
                isLanternOn = false;
                if (lanternLight != null)
                {
                    lanternLight.enabled = false;
                }
                Debug.Log("Đèn đã hết dầu!");
            }
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
        
        if (currentOil <= 0)
        {
            Debug.Log("Không thể bật đèn: Hết dầu!");
            return;
        }

        isLanternOn = !isLanternOn;
        
        if (lanternLight != null)
        {
            lanternLight.enabled = isLanternOn;
            
            if (isLanternOn)
            {
                lanternLight.intensity = normalIntensity;
                Debug.Log($"Đèn đã được bật! Dầu còn lại: {currentOil:F1}/{maxOil}");
            }
            else
            {
                Debug.Log("Đèn đã được tắt!");
            }
        }
        else
        {
            Debug.LogError("LanternSystem: Không thể bật/tắt đèn vì Light component là null!");
        }
    }

    /// <summary>
    /// Thêm dầu vào đèn (khi nhặt được item dầu)
    /// </summary>
    public void AddOil(float amount)
    {
        currentOil = Mathf.Clamp(currentOil + amount, 0f, maxOil);
        Debug.Log($"Đã thêm {amount} dầu. Tổng: {currentOil}/{maxOil}");
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
        return currentOil / maxOil;
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



