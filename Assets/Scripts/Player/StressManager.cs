using UnityEngine;

/// <summary>
/// Quản lý mức độ căng thẳng của nhân vật
/// Stress tăng khi ma đến gần, giảm khi xa ma hoặc đứng trong đèn
/// </summary>
public class StressManager : MonoBehaviour
{
    [Header("Cài đặt Stress")]
    [Range(0, 100)] public float currentStress = 0f;
    public float maxStress = 100f;

    [Header("Tốc độ & Cân bằng")]
    public float stressIncreaseRate = 5.0f; // Tốc độ tăng khi gặp ma (GIẢM XUỐNG từ 15)
    public float stressDecreaseRate = 3.0f;  // Tốc độ giảm khi an toàn
    public float dangerDistance = 10.0f;     // Khoảng cách bắt đầu thấy sợ (10 mét)

    [Header("Thời gian Ân huệ")]
    public float gracePeriod = 4.0f; // 4 giây đầu game Stress sẽ KHÔNG TĂNG

    [Header("Tham chiếu (Tự động tìm nếu để trống)")]
    public Transform player;
    public Transform[] enemies; // Danh sách ma (có thể để trống, sẽ tự tìm)
    public bool isSafe = false; // Đang đứng trong đèn (được set bởi StreetLamp)
    
    // Biến nội bộ
    private float startTime;
    private FirstPersonController playerController;
    
    // Singleton để gọi từ script khác
    public static StressManager instance;
    
    void Awake()
    {
        instance = this;
        startTime = Time.time;
    }
    
