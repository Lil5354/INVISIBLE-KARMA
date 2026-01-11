using UnityEngine;

public class MouseLookUltimate : MonoBehaviour
{
    [Header("Cài đặt")]
    public float mouseSensitivity = 100f; // Tốc độ chuột
    
    [Header("Quan trọng: Kéo Player (Cha) vào đây")]
    public Transform playerBody; // Nếu để trống, Camera sẽ tự quay 360 độ tại chỗ

    private float xRotation = 0f; // Góc quay lên/xuống
    private float yRotation = 0f; // Góc quay trái/phải (Dùng khi không có body)

    void Start()
    {
        // Khóa chuột vào giữa màn hình
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Lấy tín hiệu chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // --- XỬ LÝ QUAY LÊN / XUỐNG (Trục X) ---
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Giới hạn không gãy cổ

        // --- XỬ LÝ QUAY TRÁI / PHẢI (Trục Y) ---
        if (playerBody != null)
        {
            // CÁCH 1: CHUẨN FPS (Có thân người)
            // Camera chỉ nhìn lên xuống
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            // Cả thân người quay trái phải
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else
        {
            // CÁCH 2: FREELOOK (Quay tự do 360 độ - Camera giám sát)
            // Nếu bạn chưa kéo PlayerBody vào, code này sẽ chạy
            yRotation += mouseX;
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}
