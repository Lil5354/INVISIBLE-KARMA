using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện để chuyển cảnh (Load Scene)

/// <summary>
/// Script xử lý khi quái chạm vào Player -> Game Over
/// Gắn vào con quái với Capsule Collider (Is Trigger = true)
/// </summary>
public class MonsterAttack : MonoBehaviour
{
    [Header("Cài đặt Game Over")]
    [Tooltip("Tên scene thua (scene có hình nhân giấy) - Phải khớp với tên trong Build Settings")]
    public string gameOverSceneName = "GameOverScene"; 
    
    [Tooltip("Tự động tìm GameController để gọi GameOver()")]
    public bool useGameController = true;
    
    [Header("Tùy chọn")]
    [Tooltip("Chỉ trigger một lần (tránh spam)")]
    public bool triggerOnce = true;
    
    private bool hasTriggered = false; // Cờ để tránh trigger nhiều lần

    void Start()
    {
        Debug.Log($"[MonsterAttack] Script đã được khởi tạo trên: {gameObject.name}");
        Debug.Log($"[MonsterAttack] Game Over Scene: {gameOverSceneName}");
        
        // Kiểm tra Collider
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            if (col.isTrigger)
            {
                Debug.Log($"[MonsterAttack] ✅ Collider Is Trigger = TRUE (Đúng!)");
            }
            else
            {
                Debug.LogError($"[MonsterAttack] ⚠️ Collider Is Trigger = FALSE! Hãy bật Is Trigger trong Inspector!");
            }
        }
        else
        {
            Debug.LogError($"[MonsterAttack] ⚠️ Không tìm thấy Collider! Hãy thêm Capsule Collider với Is Trigger = true!");
        }
        
        // KIỂM TRA RIGIDBODY (QUAN TRỌNG!)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (rb.isKinematic)
            {
                Debug.Log($"[MonsterAttack] ✅ Rigidbody Is Kinematic = TRUE (Đúng!)");
            }
            else
            {
                Debug.LogError($"[MonsterAttack] ⚠️ Rigidbody Is Kinematic = FALSE! Hãy TÍCH vào ô 'Is Kinematic' trong Inspector!");
                Debug.LogError($"[MonsterAttack] ⚠️ Nếu không tích, con quái sẽ bị đổ nghiêng hoặc chui xuống đất!");
            }
            
