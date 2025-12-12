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
    
    [Header("Cài đặt Game")]
    public Vector3 playerStartPosition; // Vị trí bắt đầu của player (tự động lấy)
    public bool autoFindPlayerStart = true; // Tự động tìm vị trí player khi Start
    
    [Header("References")]
    public Transform player; // Reference đến Player
    
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
        
        // Reload lại màn chơi hiện tại
        ReloadScene();
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
        StressManager stressMgr = player.GetComponent<StressManager>();
        if (stressMgr != null)
        {
            stressMgr.ResetStress();
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

