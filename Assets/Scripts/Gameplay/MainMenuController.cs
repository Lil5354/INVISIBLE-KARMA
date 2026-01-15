using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện bắt buộc để chuyển cảnh

/// <summary>
/// Script điều khiển Main Menu
/// Quản lý các nút Play, Exit, Option
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Cài đặt Màn Chơi")]
    [Tooltip("Tên scene Intro để chuyển đến khi bấm Play")]
    public string introSceneName = "IntroStrBFC1.1"; // Scene intro sẽ chạy trước
    
    [Tooltip("Dùng tên scene thay vì index (khuyến nghị)")]
    public bool useSceneName = true;
    
    [Tooltip("Index của scene trong Build Settings (backup nếu không dùng tên)")]
    public int introSceneIndex = 1;

    [Header("Cài đặt UI Phụ")]
    public GameObject optionsPanel; // Kéo cái bảng Option vào đây (nếu có)

    /// <summary>
    /// CHỨC NĂNG CHO NÚT PLAY - Chuyển sang scene Intro
    /// </summary>
    public void PlayGame()
    {
        if (useSceneName)
        {
            // Dùng tên scene (khuyến nghị)
            Debug.Log($"[MainMenuController] Đang load scene: {introSceneName}");
            
            // Kiểm tra scene có tồn tại không
            bool sceneExists = false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (sceneName == introSceneName)
                {
                    sceneExists = true;
                    Debug.Log($"[MainMenuController] ✅ Tìm thấy scene '{introSceneName}' (Index: {i})");
                    break;
                }
            }
            
            if (!sceneExists)
            {
                Debug.LogError($"[MainMenuController] ❌ KHÔNG TÌM THẤY SCENE '{introSceneName}' TRONG BUILD SETTINGS!");
                Debug.LogError($"[MainMenuController] Vui lòng thêm scene vào File -> Build Settings");
                return;
            }
            
            SceneManager.LoadScene(introSceneName);
            Debug.Log($"[MainMenuController] ✅ Đã load scene: {introSceneName}");
        }
        else
        {
            // Dùng index (backup)
            Debug.Log($"[MainMenuController] Đang load scene với index: {introSceneIndex}");
            
            if (introSceneIndex < 0 || introSceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError($"[MainMenuController] Scene index {introSceneIndex} không hợp lệ!");
                return;
            }
            
            SceneManager.LoadScene(introSceneIndex);
            Debug.Log($"[MainMenuController] ✅ Đã load scene index {introSceneIndex}");
        }
    }

    /// <summary>
    /// CHỨC NĂNG CHO NÚT EXIT
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("[MainMenuController] Đã thoát game!"); // Hiện dòng này trong Unity Editor để biết đã bấm
        Application.Quit(); // Lệnh này chỉ chạy khi Build ra file .exe
        
        // Trong Unity Editor, Application.Quit() không hoạt động, nên dùng lệnh này để test
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    /// <summary>
    /// CHỨC NĂNG CHO NÚT OPTION - Mở bảng cài đặt
    /// </summary>
    public void OpenOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(true); // Bật bảng cài đặt lên
            Debug.Log("[MainMenuController] Đã mở bảng Options");
        }
        else
        {
            Debug.LogWarning("[MainMenuController] Options Panel chưa được gán!");
        }
    }

    /// <summary>
    /// CHỨC NĂNG CHO NÚT OPTION - Đóng bảng cài đặt
    /// </summary>
    public void CloseOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false); // Tắt bảng cài đặt đi
            Debug.Log("[MainMenuController] Đã đóng bảng Options");
        }
    }

    /// <summary>
    /// CHỨC NĂNG CHO NÚT RESTART (nếu cần dùng ở màn hình Game Over)
    /// </summary>
    public void RestartGame()
    {
        // Reload lại màn chơi hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log($"[MainMenuController] Đã restart màn chơi: {SceneManager.GetActiveScene().name}");
    }
}

