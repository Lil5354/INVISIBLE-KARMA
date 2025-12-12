using UnityEngine;

/// <summary>
/// Quản lý mức độ căng thẳng của nhân vật
/// Tự động tăng stress khi có sự kiện đáng sợ xảy ra
/// </summary>
public class StressManager : MonoBehaviour
{
    [Header("Stress Settings")]
    [SerializeField] private float baseStressDecay = 0.1f; // Giảm stress tự nhiên mỗi giây
    [SerializeField] private float maxStressLevel = 1f;
    [SerializeField] private float minStressLevel = 0f;
    
    [Header("Stress Triggers")]
    [SerializeField] private float jumpScareStress = 0.3f; // Stress khi jumpscare
    [SerializeField] private float paperFigureMoveStress = 0.05f; // Stress khi hình nhân di chuyển
    [SerializeField] private float chaseStress = 0.5f; // Stress khi bị đuổi
    
    private FirstPersonController playerController;
    private float currentStress = 0f;
    
    void Start()
    {
        playerController = GetComponent<FirstPersonController>();
        if (playerController == null)
        {
            playerController = FindObjectOfType<FirstPersonController>();
        }
    }
    
    void Update()
    {
        // Tự động giảm stress theo thời gian
        if (currentStress > minStressLevel)
        {
            currentStress -= baseStressDecay * Time.deltaTime;
            currentStress = Mathf.Clamp(currentStress, minStressLevel, maxStressLevel);
            
            if (playerController != null)
            {
                playerController.SetStressLevel(currentStress);
            }
        }
    }
    
    /// <summary>
    /// Tăng stress khi có sự kiện đáng sợ
    /// </summary>
    public void TriggerJumpscare()
    {
        AddStress(jumpScareStress);
    }
    
    /// <summary>
    /// Tăng stress khi hình nhân di chuyển
    /// </summary>
    public void OnPaperFigureMoved()
    {
        AddStress(paperFigureMoveStress);
    }
    
    /// <summary>
    /// Tăng stress khi bắt đầu chase sequence
    /// </summary>
    public void StartChase()
    {
        AddStress(chaseStress);
    }
    
    /// <summary>
    /// Thêm stress
    /// </summary>
    public void AddStress(float amount)
    {
        currentStress = Mathf.Clamp(currentStress + amount, minStressLevel, maxStressLevel);
        
        if (playerController != null)
        {
            playerController.SetStressLevel(currentStress);
        }
    }
    
    /// <summary>
    /// Đặt mức stress cụ thể
    /// </summary>
    public void SetStress(float level)
    {
        currentStress = Mathf.Clamp(level, minStressLevel, maxStressLevel);
        
        if (playerController != null)
        {
            playerController.SetStressLevel(currentStress);
        }
    }
    
    /// <summary>
    /// Lấy mức stress hiện tại
    /// </summary>
    public float GetStress()
    {
        return currentStress;
    }
    
    /// <summary>
    /// Reset stress về 0
    /// </summary>
    public void ResetStress()
    {
        currentStress = 0f;
        if (playerController != null)
        {
            playerController.SetStressLevel(0f);
        }
    }
}










