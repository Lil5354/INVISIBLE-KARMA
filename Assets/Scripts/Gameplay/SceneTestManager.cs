using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script quản lý scene test - cung cấp các tính năng debug và test
/// </summary>
public class SceneTestManager : MonoBehaviour
{
    [Header("Test Controls")]
    [SerializeField] private KeyCode resetPlayerKey = KeyCode.R;
    [SerializeField] private KeyCode toggleStressKey = KeyCode.T;
    [SerializeField] private KeyCode addStressKey = KeyCode.Y;
    [SerializeField] private KeyCode reduceStressKey = KeyCode.U;
    [SerializeField] private KeyCode toggleCursorKey = KeyCode.Escape;
    
    [Header("Stress Test")]
    [SerializeField] private float stressTestAmount = 0.2f;
    [SerializeField] private bool stressTestMode = false;
    
    [Header("Debug Display")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private KeyCode toggleDebugKey = KeyCode.F1;
    
    [Header("References")]
    [SerializeField] private FirstPersonController playerController;
    [SerializeField] private StressManager stressManager;
    [SerializeField] private Vector3 playerStartPosition = Vector3.zero;
    
    private bool cursorLocked = true;
    private bool debugVisible = true;
    
    void Start()
    {
        // Tự động tìm references nếu chưa gán
        if (playerController == null)
        {
            playerController = FindObjectOfType<FirstPersonController>();
        }
        
        if (stressManager == null)
        {
            stressManager = FindObjectOfType<StressManager>();
        }
        
        // Lưu vị trí bắt đầu
        if (playerController != null)
        {
            playerStartPosition = playerController.transform.position;
        }
        
        // Lock cursor
        LockCursor();
    }
    
    void Update()
    {
        HandleInput();
        
        if (showDebugInfo && debugVisible)
        {
            // Debug info sẽ được hiển thị trong OnGUI
        }
    }
    
    /// <summary>
    /// Xử lý input cho test controls
    /// </summary>
    void HandleInput()
    {
        // Reset Player
        if (Input.GetKeyDown(resetPlayerKey))
        {
            ResetPlayer();
        }
        
        // Toggle Stress Test Mode
        if (Input.GetKeyDown(toggleStressKey))
        {
            stressTestMode = !stressTestMode;
            Debug.Log("Stress Test Mode: " + (stressTestMode ? "ON" : "OFF"));
        }
        
        // Add Stress
        if (Input.GetKeyDown(addStressKey))
        {
            AddStress();
        }
        
        // Reduce Stress
        if (Input.GetKeyDown(reduceStressKey))
        {
            ReduceStress();
        }
        
        // Toggle Cursor
        if (Input.GetKeyDown(toggleCursorKey))
        {
            ToggleCursor();
        }
        
        // Toggle Debug Info
        if (Input.GetKeyDown(toggleDebugKey))
        {
            debugVisible = !debugVisible;
        }
        
        // Stress Test Mode - tự động tăng stress
        if (stressTestMode && stressManager != null)
        {
            stressManager.AddStress(0.1f * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// Reset Player về vị trí ban đầu
    /// </summary>
    void ResetPlayer()
    {
        if (playerController != null)
        {
            // Reset position
            CharacterController charController = playerController.GetComponent<CharacterController>();
            if (charController != null)
            {
                charController.enabled = false;
                playerController.transform.position = playerStartPosition;
                charController.enabled = true;
            }
            else
            {
                playerController.transform.position = playerStartPosition;
            }
            
            // Reset stress
            if (stressManager != null)
            {
                stressManager.ResetStress();
            }
            
            Debug.Log("Player reset to start position");
        }
    }
    
    /// <summary>
    /// Thêm stress
    /// </summary>
    void AddStress()
    {
        if (stressManager != null)
        {
            stressManager.AddStress(stressTestAmount);
            Debug.Log("Stress added: " + stressTestAmount);
        }
        else if (playerController != null)
        {
            playerController.AddStress(stressTestAmount);
            Debug.Log("Stress added: " + stressTestAmount);
        }
    }
    
    /// <summary>
    /// Giảm stress
    /// </summary>
    void ReduceStress()
    {
        if (stressManager != null)
        {
            stressManager.AddStress(-stressTestAmount);
            Debug.Log("Stress reduced: " + stressTestAmount);
        }
        else if (playerController != null)
        {
            playerController.ReduceStress(stressTestAmount);
            Debug.Log("Stress reduced: " + stressTestAmount);
        }
    }
    
    /// <summary>
    /// Toggle cursor lock
    /// </summary>
    void ToggleCursor()
    {
        cursorLocked = !cursorLocked;
        if (cursorLocked)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }
    }
    
    /// <summary>
    /// Lock cursor
    /// </summary>
    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    /// <summary>
    /// Unlock cursor
    /// </summary>
    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    /// <summary>
    /// Hiển thị thông tin debug
    /// </summary>
    void OnGUI()
    {
        if (!showDebugInfo || !debugVisible)
            return;
        
        // Style cho debug text
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;
        
        float yOffset = 10f;
        float lineHeight = 25f;
        
        // Title
        GUI.Label(new Rect(10, yOffset, 400, 20), "=== SCENE TEST MANAGER ===", style);
        yOffset += lineHeight;
        
        // Player Info
        if (playerController != null)
        {
            GUI.Label(new Rect(10, yOffset, 400, 20), 
                "Player Position: " + playerController.transform.position.ToString("F2"), style);
            yOffset += lineHeight;
            
            GUI.Label(new Rect(10, yOffset, 400, 20), 
                "Is Moving: " + playerController.IsMoving(), style);
            yOffset += lineHeight;
            
            GUI.Label(new Rect(10, yOffset, 400, 20), 
                "Speed: " + playerController.GetCurrentSpeed().ToString("F2") + " m/s", style);
            yOffset += lineHeight;
            
            GUI.Label(new Rect(10, yOffset, 400, 20), 
                "Stress Level: " + playerController.GetStressLevel().ToString("F2"), style);
            yOffset += lineHeight;
        }
        
        yOffset += lineHeight;
        
        // Controls
        GUI.Label(new Rect(10, yOffset, 400, 20), "=== CONTROLS ===", style);
        yOffset += lineHeight;
        
        GUI.Label(new Rect(10, yOffset, 400, 20), 
            resetPlayerKey + " - Reset Player", style);
        yOffset += lineHeight;
        
        GUI.Label(new Rect(10, yOffset, 400, 20), 
            toggleStressKey + " - Toggle Stress Test Mode", style);
        yOffset += lineHeight;
        
        GUI.Label(new Rect(10, yOffset, 400, 20), 
            addStressKey + " - Add Stress", style);
        yOffset += lineHeight;
        
        GUI.Label(new Rect(10, yOffset, 400, 20), 
            reduceStressKey + " - Reduce Stress", style);
        yOffset += lineHeight;
        
        GUI.Label(new Rect(10, yOffset, 400, 20), 
            toggleCursorKey + " - Toggle Cursor", style);
        yOffset += lineHeight;
        
        GUI.Label(new Rect(10, yOffset, 400, 20), 
            toggleDebugKey + " - Toggle Debug Info", style);
        yOffset += lineHeight;
        
        // Stress Test Mode Status
        if (stressTestMode)
        {
            style.normal.textColor = Color.red;
            GUI.Label(new Rect(10, yOffset, 400, 20), 
                "STRESS TEST MODE: ACTIVE", style);
            style.normal.textColor = Color.white;
        }
    }
}