    void Start()
    {
        // Tự tìm Player nếu quên kéo
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) 
            {
                player = p.transform;
            }
            else
            {
                // Fallback: Tìm từ component
                player = transform;
            }
        }
        
        // Tự tìm PlayerController
        playerController = GetComponent<FirstPersonController>();
        if (playerController == null && player != null)
        {
            playerController = player.GetComponent<FirstPersonController>();
        }
        if (playerController == null)
        {
            playerController = FindObjectOfType<FirstPersonController>();
        }
        
        // Tự tìm Ma nếu quên kéo (tìm PaperEnemy components)
        if (enemies == null || enemies.Length == 0)
        {
            // Tìm theo Tag "Enemy" trước
            GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemyObjs.Length > 0)
            {
                enemies = new Transform[enemyObjs.Length];
                for (int i = 0; i < enemyObjs.Length; i++) 
                {
                    enemies[i] = enemyObjs[i].transform;
                }
            }
            else
            {
                // Fallback: Tìm PaperEnemy components
                PaperEnemy[] paperEnemies = FindObjectsOfType<PaperEnemy>();
                if (paperEnemies.Length > 0)
                {
                    enemies = new Transform[paperEnemies.Length];
                    for (int i = 0; i < paperEnemies.Length; i++)
                    {
                        enemies[i] = paperEnemies[i].transform;
                    }
                }
            }
        }
    }
    
    void Update()
    {
        // 1. THỜI GIAN ÂN HUỆ (4 giây đầu không làm gì cả)
        if (Time.time - startTime < gracePeriod)
        {
            currentStress = 0f; // Đảm bảo stress = 0 trong thời gian ân huệ
            if (playerController != null)
            {
                playerController.SetStressLevel(0f);
            }
            return;
        }

        // 2. LOGIC TÍNH TOÁN
        // Mặc định là bình tĩnh (giảm stress)
        bool isPanic = false; 
        float panicFactor = 0f;

        // Nếu ĐANG TRONG ĐÈN -> Chắc chắn an toàn -> Không cần check ma
        if (isSafe)
        {
            DecreaseStress(2.0f); // Giảm nhanh gấp đôi
            currentStress = Mathf.Clamp(currentStress, 0, maxStress);
            UpdatePlayerController();
            return;
        }

        // Nếu RA NGOÀI TỐI -> Kiểm tra xem có ma gần không?
        if (enemies != null && player != null)
        {
            float closestDistance = Mathf.Infinity;

            foreach (Transform enemy in enemies)
            {
                if (enemy == null) continue;
                
                // CHỈ TÍNH ENEMY ĐÃ BẮT ĐẦU HOẠT ĐỘNG
                PaperEnemy paperEnemy = enemy.GetComponent<PaperEnemy>();
                if (paperEnemy != null && !paperEnemy.IsEnemyActive())
                {
                    continue; // Bỏ qua enemy chưa active
                }
                
                float d = Vector3.Distance(player.position, enemy.position);
                if (d < closestDistance) closestDistance = d;
            }

            // Nếu con ma gần nhất nằm trong vùng nguy hiểm
            if (closestDistance < dangerDistance && closestDistance < Mathf.Infinity)
            {
                isPanic = true;
                // Càng gần càng sợ (Công thức: 1 - tỉ lệ khoảng cách)
                panicFactor = 1 - (closestDistance / dangerDistance);
            }
        }

        // 3. ÁP DỤNG KẾT QUẢ
        if (isPanic)
        {
            // Có ma -> Tăng Stress
            currentStress += stressIncreaseRate * panicFactor * Time.deltaTime;
        }
        else
        {
            // Không có ma (dù đang ở trong tối) -> Giảm Stress
            DecreaseStress(1.0f);
        }

        // Kẹp chỉ số 0-100
        currentStress = Mathf.Clamp(currentStress, 0, maxStress);
        
        // Debug log khi stress cao
        if (currentStress >= 90f)
        {
            if (Time.frameCount % 60 == 0) // Log mỗi giây để tránh spam
            {
                Debug.LogWarning($"[StressManager] Stress rất cao: {currentStress:F1}/100");
            }
        }
        if (currentStress >= maxStress - 0.01f)
        {
            Debug.LogError($"[StressManager] ⚠️ STRESS ĐẠT TỐI ĐA: {currentStress:F1}/100 - GAME OVER!");
        }
        
        // Cập nhật stress cho player controller
        UpdatePlayerController();
    }
    
    void UpdatePlayerController()
    {
        if (playerController != null)
        {
            playerController.SetStressLevel(currentStress / maxStress); // Chuyển về 0-1 cho FirstPersonController
        }
    }
    
    void DecreaseStress(float multiplier)
    {
        currentStress -= stressDecreaseRate * multiplier * Time.deltaTime;
    }
    
    
    /// <summary>
    /// Hàm gọi từ script đèn đường để báo hiệu an toàn
    /// </summary>
    public void SetSafeStatus(bool status)
    {
        isSafe = status;
    }
    
    /// <summary>
    /// Thêm stress (để các script khác gọi)
    /// </summary>
    public void AddStress(float amount)
    {
        currentStress = Mathf.Clamp(currentStress + amount, 0, maxStress);
        UpdatePlayerController();
    }
    
    /// <summary>
    /// Đặt mức stress cụ thể
    /// </summary>
    public void SetStress(float level)
    {
        currentStress = Mathf.Clamp(level, 0, maxStress);
        UpdatePlayerController();
    }
    
    /// <summary>
    /// Lấy mức stress hiện tại (0-100)
    /// </summary>
    public float GetStress()
    {
        return currentStress;
    }
    
    /// <summary>
    /// Lấy mức stress dưới dạng phần trăm (0-1)
    /// </summary>
    public float GetStressPercentage()
    {
        return currentStress / maxStress;
    }
    
    /// <summary>
    /// Kiểm tra xem stress có đạt mức tối đa không (Game Over)
    /// </summary>
    public bool IsStressMaxed()
    {
        // Kiểm tra chính xác >= 100.0f (với tolerance nhỏ để tránh lỗi float)
        return currentStress >= (maxStress - 0.01f);
    }
    
    /// <summary>
    /// Reset stress về 0
    /// </summary>
    public void ResetStress()
    {
        currentStress = 0f;
        UpdatePlayerController();
    }
}