            if (!rb.useGravity)
            {
                Debug.Log($"[MonsterAttack] ✅ Rigidbody Use Gravity = FALSE (Đúng! AI tự lo trọng lực)");
            }
            else
            {
                Debug.LogWarning($"[MonsterAttack] ⚠️ Rigidbody Use Gravity = TRUE! Nên BỎ TÍCH để AI tự điều khiển.");
            }
        }
        else
        {
            Debug.LogError($"[MonsterAttack] ❌ KHÔNG TÌM THẤY RIGIDBODY!");
            Debug.LogError($"[MonsterAttack] ❌ Để trigger hoạt động, con quái PHẢI có Rigidbody!");
            Debug.LogError($"[MonsterAttack] ❌ Hãy Add Component -> Rigidbody và cấu hình:");
            Debug.LogError($"[MonsterAttack]    - Use Gravity: ❌ BỎ TÍCH");
            Debug.LogError($"[MonsterAttack]    - Is Kinematic: ✅ TÍCH VÀO");
        }
        
        // Kiểm tra NavMeshAgent (để đảm bảo Stopping Distance = 0)
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            if (agent.stoppingDistance <= 0.1f)
            {
                Debug.Log($"[MonsterAttack] ✅ NavMeshAgent Stopping Distance = {agent.stoppingDistance} (Đúng!)");
            }
            else
            {
                Debug.LogWarning($"[MonsterAttack] ⚠️ NavMeshAgent Stopping Distance = {agent.stoppingDistance} (Quá lớn!)");
                Debug.LogWarning($"[MonsterAttack] ⚠️ Nên chỉnh về 0 để quái lao sát vào Player.");
            }
        }
    }

    /// <summary>
    /// Hàm này tự động chạy khi có vật thể đi vào vùng Trigger
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // Dòng này để kiểm tra xem con quái có chạm vào BẤT CỨ CÁI GÌ không
        Debug.Log($"[MonsterAttack] 🔔 Quái vừa chạm vào: {other.gameObject.name} (Tag: {other.tag})");
        
        // Kiểm tra xem vật đó có phải là Player không
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[MonsterAttack] ✅ -> ĐÚNG LÀ PLAYER RỒI! GAME OVER!");
            
            // Tránh trigger nhiều lần
            if (triggerOnce && hasTriggered)
            {
                Debug.LogWarning("[MonsterAttack] ⚠️ Đã trigger rồi, bỏ qua!");
                return;
            }
            
            hasTriggered = true;
            TriggerGameOver();
        }
        else 
        {
            Debug.Log($"[MonsterAttack] ⚠️ -> Nhưng cái này không phải Player (Sai Tag hoặc va nhầm đồ vật)");
            Debug.Log($"[MonsterAttack] ⚠️ -> Object: {other.name}, Tag: {other.tag}");
            Debug.Log($"[MonsterAttack] ⚠️ -> Hãy kiểm tra Tag của Player có đúng là 'Player' (viết hoa chữ P) không!");
        }
    }
    
    /// <summary>
    /// Xử lý Game Over
    /// </summary>
    void TriggerGameOver()
    {
        Debug.Log("═══════════════════════════════════════");
        Debug.Log("[MonsterAttack] 🎯 ĐÃ BẮT ĐƯỢC NGƯỜI CHƠI! -> GAME OVER");
        Debug.Log("═══════════════════════════════════════");

        // Nếu dùng GameController, gọi method GameOver() của nó
        if (useGameController)
        {
            GameController gameController = GameController.instance;
            if (gameController != null)
            {
                Debug.Log("[MonsterAttack] ✅ Đã tìm thấy GameController, gọi GameOver()...");
                gameController.GameOver();
                
                // Nếu GameController có hỗ trợ chuyển scene thua, nó sẽ tự xử lý
                // Nếu không, chúng ta sẽ chuyển scene sau một chút delay
                StartCoroutine(LoadGameOverSceneDelayed(2f));
                return;
            }
            else
            {
                Debug.LogWarning("[MonsterAttack] ⚠️ Không tìm thấy GameController! Sẽ chuyển scene trực tiếp.");
            }
        }
        
        // Chuyển scene trực tiếp nếu không dùng GameController
        LoadGameOverScene();
    }
    
    /// <summary>
    /// Chuyển sang scene thua ngay lập tức
    /// </summary>
    void LoadGameOverScene()
    {
        // Mở khóa con trỏ chuột để người chơi có thể bấm nút ở màn hình thua
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log($"[MonsterAttack] 🚀 Đang chuyển sang scene: {gameOverSceneName}");
        
        // Kiểm tra scene có tồn tại không
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == gameOverSceneName)
            {
                sceneExists = true;
                Debug.Log($"[MonsterAttack] ✅ Tìm thấy scene '{gameOverSceneName}' trong Build Settings (Index: {i})");
                break;
            }
        }
        
        if (!sceneExists)
        {
            Debug.LogError($"[MonsterAttack] ❌ KHÔNG TÌM THẤY SCENE '{gameOverSceneName}' TRONG BUILD SETTINGS!");
            Debug.LogError($"[MonsterAttack] ❌ Vui lòng thêm scene '{gameOverSceneName}' vào File -> Build Settings -> Add Open Scenes");
            Debug.LogError($"[MonsterAttack] ⚠️ Hoặc đổi tên 'Game Over Scene Name' trong Inspector cho đúng!");
            return;
        }
        
        // Chuyển sang màn hình thua
        SceneManager.LoadScene(gameOverSceneName);
        Debug.Log($"[MonsterAttack] ✅ Đã gọi SceneManager.LoadScene('{gameOverSceneName}')");
    }
    
    /// <summary>
    /// Chuyển scene thua sau delay (để GameController có thời gian hiển thị UI)
    /// </summary>
    System.Collections.IEnumerator LoadGameOverSceneDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LoadGameOverScene();
    }
    
    /// <summary>
    /// Reset trigger (có thể gọi từ script khác nếu cần)
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        Debug.Log("[MonsterAttack] Đã reset trigger");
    }
}


