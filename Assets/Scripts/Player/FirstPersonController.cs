using UnityEngine;

/// <summary>
/// First Person Controller với di chuyển, quay đầu, head bobbing và breathing animation
/// Tạo cảm giác tự nhiên và căng thẳng cho nhân vật Linh
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundMask = 1;
    
    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalLookLimit = 80f;
    [SerializeField] private bool invertY = false;
    
    [Header("Head Bobbing Settings")]
    [SerializeField] private float bobbingAmount = 0.05f;
    [SerializeField] private float bobbingSpeed = 10f;
    [SerializeField] private float bobbingSmoothing = 10f;
    
    [Header("Breathing/Idle Animation (Căng thẳng)")]
    [SerializeField] private float breathingIntensity = 0.02f;
    [SerializeField] private float breathingSpeed = 1.5f;
    [SerializeField] private float idleSwayAmount = 0.01f;
    [SerializeField] private float idleSwaySpeed = 0.5f;
    
    [Header("Stress Effects")]
    [SerializeField] private float stressLevel = 0f; // 0-1, tăng dần khi căng thẳng
    [SerializeField] private float maxStressShake = 0.05f;
    [SerializeField] private float heartbeatIntensity = 0.03f;
    
    [Header("Camera Reference")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform cameraHolder; // Empty GameObject để giữ camera
    
    // Private variables
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private float verticalRotation = 0f;
    
    // Head bobbing
    private float bobbingTimer = 0f;
    private float defaultCameraY = 0f;
    private Vector3 cameraOriginalPosition;
    
    // Breathing/Idle
    private float breathingTimer = 0f;
    private float idleSwayTimer = 0f;
    private Vector3 idleSwayOffset = Vector3.zero;
    
    // Stress
    private float stressShakeTimer = 0f;
    private float heartbeatTimer = 0f;
    
    // Movement state
    private Vector2 moveInput;
    private bool isMoving;
    private float currentSpeed;
    
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        
        // Tự động tìm camera nếu chưa gán
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>();
            }
        }
        
        // Tạo camera holder nếu chưa có
        if (cameraHolder == null && playerCamera != null)
        {
            GameObject holder = new GameObject("CameraHolder");
            holder.transform.SetParent(transform);
            holder.transform.localPosition = new Vector3(0, 1.6f, 0); // Chiều cao mắt người
            cameraHolder = holder.transform;
            
            playerCamera.transform.SetParent(cameraHolder);
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.localRotation = Quaternion.identity;
        }
        
        // Lưu vị trí camera gốc
        if (playerCamera != null)
        {
            cameraOriginalPosition = playerCamera.transform.localPosition;
            defaultCameraY = cameraOriginalPosition.y;
        }
        
        // Khóa và ẩn con trỏ chuột
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleHeadBobbing();
        HandleBreathingAndIdle();
        HandleStressEffects();
        ApplyCameraEffects();
    }
    
    /// <summary>
    /// Xử lý quay đầu bằng chuột
    /// </summary>
    void HandleMouseLook()
    {
        // Lấy input chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        // Xoay nhân vật theo trục Y (trái/phải)
        transform.Rotate(Vector3.up * mouseX);
        
        // Xoay camera theo trục X (lên/xuống)
        if (invertY)
            verticalRotation += mouseY;
        else
            verticalRotation -= mouseY;
        
        // Giới hạn góc nhìn lên/xuống
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);
        
        // Áp dụng rotation cho camera holder
        if (cameraHolder != null)
        {
            cameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
    
    /// <summary>
    /// Xử lý di chuyển WASD
    /// </summary>
    void HandleMovement()
    {
        // Kiểm tra đang đứng trên mặt đất
        isGrounded = Physics.CheckSphere(
            transform.position + Vector3.down * (characterController.height / 2 - characterController.radius),
            groundCheckDistance,
            groundMask
        );
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset velocity khi đã chạm đất
        }
        
        // Lấy input di chuyển
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical"); // W/S
        
        moveInput = new Vector2(horizontal, vertical);
        isMoving = moveInput.magnitude > 0.1f;
        
        // Tính toán hướng di chuyển
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection.Normalize();
        
        // Xác định tốc độ (đi bộ hoặc chạy)
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        // Di chuyển nhân vật
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
        
        // Áp dụng trọng lực
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    
    /// <summary>
    /// Xử lý head bobbing khi di chuyển
    /// </summary>
    void HandleHeadBobbing()
    {
        if (isMoving && isGrounded)
        {
            // Tăng timer dựa trên tốc độ
            float speedMultiplier = currentSpeed / walkSpeed;
            bobbingTimer += Time.deltaTime * bobbingSpeed * speedMultiplier;
            
            // Tính toán offset bobbing (sine wave)
            float bobbingOffset = Mathf.Sin(bobbingTimer) * bobbingAmount * speedMultiplier;
            
            // Áp dụng bobbing vào camera
            Vector3 bobbingPosition = cameraOriginalPosition;
            bobbingPosition.y += bobbingOffset;
            
            // Smooth transition
            if (playerCamera != null)
            {
                playerCamera.transform.localPosition = Vector3.Lerp(
                    playerCamera.transform.localPosition,
                    bobbingPosition,
                    Time.deltaTime * bobbingSmoothing
                );
            }
        }
        else
        {
            // Trở về vị trí gốc khi không di chuyển
            if (playerCamera != null)
            {
                playerCamera.transform.localPosition = Vector3.Lerp(
                    playerCamera.transform.localPosition,
                    cameraOriginalPosition,
                    Time.deltaTime * bobbingSmoothing
                );
            }
            bobbingTimer = 0f;
        }
    }
    
    /// <summary>
    /// Xử lý breathing và idle animation khi đứng im (trạng thái căng thẳng)
    /// </summary>
    void HandleBreathingAndIdle()
    {
        if (!isMoving && isGrounded)
        {
            // Breathing animation (thở nhanh khi căng thẳng)
            breathingTimer += Time.deltaTime * breathingSpeed * (1f + stressLevel);
            float breathingOffset = Mathf.Sin(breathingTimer) * breathingIntensity * (1f + stressLevel);
            
            // Idle sway (lắc nhẹ đầu khi căng thẳng)
            idleSwayTimer += Time.deltaTime * idleSwaySpeed;
            float swayX = Mathf.Sin(idleSwayTimer) * idleSwayAmount * (1f + stressLevel * 2f);
            float swayY = Mathf.Cos(idleSwayTimer * 0.7f) * idleSwayAmount * (1f + stressLevel * 2f);
            
            idleSwayOffset = new Vector3(swayX, swayY + breathingOffset, 0f);
        }
        else
        {
            // Reset khi di chuyển
            idleSwayOffset = Vector3.Lerp(idleSwayOffset, Vector3.zero, Time.deltaTime * 5f);
            breathingTimer = 0f;
        }
    }
    
    /// <summary>
    /// Xử lý hiệu ứng căng thẳng (camera shake, heartbeat)
    /// </summary>
    void HandleStressEffects()
    {
        if (stressLevel > 0.1f)
        {
            // Camera shake khi căng thẳng
            stressShakeTimer += Time.deltaTime * 10f;
            float shakeX = (Mathf.PerlinNoise(stressShakeTimer, 0) - 0.5f) * maxStressShake * stressLevel;
            float shakeY = (Mathf.PerlinNoise(0, stressShakeTimer) - 0.5f) * maxStressShake * stressLevel;
            
            // Heartbeat effect (nhịp tim)
            heartbeatTimer += Time.deltaTime * (2f + stressLevel * 3f);
            float heartbeat = Mathf.Sin(heartbeatTimer * Mathf.PI * 2f) * heartbeatIntensity * stressLevel;
            heartbeat = heartbeat > 0 ? heartbeat : 0; // Chỉ shake khi tim đập
            
            idleSwayOffset += new Vector3(shakeX, shakeY + heartbeat, 0f);
        }
    }
    
    /// <summary>
    /// Áp dụng tất cả hiệu ứng camera
    /// </summary>
    void ApplyCameraEffects()
    {
        if (playerCamera != null)
        {
            // Kết hợp tất cả các offset
            Vector3 finalPosition = cameraOriginalPosition + idleSwayOffset;
            
            // Nếu đang di chuyển, head bobbing đã được xử lý riêng
            if (!isMoving)
            {
                playerCamera.transform.localPosition = Vector3.Lerp(
                    playerCamera.transform.localPosition,
                    finalPosition,
                    Time.deltaTime * 10f
                );
            }
        }
    }
    
    /// <summary>
    /// Tăng/giảm mức độ căng thẳng (0-1)
    /// </summary>
    public void SetStressLevel(float level)
    {
        stressLevel = Mathf.Clamp01(level);
    }
    
    /// <summary>
    /// Thêm căng thẳng
    /// </summary>
    public void AddStress(float amount)
    {
        stressLevel = Mathf.Clamp01(stressLevel + amount);
    }
    
    /// <summary>
    /// Giảm căng thẳng
    /// </summary>
    public void ReduceStress(float amount)
    {
        stressLevel = Mathf.Clamp01(stressLevel - amount);
    }
    
    /// <summary>
    /// Lấy mức độ căng thẳng hiện tại
    /// </summary>
    public float GetStressLevel()
    {
        return stressLevel;
    }
    
    /// <summary>
    /// Kiểm tra nhân vật có đang di chuyển không
    /// </summary>
    public bool IsMoving()
    {
        return isMoving;
    }
    
    /// <summary>
    /// Lấy tốc độ hiện tại
    /// </summary>
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
    void OnDrawGizmosSelected()
    {
        // Vẽ sphere để debug ground check
        if (characterController != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                transform.position + Vector3.down * (characterController.height / 2 - characterController.radius),
                groundCheckDistance
            );
        }
    }
}










