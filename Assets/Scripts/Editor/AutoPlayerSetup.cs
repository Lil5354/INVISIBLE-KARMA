using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Linq;
using System.Reflection;

/// <summary>
/// Editor script để tự động setup Player và SceneTestManager từ menu Unity
/// Sử dụng: Tools > Auto Setup Player & Test Manager
/// </summary>
public class AutoPlayerSetup
{
    // Helper method để tìm type trong tất cả assemblies
    static Type FindType(string typeName)
    {
        foreach (Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }
        return null;
    }
    
    [MenuItem("Tools/Auto Setup Player & Test Manager")]
    public static void SetupPlayerAndTestManager()
    {
        // Tìm PlayerSetupHelper type bằng reflection
        Type playerSetupHelperType = FindType("PlayerSetupHelper");
        if (playerSetupHelperType == null)
        {
            Debug.LogError("Cannot find PlayerSetupHelper class. Please ensure scripts are compiled.");
            EditorUtility.DisplayDialog("Error", "Cannot find PlayerSetupHelper class. Please check Console for compilation errors.", "OK");
            return;
        }
        
        // Tìm hoặc tạo PlayerSetupHelper
        Component setupHelper = UnityEngine.Object.FindObjectOfType(playerSetupHelperType) as Component;
        if (setupHelper == null)
        {
            GameObject helperObj = new GameObject("PlayerSetupHelper");
            setupHelper = helperObj.AddComponent(playerSetupHelperType);
            Debug.Log("Created PlayerSetupHelper GameObject");
        }
        
        // Gọi SetupPlayer method bằng reflection
        var setupPlayerMethod = playerSetupHelperType.GetMethod("SetupPlayer");
        if (setupPlayerMethod != null)
        {
            setupPlayerMethod.Invoke(setupHelper, null);
        }
        
        // Tìm SceneTestManager type
        Type sceneTestManagerType = FindType("SceneTestManager");
        if (sceneTestManagerType != null)
        {
            Component testManager = UnityEngine.Object.FindObjectOfType(sceneTestManagerType) as Component;
            if (testManager == null)
            {
                GameObject testManagerObj = new GameObject("SceneTestManager");
                testManagerObj.AddComponent(sceneTestManagerType);
                Debug.Log("Created SceneTestManager GameObject");
            }
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
    
    [MenuItem("Tools/Setup Player Only")]
    public static void SetupPlayerOnly()
    {
        // Tìm PlayerSetupHelper type bằng reflection
        Type playerSetupHelperType = FindType("PlayerSetupHelper");
        if (playerSetupHelperType == null)
        {
            Debug.LogError("Cannot find PlayerSetupHelper class. Please ensure scripts are compiled.");
            return;
        }
        
        // Tìm hoặc tạo PlayerSetupHelper
        Component setupHelper = UnityEngine.Object.FindObjectOfType(playerSetupHelperType) as Component;
        if (setupHelper == null)
        {
            GameObject helperObj = new GameObject("PlayerSetupHelper");
            setupHelper = helperObj.AddComponent(playerSetupHelperType);
        }
        
        // Gọi SetupPlayer method
        var setupPlayerMethod = playerSetupHelperType.GetMethod("SetupPlayer");
        if (setupPlayerMethod != null)
        {
            setupPlayerMethod.Invoke(setupHelper, null);
        }
        
        // Đánh dấu scene đã thay đổi
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        
        Debug.Log("Player setup complete!");
    }
    
    [MenuItem("Tools/Setup Test Manager Only")]
    public static void SetupTestManagerOnly()
    {
        // Tìm SceneTestManager type
        Type sceneTestManagerType = FindType("SceneTestManager");
        if (sceneTestManagerType == null)
        {
            Debug.LogError("Cannot find SceneTestManager class. Please ensure scripts are compiled.");
            return;
        }
        
        Component testManager = UnityEngine.Object.FindObjectOfType(sceneTestManagerType) as Component;
        if (testManager == null)
        {
            GameObject testManagerObj = new GameObject("SceneTestManager");
            testManagerObj.AddComponent(sceneTestManagerType);
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
}

