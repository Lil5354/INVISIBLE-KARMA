using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Cài đặt Di chuyển (Nặng nề)")]
    public float walkingSpeed = 2.5f; // Giảm xuống thấp để tạo cảm giác lê bước
    public float runningSpeed = 4.5f; // Chạy cũng không nhanh lắm
    public float jumpSpeed = 3.0f;    // Nhảy thấp thôi
    public float gravity = 20.0f;

    [Header("Cài đặt Camera & Nhìn")]
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f; // Giới hạn góc nhìn hẹp hơn chút cho tù túng

    [Header("Hiệu ứng Thở (Head Bob)")]
    public bool enableHeadBob = true;
    public float breathingSpeed = 2.0f;  // Tốc độ thở (càng cao thở càng gấp)
    public float breathingAmount = 0.05f; // Độ rung (càng cao đầu gật càng mạnh)
    
    private float defaultPosY = 0;       // Vị trí Camera ban đầu
    private float timer = 0;             // Bộ đếm thời gian cho nhịp thở

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Khóa chuột
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Lưu lại vị trí độ cao ban đầu của Camera (thường là 0.6 hoặc 0.8)
        if (playerCamera != null)
        {
            defaultPosY = playerCamera.transform.localPosition.y;
        }
    }

    void Update()
    {
        // --- 1. XỬ LÝ DI CHUYỂN ---
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        
        // Tính toán tốc độ
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Xử lý nhảy (nếu cần)
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Trọng lực
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            // Reset velocity khi chạm đất
            if (movementDirectionY < 0)
            {
                moveDirection.y = -2f;
            }
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // --- 2. XỬ LÝ CAMERA & NHÌN ---
        if (canMove && playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        // --- 3. HIỆU ỨNG THỞ (HEAD BOB) KHI ĐỨNG YÊN ---
        if (enableHeadBob && playerCamera != null)
        {
            HandleBreathingEffect();
        }
    }

    void HandleBreathingEffect()
    {
        // Kiểm tra xem nhân vật có đang đứng yên không (tốc độ gần bằng 0)
        if (Mathf.Abs(characterController.velocity.x) < 0.1f && Mathf.Abs(characterController.velocity.z) < 0.1f && characterController.isGrounded)
        {
            // Tăng bộ đếm thời gian
            timer += Time.deltaTime * breathingSpeed;
            
            // Tính toán vị trí Y mới bằng sóng Sin (lên xuống nhịp nhàng)
            float newY = defaultPosY + Mathf.Sin(timer) * breathingAmount;
            
            // Gán vị trí mới cho Camera
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                newY,
                playerCamera.transform.localPosition.z
            );
        }
        else
        {
            // Khi di chuyển, reset timer để nhịp thở không bị loạn
            timer = 0;
            
            // Trả camera về vị trí gốc từ từ (Lerp) cho mượt
            Vector3 targetPos = new Vector3(playerCamera.transform.localPosition.x, defaultPosY, playerCamera.transform.localPosition.z);
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPos, Time.deltaTime * 5f);
        }
    }
}
