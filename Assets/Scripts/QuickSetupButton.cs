using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// Script đơn giản để setup Player và Test Manager - có thể chạy từ Inspector
/// Chỉ cần add script này vào một GameObject và click button "Setup Everything"
/// </summary>
public class QuickSetupButton : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Setup Player & Test Manager")]
    public void SetupEverything()
    {
        // Tìm hoặc tạo PlayerSetupHelper
        PlayerSetupHelper setupHelper = FindObjectOfType<PlayerSetupHelper>();
        if (setupHelper == null)
        {
            GameObject helperObj = new GameObject("PlayerSetupHelper");
            setupHelper = helperObj.AddComponent<PlayerSetupHelper>();
            Debug.Log("Created PlayerSetupHelper GameObject");
        }
        
        // Setup Player
        setupHelper.SetupPlayer();
        
        // Tìm hoặc tạo SceneTestManager
        SceneTestManager testManager = FindObjectOfType<SceneTestManager>();
        if (testManager == null)
        {
            GameObject testManagerObj = new GameObject("SceneTestManager");
            testManager = testManagerObj.AddComponent<SceneTestManager>();
            Debug.Log("Created SceneTestManager GameObject");
        }
        
        // Đánh dấu scene đã thay đổi
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        
        Debug.Log("=== Auto Setup Complete ===");
        Debug.Log("Player và SceneTestManager đã được setup!");
        Debug.Log("Bây giờ bạn có thể Play scene để test.");
        
        // Hiển thị dialog thông báo
        EditorUtility.DisplayDialog(
            "Setup Complete", 
            "Player và SceneTestManager đã được setup thành công!\n\n" +
            "Bây giờ bạn có thể:\n" +
            "1. Play scene để test\n" +
            "2. Sử dụng WASD để di chuyển\n" +
            "3. Nhấn F1 để xem debug info\n" +
            "4. Nhấn Escape để toggle cursor",
            "OK"
        );
    }
    
    [ContextMenu("Setup Player Only")]
    public void SetupPlayerOnly()
    {
        // Tìm hoặc tạo PlayerSetupHelper
        PlayerSetupHelper setupHelper = FindObjectOfType<PlayerSetupHelper>();
        if (setupHelper == null)
        {
            GameObject helperObj = new GameObject("PlayerSetupHelper");
            setupHelper = helperObj.AddComponent<PlayerSetupHelper>();
        }
        
        // Setup Player
        setupHelper.SetupPlayer();
        
        // Đánh dấu scene đã thay đổi
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        
        Debug.Log("Player setup complete!");
    }
    
    [ContextMenu("Setup Test Manager Only")]
    public void SetupTestManagerOnly()
    {
        // Tìm hoặc tạo SceneTestManager
        SceneTestManager testManager = FindObjectOfType<SceneTestManager>();
        if (testManager == null)
        {
            GameObject testManagerObj = new GameObject("SceneTestManager");
            testManager = testManagerObj.AddComponent<SceneTestManager>();
            Debug.Log("Created SceneTestManager GameObject");
        }
        else
        {
            Debug.Log("SceneTestManager already exists");
        }
        
        // Đánh dấu scene đã thay đổi
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        
        Debug.Log("SceneTestManager setup complete!");
    }
#endif
}

