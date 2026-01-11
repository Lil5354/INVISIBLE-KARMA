using UnityEngine;

/// <summary>
/// Script điều khiển Camera bằng chuột (Mouse Look)
/// Gắn vào Camera để xoay đầu nhìn xung quanh
/// </summary>
public class MouseLook : MonoBehaviour
{
    [Header("Cài đặt")]
    [Tooltip("Tốc độ xoay (độ nhạy chuột) - Số càng lớn càng nhạy")]
    public float tocDoChuot = 80f; // Tốc độ xoay (độ nhạy chuột) - Giảm xuống để mượt hơn
    
    [Tooltip("Kéo nhân vật (Player object) vào đây để xoay người theo")]
    public Transform coTheNhanVat;  // Kéo nhân vật vào đây để xoay người theo

    [Header("Tùy chọn")]
    [Tooltip("Giới hạn góc nhìn lên/xuống (độ) - FPS thực tế thường 60-70 độ")]
    public float lookXLimit = 60f; // Giảm từ 90 xuống 60 để giống FPS thực tế
    
    [Tooltip("Làm mượt chuyển động camera (Smoothing) - Số càng lớn càng mượt")]
    public float smoothing = 10f; // Làm mượt chuyển động
    
    [Tooltip("Tự động khóa cursor khi Start")]
    public bool lockCursorOnStart = true;

    float xRotation = 0f; // Biến lưu góc xoay lên/xuống
    float currentXRotation = 0f; // Góc xoay hiện tại (để smooth)

    void Start()
    {
        // Giấu con trỏ chuột đi và khóa nó vào giữa màn hình
        if (lockCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("[MouseLook] ✅ Cursor đã được khóa và ẩn");
        }
        
        // Tự động tìm Player nếu chưa gán
        if (coTheNhanVat == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                coTheNhanVat = player.transform;
                Debug.Log($"[MouseLook] ✅ Đã tự động tìm thấy Player: {player.name}");
            }
            else
            {
                Debug.LogWarning("[MouseLook] ⚠️ Không tìm thấy Player! Hãy kéo Player vào field 'Co The Nhan Vat'.");
            }
        }
    }

    void Update()
    {
        // Chỉ xử lý khi cursor bị khóa (đang chơi game)
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }
        
        // Lấy thông tin di chuyển của chuột
        float mouseX = Input.GetAxis("Mouse X") * tocDoChuot * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * tocDoChuot * Time.deltaTime;

        // Xử lý xoay lên/xuống (trục X)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -lookXLimit, lookXLimit); // Giới hạn góc nhìn (60 độ như FPS thực tế)

        // Smooth camera rotation để mượt hơn
        currentXRotation = Mathf.Lerp(currentXRotation, xRotation, Time.deltaTime * smoothing);
        
        // Xoay Camera lên xuống (với smoothing)
        transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);

        // Xoay toàn bộ cơ thể nhân vật sang trái/phải (mượt hơn)
        if (coTheNhanVat != null)
        {
            coTheNhanVat.Rotate(Vector3.up * mouseX);
        }
        
        // Toggle cursor lock với phím ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Debug.Log("[MouseLook] Cursor đã được mở khóa (ESC)");
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Debug.Log("[MouseLook] Cursor đã được khóa lại");
            }
        }
    }
}

