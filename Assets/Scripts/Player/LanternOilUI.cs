using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script quản lý UI hiển thị dầu đèn lồng
/// Subscribe vào event OnOilChanged để cập nhật UI ngay lập tức
/// </summary>
public class LanternOilUI : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("Text component hiển thị số dầu (ví dụ: '50.0/100') - Kéo Text component vào đây")]
    public Text oilText; // UI Text cũ
    
    [Tooltip("Slider hiển thị thanh dầu (0-1)")]
    public Slider oilSlider;
    
    [Tooltip("Image fill hiển thị thanh dầu (dùng fillAmount)")]
    public Image oilFillImage;
    
    [Header("Cài đặt")]
    [Tooltip("Format hiển thị số dầu (ví dụ: 'F1' = 1 chữ số thập phân)")]
    public string oilFormat = "F1";
    
    [Tooltip("Tự động tìm LanternSystem trong scene")]
    public bool autoFindLanternSystem = true;
    
    private LanternSystem lanternSystem;
    private float maxOil = 100f;
    
    void Start()
    {
        // Subscribe vào event
        LanternSystem.OnOilChanged += UpdateOilUI;
        
        // Tìm LanternSystem
        if (autoFindLanternSystem)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                lanternSystem = player.GetComponent<LanternSystem>();
                if (lanternSystem == null)
                {
                    lanternSystem = player.GetComponentInChildren<LanternSystem>();
                }
                
                if (lanternSystem != null)
                {
                    maxOil = lanternSystem.maxOil;
                    // Cập nhật UI lần đầu
                    UpdateOilUI(lanternSystem.currentOil, maxOil);
                    Debug.Log($"[LanternOilUI] Đã tìm thấy LanternSystem và subscribe event!");
                }
                else
                {
                    Debug.LogWarning("[LanternOilUI] Không tìm thấy LanternSystem! UI sẽ không cập nhật.");
                }
            }
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe khi object bị destroy
        LanternSystem.OnOilChanged -= UpdateOilUI;
    }
    
    /// <summary>
    /// Cập nhật UI khi dầu thay đổi (được gọi từ event)
    /// </summary>
    void UpdateOilUI(float currentOil, float maxOilValue)
    {
        maxOil = maxOilValue;
        
        // Cập nhật Text (UI Text cũ)
        if (oilText != null)
        {
            oilText.text = $"{currentOil.ToString(oilFormat)}/{maxOil.ToString(oilFormat)}";
        }
        
        
        // Cập nhật Slider
        if (oilSlider != null)
        {
            oilSlider.value = currentOil / maxOil;
            oilSlider.maxValue = maxOil;
        }
        
        // Cập nhật Image fill
        if (oilFillImage != null)
        {
            oilFillImage.fillAmount = currentOil / maxOil;
        }
        
        Debug.Log($"[LanternOilUI] ✅ UI đã được cập nhật: {currentOil:F1}/{maxOil:F1}");
    }
    
    /// <summary>
    /// Cập nhật UI thủ công (có thể gọi từ Inspector hoặc script khác)
    /// </summary>
    [ContextMenu("Update UI Manually")]
    public void UpdateUIManually()
    {
        if (lanternSystem != null)
        {
            UpdateOilUI(lanternSystem.currentOil, lanternSystem.maxOil);
        }
        else
        {
            Debug.LogWarning("[LanternOilUI] Không có LanternSystem reference! Không thể cập nhật UI.");
        }
    }
}

