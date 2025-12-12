using UnityEngine;

/// <summary>
/// Helper script để tự động setup Player với PlayerController
/// </summary>
public class PlayerSetupHelper : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupOnStart = false;
    
    [Header("Player Settings")]
    [SerializeField] private Vector3 playerStartPosition = new Vector3(0, 0, 0);
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float playerRadius = 0.5f;
    
    [Header("Camera Settings")]
    [SerializeField] private float cameraHeight = 1.6f;
    [SerializeField] private float fieldOfView = 75f;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupPlayer();
        }
    }
    
    /// <summary>
    /// Tự động setup Player với PlayerController
    /// </summary>
    [ContextMenu("Setup Player")]
    public void SetupPlayer()
    {
        GameObject player = null;
        
        // Tìm hoặc tạo Player
        PlayerController existingController = FindObjectOfType<PlayerController>();
        if (existingController != null)
        {
            player = existingController.gameObject;
            Debug.Log("Found existing Player: " + player.name);
        }
        else
        {
            player = new GameObject("Player");
            player.transform.position = playerStartPosition;
            Debug.Log("Created new Player GameObject");
        }
        
        // Setup CharacterController
        CharacterController charController = player.GetComponent<CharacterController>();
        if (charController == null)
        {
            charController = player.AddComponent<CharacterController>();
        }
        charController.height = playerHeight;
        charController.radius = playerRadius;
        charController.center = new Vector3(0, playerHeight / 2f, 0);
        charController.slopeLimit = 45f;
        charController.stepOffset = 0.3f;
        Debug.Log("CharacterController configured");
        
        // Setup PlayerController
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null)
        {
            playerController = player.AddComponent<PlayerController>();
        }
        Debug.Log("PlayerController added");
        
        // Setup Camera
        SetupCamera(player, cameraHeight, fieldOfView);
        
        // Setup Lantern System
        SetupLanternSystem(player);
        
        Debug.Log("=== Player Setup Complete ===");
        Debug.Log("Player is ready to use!");
        Debug.Log("Controls: WASD to move, Mouse to look, Left Shift to run, Space to jump");
        Debug.Log("Press F to toggle lantern!");
    }
    
    /// <summary>
    /// Setup Camera cho Player
    /// </summary>
    void SetupCamera(GameObject player, float height, float fov)
    {
        // Tìm hoặc tạo CameraHolder
        Transform cameraHolder = player.transform.Find("CameraHolder");
        if (cameraHolder == null)
        {
            GameObject holderObj = new GameObject("CameraHolder");
            holderObj.transform.SetParent(player.transform);
            holderObj.transform.localPosition = new Vector3(0, height, 0);
            holderObj.transform.localRotation = Quaternion.identity;
            cameraHolder = holderObj.transform;
            Debug.Log("Created CameraHolder");
        }
        
        // Tìm hoặc tạo Main Camera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            camObj.AddComponent<AudioListener>();
        }
        
        // Đặt camera làm child của CameraHolder
        if (mainCam.transform.parent != cameraHolder)
        {
            mainCam.transform.SetParent(cameraHolder);
            mainCam.transform.localPosition = Vector3.zero;
            mainCam.transform.localRotation = Quaternion.identity;
        }
        
        mainCam.fieldOfView = fov;
        
        // Gán camera vào PlayerController
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.playerCamera = mainCam;
        }
        
        Debug.Log("Camera configured and assigned to PlayerController");
    }
    
    /// <summary>
    /// Setup Lantern System cho Player
    /// </summary>
    void SetupLanternSystem(GameObject player)
    {
        // Kiểm tra xem đã có LanternSystem chưa
        LanternSystem lanternSystem = player.GetComponent<LanternSystem>();
        if (lanternSystem == null)
        {
            lanternSystem = player.AddComponent<LanternSystem>();
            Debug.Log("LanternSystem component added");
        }
        
        // Tìm CameraHolder
        Transform cameraHolder = player.transform.Find("CameraHolder");
        if (cameraHolder == null)
        {
            Debug.LogWarning("PlayerSetupHelper: CameraHolder không tồn tại! Tạo CameraHolder trước...");
            GameObject holderObj = new GameObject("CameraHolder");
            holderObj.transform.SetParent(player.transform);
            holderObj.transform.localPosition = new Vector3(0, cameraHeight, 0);
            cameraHolder = holderObj.transform;
        }
        
        // Kiểm tra xem đã có Point Light chưa
        Light existingLight = cameraHolder.GetComponentInChildren<Light>();
        if (existingLight == null)
        {
            // Tìm trong Player
            existingLight = player.GetComponentInChildren<Light>();
        }
        
        // Tìm bằng tên "LanternLight"
        if (existingLight == null)
        {
            Transform lanternObj = cameraHolder.Find("LanternLight");
            if (lanternObj != null)
            {
                existingLight = lanternObj.GetComponent<Light>();
            }
        }
        
        // Nếu chưa có, tạo Point Light mới
        if (existingLight == null)
        {
            GameObject lightObj = new GameObject("LanternLight");
            lightObj.transform.SetParent(cameraHolder);
            lightObj.transform.localPosition = new Vector3(0, -0.3f, 0); // Thấp hơn camera một chút
            lightObj.transform.localRotation = Quaternion.identity;
            
            Light pointLight = lightObj.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.range = 6f; // Bán kính vùng an toàn
            pointLight.color = new Color(1f, 0.78f, 0.39f); // Màu vàng cam (#FFC864)
            pointLight.intensity = 1.5f;
            pointLight.shadows = LightShadows.Soft;
            pointLight.enabled = false; // Tắt lúc đầu, script sẽ tự bật
            
            // Gán vào LanternSystem
            lanternSystem.lanternLight = pointLight;
            
            Debug.Log("Created LanternLight Point Light and assigned to LanternSystem");
        }
        else
        {
            // Đảm bảo là Point Light
            if (existingLight.type != LightType.Point)
            {
                existingLight.type = LightType.Point;
                Debug.LogWarning("PlayerSetupHelper: Light không phải Point Light, đã chuyển đổi!");
            }
            
            // Gán vào LanternSystem nếu chưa có
            if (lanternSystem.lanternLight == null)
            {
                lanternSystem.lanternLight = existingLight;
                Debug.Log("Assigned existing Light to LanternSystem");
            }
            
            // Đảm bảo Range khớp với Safe Distance
            if (Mathf.Abs(existingLight.range - lanternSystem.safeDistance) > 0.1f)
            {
                existingLight.range = lanternSystem.safeDistance;
                Debug.Log($"Updated Light range to match Safe Distance: {lanternSystem.safeDistance}");
            }
        }
    }
    
    /// <summary>
    /// Reset Player về vị trí ban đầu
    /// </summary>
    [ContextMenu("Reset Player Position")]
    public void ResetPlayerPosition()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            CharacterController charController = player.GetComponent<CharacterController>();
            if (charController != null)
            {
                charController.enabled = false;
                player.transform.position = playerStartPosition;
                charController.enabled = true;
            }
            else
            {
                player.transform.position = playerStartPosition;
            }
            Debug.Log("Player position reset to: " + playerStartPosition);
        }
    }
}

