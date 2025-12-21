using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Script quản lý Game - Game Over, Reset, Delay
/// </summary>
public class GameController : MonoBehaviour
{
    [Header("Cài đặt UI")]
    public GameObject losePanel; // Kéo Panel chứa chữ "YOU LOSE" vào đây
    public Text loseText; // (Tùy chọn) Kéo Text component vào nếu muốn đổi chữ
    public float loseDisplayTime = 2f; // Thời gian hiển thị "YOU LOSE" trước khi reload
    
    [Header("Cài đặt Scene Thua")]
    [Tooltip("Tên scene thua (scene có hình nhân giấy) - Để trống nếu muốn reload scene hiện tại")]
    public string gameOverSceneName = ""; // Tên scene thua (để trống = reload scene hiện tại)
    
    [Tooltip("Chuyển sang scene thua thay vì reload scene hiện tại")]
    public bool useGameOverScene = false;
    
    [Header("Cài đặt Game")]
    public Vector3 playerStartPosition; // Vị trí bắt đầu của player (tự động lấy)
    public bool autoFindPlayerStart = true; // Tự động tìm vị trí player khi Start
    
    [Header("References")]
    public Transform player; // Reference đến Player
    public StressManager stressManager; // Reference đến StressManager
    
    public static GameController instance; // Singleton để gọi từ bất cứ đâu
    
    private bool gameOver = false;
    private bool gameWon = false;
    
    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Tự động tìm Player nếu chưa gán
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        
        // Tự động tìm StressManager nếu chưa gán
        if (stressManager == null && player != null)
        {
            stressManager = player.GetComponent<StressManager>();
            if (stressManager == null)
            {
                stressManager = player.GetComponentInChildren<StressManager>();
            }
        }
        
        // Lưu vị trí bắt đầu của player
        if (autoFindPlayerStart && player != null)
        {
            playerStartPosition = player.position;
        }
        
