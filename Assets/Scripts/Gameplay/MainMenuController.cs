using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện bắt buộc để chuyển cảnh

/// <summary>
/// Script điều khiển Main Menu
/// Quản lý các nút Play, Exit, Option
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Cài đặt Màn Chơi")]
    [Tooltip("Index của scene Chapter1 trong Build Settings (MainMenu = 0, Chapter1 = 1)")]
    public int chapter1SceneIndex = 1; // Index 1 = Chapter1 trong Build Settings

    [Header("Cài đặt UI Phụ")]
    public GameObject optionsPanel; // Kéo cái bảng Option vào đây (nếu có)

    /// <summary>
    /// CHỨC NĂNG CHO NÚT PLAY - Dùng Index thay vì tên (an toàn hơn)
    /// </summary>
    public void PlayGame()
    {
        Debug.Log($"[MainMenuController] Đang load màn chơi với index: {chapter1SceneIndex}");
        
        // Kiểm tra index có hợp lệ không
        if (chapter1SceneIndex < 0 || chapter1SceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"[MainMenuController] Scene index {chapter1SceneIndex} không hợp lệ! Tổng số scene trong Build Settings: {SceneManager.sceneCountInBuildSettings}");
            Debug.LogError($"[MainMenuController] Vui lòng kiểm tra Build Settings và đảm bảo Chapter1 đã được thêm vào!");
            return;
        }
        
        // Load scene bằng index (an toàn hơn dùng tên)
        SceneManager.LoadScene(chapter1SceneIndex);
        Debug.Log($"[MainMenuController] ✅ Đã load scene index {chapter1SceneIndex} thành công!");
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

