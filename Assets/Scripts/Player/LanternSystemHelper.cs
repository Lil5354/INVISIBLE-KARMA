using UnityEngine;

/// <summary>
/// Helper script để tự động setup và debug LanternSystem
/// Gắn vào Player để tự động tạo Point Light nếu chưa có
/// </summary>
public class LanternSystemHelper : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupOnStart = true;
    
    [Header("Light Settings")]
    [SerializeField] private float lightRange = 6f;
    [SerializeField] private Color lightColor = new Color(1f, 0.78f, 0.39f); // #FFC864
    [SerializeField] private float lightIntensity = 1.5f;
    [SerializeField] private Vector3 lightOffset = new Vector3(0, -0.3f, 0); // Offset từ CameraHolder
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private KeyCode debugKey = KeyCode.L;
    
    private LanternSystem lanternSystem;
    private StressManager stressManager;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupLanternSystem();
        }
        
        // Tìm StressManager
        stressManager = GetComponent<StressManager>();
        if (stressManager == null)
        {
            stressManager = GetComponentInChildren<StressManager>();
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(debugKey) && showDebugInfo)
        {
            DebugLanternSystem();
        }
    }
    
    /// <summary>
    /// Tự động setup LanternSystem và Point Light
    /// </summary>
    [ContextMenu("Setup Lantern System")]
    public void SetupLanternSystem()
    {
        // Tìm hoặc tạo LanternSystem
        lanternSystem = GetComponent<LanternSystem>();
        if (lanternSystem == null)
        {
            lanternSystem = gameObject.AddComponent<LanternSystem>();
            Debug.Log("LanternSystemHelper: Đã tạo LanternSystem component");
        }
        
        // Kiểm tra xem đã có Light chưa
        if (lanternSystem.lanternLight == null)
        {
            CreateLanternLight();
        }
        else
        {
            Debug.Log("LanternSystemHelper: Đã tìm thấy Light component!");
        }
    }
    
    /// <summary>
    /// Tạo Point Light cho đèn lồng
    /// </summary>
    void CreateLanternLight()
    {
        // Tìm CameraHolder
        Transform cameraHolder = transform.Find("CameraHolder");
        if (cameraHolder == null)
        {
            // Tạo CameraHolder nếu chưa có
            GameObject holderObj = new GameObject("CameraHolder");
            holderObj.transform.SetParent(transform);
            holderObj.transform.localPosition = new Vector3(0, 1.6f, 0);
            cameraHolder = holderObj.transform;
            Debug.Log("LanternSystemHelper: Đã tạo CameraHolder");
        }
        
        // Tạo Point Light
        GameObject lightObj = new GameObject("LanternLight");
        lightObj.transform.SetParent(cameraHolder);
        lightObj.transform.localPosition = lightOffset;
        lightObj.transform.localRotation = Quaternion.identity;
        
        Light pointLight = lightObj.AddComponent<Light>();
        pointLight.type = LightType.Point;
        pointLight.range = lightRange;
        pointLight.color = lightColor;
        pointLight.intensity = lightIntensity;
        pointLight.shadows = LightShadows.Soft;
        pointLight.enabled = false; // Tắt lúc đầu
        
        // Gán vào LanternSystem
        lanternSystem.lanternLight = pointLight;
        lanternSystem.safeDistance = lightRange; // Đảm bảo khớp
        
        Debug.Log($"LanternSystemHelper: Đã tạo LanternLight! Range: {lightRange}, Color: {lightColor}");
    }
    
    /// <summary>
    /// Debug thông tin LanternSystem
    /// </summary>
    [ContextMenu("Debug Lantern System")]
    public void DebugLanternSystem()
    {
        if (lanternSystem == null)
        {
            lanternSystem = GetComponent<LanternSystem>();
        }
        
        if (lanternSystem == null)
        {
            Debug.LogError("LanternSystemHelper: Không tìm thấy LanternSystem component!");
            return;
        }
        
        Debug.Log("=== LANTERN SYSTEM DEBUG ===");
        Debug.Log($"Lantern On: {lanternSystem.isLanternOn}");
        Debug.Log($"Current Oil: {lanternSystem.currentOil:F1}/{lanternSystem.maxOil}");
        Debug.Log($"Oil Percentage: {lanternSystem.GetOilPercentage() * 100:F1}%");
        Debug.Log($"Safe Distance: {lanternSystem.safeDistance}");
        
        if (lanternSystem.lanternLight != null)
        {
            Debug.Log($"Light Component: {lanternSystem.lanternLight.name}");
            Debug.Log($"Light Enabled: {lanternSystem.lanternLight.enabled}");
            Debug.Log($"Light Type: {lanternSystem.lanternLight.type}");
            Debug.Log($"Light Range: {lanternSystem.lanternLight.range}");
            Debug.Log($"Light Intensity: {lanternSystem.lanternLight.intensity}");
            Debug.Log($"Light Color: {lanternSystem.lanternLight.color}");
        }
        else
        {
            Debug.LogError("LanternSystemHelper: Light component là NULL!");
        }
        
        Debug.Log("===========================");
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        // Khai báo biến dùng chung
        float yOffset = 10f;
        float lineHeight = 20f;
        
        // LUÔN TÌM LẠI LanternSystem mỗi frame để đảm bảo đọc từ instance đúng
        // (Tránh trường hợp có nhiều instance và đọc nhầm)
        LanternSystem currentLantern = GetComponent<LanternSystem>();
        if (currentLantern == null)
        {
            currentLantern = GetComponentInChildren<LanternSystem>();
        }
        
        // Nếu vẫn không tìm thấy, dùng reference cũ (nếu có)
        if (currentLantern == null)
        {
            currentLantern = lanternSystem;
        }
        
        // Cập nhật reference
        lanternSystem = currentLantern;
        
        if (lanternSystem != null)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            style.normal.textColor = Color.yellow;
            style.alignment = TextAnchor.UpperRight;
            
            GUI.Label(new Rect(Screen.width - 250, yOffset, 240, 20), 
                "=== LANTERN SYSTEM ===", style);
            yOffset += lineHeight;
            
            // Status với màu sắc
            bool isOn = lanternSystem.isLanternOn;
            style.normal.textColor = isOn ? Color.green : Color.red;
            GUI.Label(new Rect(Screen.width - 250, yOffset, 240, 20), 
                $"Status: {(isOn ? "ON" : "OFF")}", style);
            style.normal.textColor = Color.yellow;
            yOffset += lineHeight;
            
            if (lanternSystem.lanternLight == null)
            {
                style.normal.textColor = Color.red;
                GUI.Label(new Rect(Screen.width - 250, yOffset, 240, 20), 
                    "⚠ Light: NULL!", style);
                style.normal.textColor = Color.yellow;
            }
            else
            {
                bool lightEnabled = lanternSystem.lanternLight.enabled;
                style.normal.textColor = lightEnabled ? Color.green : Color.red;
                GUI.Label(new Rect(Screen.width - 250, yOffset, 240, 20), 
                    $"Light: {(lightEnabled ? "ON" : "OFF")}", style);
                style.normal.textColor = Color.yellow;
                
                // Cảnh báo nếu không đồng bộ
                if (isOn != lightEnabled)
                {
                    yOffset += lineHeight;
                    style.normal.textColor = Color.red;
                    GUI.Label(new Rect(Screen.width - 250, yOffset, 240, 20), 
                        "⚠ KHÔNG ĐỒNG BỘ!", style);
                    style.normal.textColor = Color.yellow;
                }
            }
            yOffset += lineHeight;
            
            GUI.Label(new Rect(Screen.width - 250, yOffset, 240, 20), 
                $"Press F to toggle | {debugKey} to debug", style);
        }
        
        // Hiển thị STRESS SYSTEM
        if (stressManager == null)
        {
            stressManager = GetComponent<StressManager>();
            if (stressManager == null)
            {
                stressManager = GetComponentInChildren<StressManager>();
            }
            if (stressManager == null)
            {
                // Thử tìm trong scene
                stressManager = FindObjectOfType<StressManager>();
            }
        }
        
        if (stressManager != null)
        {
            GUIStyle stressStyle = new GUIStyle(GUI.skin.label);
            stressStyle.fontSize = 14;
            stressStyle.alignment = TextAnchor.UpperRight;
            
            // Tính vị trí Y bắt đầu (dưới Lantern System)
            float stressStartY = yOffset + lineHeight + 20f; // Cách Lantern System 20px
            float stressCurrentY = stressStartY;
            float stressLineHeight = 20f;
            
            // Title
            stressStyle.normal.textColor = Color.red;
            GUI.Label(new Rect(Screen.width - 250, stressCurrentY, 240, 20), 
                "=== STRESS SYSTEM ===", stressStyle);
            stressCurrentY += stressLineHeight;
            
            // Stress value
            float currentStress = stressManager.GetStress();
            float maxStress = 100f;
            float stressPercent = stressManager.GetStressPercentage();
            
            // Màu sắc dựa trên mức stress
            if (currentStress >= 80f)
            {
                stressStyle.normal.textColor = Color.red; // Rất nguy hiểm
            }
            else if (currentStress >= 50f)
            {
                stressStyle.normal.textColor = new Color(1f, 0.5f, 0f); // Cam - Cảnh báo
            }
            else if (currentStress >= 25f)
            {
                stressStyle.normal.textColor = Color.yellow; // Vàng - Cẩn thận
            }
            else
            {
                stressStyle.normal.textColor = Color.green; // Xanh - An toàn
            }
            
            GUI.Label(new Rect(Screen.width - 250, stressCurrentY, 240, 20), 
                $"Stress: {currentStress:F1}/{maxStress}", stressStyle);
            stressCurrentY += stressLineHeight;
            
            GUI.Label(new Rect(Screen.width - 250, stressCurrentY, 240, 20), 
                $"Stress: {stressPercent * 100:F1}%", stressStyle);
            stressCurrentY += stressLineHeight;
            
            // Cảnh báo nếu stress cao
            if (currentStress >= 80f)
            {
                stressStyle.normal.textColor = Color.red;
                GUI.Label(new Rect(Screen.width - 250, stressCurrentY, 240, 20), 
                    "⚠️ NGUY HIỂM!", stressStyle);
            }
            else if (currentStress >= 50f)
            {
                stressStyle.normal.textColor = new Color(1f, 0.5f, 0f);
                GUI.Label(new Rect(Screen.width - 250, stressCurrentY, 240, 20), 
                    "⚠️ CẢNH BÁO!", stressStyle);
            }
        }
        else
        {
            // Debug: Hiển thị cảnh báo nếu không tìm thấy StressManager
            GUIStyle warningStyle = new GUIStyle(GUI.skin.label);
            warningStyle.fontSize = 12;
            warningStyle.normal.textColor = Color.red;
            warningStyle.alignment = TextAnchor.UpperRight;
            GUI.Label(new Rect(Screen.width - 250, 150f, 240, 20), 
                "⚠️ StressManager not found!", warningStyle);
        }
    }
}

