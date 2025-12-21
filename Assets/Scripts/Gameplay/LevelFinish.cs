using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện chuyển cảnh
using System.Collections;

/// <summary>
/// Script xử lý khi người chơi đến cuối đường và chiến thắng
/// Gắn vào object WinZone (Cube với Is Trigger = true)
/// </summary>
public class LevelFinish : MonoBehaviour
{
    [Header("Cài đặt Chuyển Màn")]
    [Tooltip("Tên chính xác của màn 2 (phải khớp với tên trong Build Settings)")]
    public string nextSceneName = "Level2"; // Tên chính xác của màn 2 (Tạo sau)
    
    [Tooltip("Thời gian chờ (giây) trước khi chuyển cảnh")]
    public float delayTime = 3.0f; // Chờ 3 giây rồi mới chuyển

    [Header("Giao diện")]
    [Tooltip("Kéo cái WinPanel (UI) vào đây")]
    public GameObject winPanel; // Kéo cái WinPanel vào đây

    [Header("Tùy chọn")]
    [Tooltip("Tự động disable PlayerController khi thắng")]
    public bool disablePlayerOnWin = true;

    private bool hasWon = false; // Cờ kiểm tra để tránh thắng 2 lần
    private PlayerController playerController; // Reference đến PlayerController để disable

    void Start()
    {
        Debug.Log($"[LevelFinish] ✅ Script đã được khởi tạo trên object: {gameObject.name}");
        Debug.Log($"[LevelFinish] 📍 Vị trí WinZone: {transform.position}");
        Debug.Log($"[LevelFinish] 🎯 Next Scene: {nextSceneName} (sẽ setup sau)");
        
        // Kiểm tra Box Collider
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            if (boxCollider.isTrigger)
            {
                Debug.Log($"[LevelFinish] ✅ Box Collider Is Trigger = TRUE (Đúng!)");
            }
            else
            {
                Debug.LogError($"[LevelFinish] ⚠️ Box Collider Is Trigger = FALSE! Hãy bật Is Trigger trong Inspector!");
            }
        }
        else
        {
            Debug.LogError($"[LevelFinish] ⚠️ Không tìm thấy Box Collider! Hãy thêm Box Collider vào object này!");
        }

        // Kiểm tra Mesh Renderer (nên tắt để làm vô hình)
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            if (meshRenderer.enabled)
            {
                Debug.LogWarning($"[LevelFinish] ⚠️ Mesh Renderer đang BẬT! Nên tắt để WinZone vô hình.");
            }
            else
            {
                Debug.Log($"[LevelFinish] ✅ Mesh Renderer đã TẮT (WinZone vô hình)");
            }
        }

        // Kiểm tra WinPanel (chỉ log thông tin, không cảnh báo vì sẽ setup sau)
        if (winPanel != null)
        {
            Debug.Log($"[LevelFinish] ℹ️ WinPanel đã được gán: {winPanel.name}");
        }
        else
        {
            Debug.Log($"[LevelFinish] ℹ️ WinPanel chưa được gán (sẽ setup sau)");
        }

        // Tìm PlayerController để disable khi thắng
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Debug.Log($"[LevelFinish] ✅ Đã tìm thấy PlayerController");
            }
            else
            {
                Debug.LogWarning($"[LevelFinish] ⚠️ Không tìm thấy PlayerController trên Player!");
            }
        }
        else
        {
            Debug.LogWarning($"[LevelFinish] ⚠️ Không tìm thấy GameObject với tag 'Player'!");
        }
    }

    /// <summary>
    /// Hàm này được gọi khi có object đi vào trigger zone
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[LevelFinish] 🔔 OnTriggerEnter được gọi! Object: {other.name}, Tag: {other.tag}");
        
        // Kiểm tra xem ai chạm vào? Phải là Player không?
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[LevelFinish] ✅ Đã phát hiện Player đi vào WinZone!");
            
            if (!hasWon)
            {
                Debug.Log($"[LevelFinish] 🎉 BẮT ĐẦU XỬ LÝ CHIẾN THẮNG!");
                WinGame();
            }
            else
            {
                Debug.LogWarning($"[LevelFinish] ⚠️ Đã thắng rồi, bỏ qua trigger này!");
            }
        }
        else
        {
            Debug.Log($"[LevelFinish] ℹ️ Object '{other.name}' (Tag: {other.tag}) không phải Player, bỏ qua.");
        }
    }

    /// <summary>
    /// Hàm xử lý khi người chơi chiến thắng
    /// </summary>
    void WinGame()
    {
        hasWon = true;
        
        // LOG CHIẾN THẮNG - MỤC ĐÍCH CHÍNH ĐỂ TEST/DEBUG
        Debug.Log("═══════════════════════════════════════");
        Debug.Log("[LevelFinish] 🏆 CHIẾN THẮNG! 🏆");
        Debug.Log("[LevelFinish] ✅ ĐÃ WINGAME - Người chơi đã đến cuối đường!");
        Debug.Log("═══════════════════════════════════════");

        // 1. Kiểm tra WinPanel (chỉ log, không hiện)
        if (winPanel != null)
        {
            Debug.Log($"[LevelFinish] ℹ️ WinPanel đã được gán: {winPanel.name} (chưa hiển thị - sẽ setup sau)");
        }
        else
        {
            Debug.Log("[LevelFinish] ℹ️ WinPanel chưa được setup (sẽ setup sau)");
        }

        // 2. Thông báo cho GameController (nếu có)
        GameController gameController = FindObjectOfType<GameController>();
        if (gameController != null)
        {
            gameController.GameWin();
            Debug.Log("[LevelFinish] ✅ Đã thông báo GameController về chiến thắng");
        }

        // 3. Kiểm tra scene (chỉ log, không chuyển)
        Debug.Log($"[LevelFinish] ℹ️ Next Scene Name được cấu hình: '{nextSceneName}'");
        
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == nextSceneName)
            {
                sceneExists = true;
                Debug.Log($"[LevelFinish] ✅ Tìm thấy scene '{nextSceneName}' trong Build Settings (Index: {i})");
                Debug.Log($"[LevelFinish] ℹ️ Scene đã sẵn sàng, nhưng chưa chuyển cảnh (sẽ setup sau)");
                break;
            }
        }
        
        if (!sceneExists)
        {
            Debug.Log($"[LevelFinish] ℹ️ Scene '{nextSceneName}' chưa có trong Build Settings (sẽ setup sau)");
        }
        
        Debug.Log("[LevelFinish] ✅ TEST HOÀN TẤT - WinGame đã được gọi thành công!");
    }

    /// <summary>
    /// Hàm này được gọi khi object rời khỏi trigger (debug)
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[LevelFinish] 👋 Player đã rời khỏi WinZone: {other.name}");
        }
    }
}
