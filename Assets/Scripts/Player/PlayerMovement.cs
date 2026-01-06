using UnityEngine;

/// <summary>
/// Script điều khiển di chuyển nhân vật bằng W, A, S, D
/// Sử dụng Character Controller để không đi xuyên tường
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Tham chiếu")]
    [Tooltip("Tham chiếu đến Character Controller - Để trống sẽ tự động tìm")]
    public CharacterController controller; // Tham chiếu đến "đôi chân"
    
    [Header("Cài đặt Di chuyển")]
    [Tooltip("Tốc độ đi bộ (đơn vị/giây)")]
    public float tocDoDiChuyen = 12f;      // Tốc độ đi bộ
    
    [Tooltip("Trọng lực (để nhân vật rơi xuống đất)")]
    public float trongLuc = -9.81f;        // Trọng lực (để nhân vật rơi xuống đất)
    
    [Header("Tùy chọn")]
    [Tooltip("Kiểm tra xem nhân vật có đang chạm đất không")]
    public bool checkGrounded = true;
    
    [Tooltip("Khoảng cách kiểm tra chạm đất (Ground Check)")]
    public float groundDistance = 0.4f;
    
    [Tooltip("Layer của mặt đất (Ground Layer)")]
    public LayerMask groundMask;

    Vector3 velocity; // Biến lưu vận tốc rơi
    bool isGrounded; // Kiểm tra có đang chạm đất không

    void Start()
    {
        // Tự động tìm Character Controller nếu chưa gán
        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
            if (controller != null)
            {
                Debug.Log($"[PlayerMovement] ✅ Đã tự động tìm thấy Character Controller trên: {gameObject.name}");
            }
            else
            {
                Debug.LogError($"[PlayerMovement] ❌ KHÔNG TÌM THẤY Character Controller!");
                Debug.LogError($"[PlayerMovement] ❌ Hãy Add Component -> Character Controller vào {gameObject.name}");
            }
        }
        
        // Kiểm tra Ground Layer
        if (groundMask == 0)
        {
            Debug.LogWarning("[PlayerMovement] ⚠️ Ground Mask chưa được cấu hình. Nhân vật có thể không phát hiện được mặt đất.");
        }
    }

    void Update()
    {
        // Kiểm tra Character Controller
        if (controller == null)
        {
            return;
        }
        
        // Kiểm tra có đang chạm đất không (nếu bật checkGrounded)
        if (checkGrounded)
        {
            isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);
            
            // Reset velocity khi chạm đất
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Giữ một chút velocity để đảm bảo luôn chạm đất
            }
        }
        else
        {
            // Nếu không check grounded, luôn coi như đang chạm đất
            isGrounded = controller.isGrounded;
        }

        // 1. Lấy tín hiệu từ bàn phím (W, A, S, D)
        // x là trái/phải (A/D), z là tới/lùi (W/S)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 2. Tính toán hướng di chuyển theo hướng nhân vật đang nhìn
        // transform.right là hướng bên phải, transform.forward là hướng phía trước
        Vector3 move = transform.right * x + transform.forward * z;
        
        // Chuẩn hóa vector để tốc độ không đổi khi đi chéo
        move = move.normalized;

        // 3. Ra lệnh cho CharacterController di chuyển
        controller.Move(move * tocDoDiChuyen * Time.deltaTime);

        // 4. Xử lý trọng lực (Rơi xuống)
        velocity.y += trongLuc * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    /// <summary>
    /// Vẽ Gizmo để hiển thị Ground Check trong Scene View
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (checkGrounded)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position, groundDistance);
        }
    }
}