        // Ẩn bảng thua lúc đầu game
        if (losePanel != null)
        {
            losePanel.SetActive(false);
        }
    }
    
    void Update()
    {
        // Tự động tìm StressManager nếu chưa có (mỗi frame để đảm bảo)
        if (stressManager == null && player != null)
        {
            stressManager = player.GetComponent<StressManager>();
            if (stressManager == null)
            {
                stressManager = player.GetComponentInChildren<StressManager>();
            }
            if (stressManager == null)
            {
                stressManager = FindObjectOfType<StressManager>();
            }
        }
        
        // Kiểm tra stress >= 100 → Game Over
        if (!gameOver && !gameWon)
        {
            if (stressManager != null)
            {
                float currentStress = stressManager.GetStress();
                
                // Kiểm tra stress >= 100 (với tolerance nhỏ)
                if (currentStress >= 99.9f)
                {
                    Debug.LogError($"[GameController] ⚠️ STRESS ĐẠT 100! ({currentStress:F1}/100) - TRIGGERING GAME OVER!");
                    GameOver();
                }
                else if (currentStress >= 90f)
                {
                    // Debug log khi stress cao (chỉ log mỗi giây để tránh spam)
                    if (Time.frameCount % 60 == 0) // Mỗi ~1 giây (60 FPS)
                    {
                        Debug.LogWarning($"[GameController] Stress cao: {currentStress:F1}/100");
                    }
                }
            }
            else
            {
                // Debug: Cảnh báo nếu không tìm thấy StressManager
                if (Time.frameCount % 300 == 0) // Mỗi ~5 giây
                {
                    Debug.LogWarning("[GameController] Không tìm thấy StressManager! Hãy gắn StressManager vào Player.");
                }
            }
        }
    }
    
    /// <summary>
    /// Hàm này sẽ được gọi khi Ma bắt được Player
    /// </summary>
    public void GameOver()
    {
        if (gameOver || gameWon)
        {
            Debug.LogWarning("GameController: GameOver() đã được gọi nhưng game đã over/win rồi!");
            return; // Tránh gọi nhiều lần
        }
        
        gameOver = true;
        Debug.Log("=== GAME OVER! Player bị bắt! ===");
        
        // 1. Hiện bảng You Lose
        if (losePanel != null)
        {
            losePanel.SetActive(true);
            Debug.Log("GameController: LosePanel đã được bật!");
        }
        else
        {
            Debug.LogError("GameController: LosePanel là NULL! Hãy gán LosePanel vào GameController!");
        }
        
        // 2. Dừng thời gian lại (để ma không chạy nữa)
        Time.timeScale = 0;
        Debug.Log("GameController: Time.timeScale = 0 (Game paused)");
        
        // 3. Đợi người chơi nhấn nút hoặc tự động Reset sau vài giây
        StartCoroutine(ResetGameDelay());
        Debug.Log($"GameController: Sẽ reload scene sau {loseDisplayTime} giây...");
    }
    
    /// <summary>
    /// Reset game sau delay
    /// </summary>
    IEnumerator ResetGameDelay()
    {
        // Chờ loseDisplayTime giây (dùng WaitForSecondsRealtime vì Time.timeScale đang = 0)
        yield return new WaitForSecondsRealtime(loseDisplayTime);
        
        // Trả lại thời gian bình thường trước khi reload
        Time.timeScale = 1;
        
        // Chuyển sang scene thua hoặc reload scene hiện tại
        if (useGameOverScene && !string.IsNullOrEmpty(gameOverSceneName))
        {
            LoadGameOverScene();
        }
        else
        {
            ReloadScene();
        }
    }
    
    /// <summary>
    /// Chuyển sang scene thua (scene có hình nhân giấy)
    /// </summary>
    public void LoadGameOverScene()
    {
        if (string.IsNullOrEmpty(gameOverSceneName))
        {
            Debug.LogWarning("[GameController] Game Over Scene Name chưa được cấu hình! Sẽ reload scene hiện tại.");
            ReloadScene();
            return;
        }
        
        // Mở khóa con trỏ chuột
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log($"[GameController] 🚀 Đang chuyển sang scene thua: {gameOverSceneName}");
        
        // Kiểm tra scene có tồn tại không
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == gameOverSceneName)
            {
                sceneExists = true;
                Debug.Log($"[GameController] ✅ Tìm thấy scene '{gameOverSceneName}' trong Build Settings (Index: {i})");
                break;
            }
        }
        
        if (!sceneExists)
        {
            Debug.LogError($"[GameController] ❌ KHÔNG TÌM THẤY SCENE '{gameOverSceneName}' TRONG BUILD SETTINGS!");
            Debug.LogError($"[GameController] ❌ Vui lòng thêm scene '{gameOverSceneName}' vào File -> Build Settings -> Add Open Scenes");
            Debug.LogError($"[GameController] ⚠️ Sẽ reload scene hiện tại thay vì chuyển scene thua.");
            ReloadScene();
            return;
        }
        
        // Chuyển sang scene thua
        SceneManager.LoadScene(gameOverSceneName);
        Debug.Log($"[GameController] ✅ Đã chuyển sang scene thua: {gameOverSceneName}");
    }
    
    /// <summary>
    /// Reload scene hiện tại
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    /// <summary>
    /// Reset Player về vị trí ban đầu (không reload scene)
    /// </summary>
    public void ResetPlayer()
    {
        if (player == null) return;
        
        CharacterController charController = player.GetComponent<CharacterController>();
        if (charController != null)
        {
            charController.enabled = false;
            player.position = playerStartPosition;
            charController.enabled = true;
        }
        else
        {
            player.position = playerStartPosition;
        }
        
        // Reset stress nếu có
        if (stressManager == null)
        {
            stressManager = player.GetComponent<StressManager>();
            if (stressManager == null)
            {
                stressManager = player.GetComponentInChildren<StressManager>();
            }
        }
        if (stressManager != null)
        {
            stressManager.ResetStress();
        }
        
        // Reset đèn lồng nếu có
        LanternSystem lantern = player.GetComponent<LanternSystem>();
        if (lantern != null)
        {
            lantern.currentOil = lantern.maxOil;
            if (lantern.isLanternOn)
            {
                lantern.ToggleLantern(); // Tắt đèn
            }
        }
        
        Debug.Log("Player đã được reset về vị trí ban đầu!");
    }
    
    /// <summary>
    /// Gọi khi player chiến thắng
    /// </summary>
    public void GameWin()
    {
        if (gameOver || gameWon) return;
        
        gameWon = true;
        Debug.Log("Victory! Bạn đã chiến thắng!");
        
        // Có thể thêm UI victory ở đây
    }
    
    /// <summary>
    /// Kiểm tra game có đang over không
    /// </summary>
    public bool IsGameOver()
    {
        return gameOver;
    }
    
    /// <summary>
    /// Kiểm tra game có đã win không
    /// </summary>
    public bool IsGameWon()
    {
        return gameWon;
    }
}

