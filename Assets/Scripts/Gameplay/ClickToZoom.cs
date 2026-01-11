using UnityEngine;

/// <summary>
/// Script để click vào hình nhân gỗ và chuyển sang chế độ giải đố
/// Gắn script này vào GameObject Hình Nhân Gỗ 3D
/// </summary>
public class ClickToZoom : MonoBehaviour
{
    [Header("Cài đặt tương tác")]
    [Tooltip("Khoảng cách tối đa để có thể click (mét)")]
    public float khoangCachToiDa = 5f;

    [Tooltip("Cần click chuột trái để zoom (false = tự động khi đến gần)")]
    public bool canClick = true;

    [Header("Hiệu ứng (Tùy chọn)")]
    public GameObject hieuUngClick;
    public AudioClip amThanhClick;

    [Header("Cài đặt Auto Trigger")]
    [Tooltip("Tự động trigger khi đến gần (thay vì click)")]
    public bool tuDongTrigger = false;

    [Tooltip("Bán kính tự động trigger (mét) - Chỉ dùng khi tuDongTrigger = true")]
    public float banKinhAutoTrigger = 2f;

    private bool daKichHoat = false; // Tránh trigger nhiều lần
    private Transform playerTransform;

    void Start()
    {
        // Tìm Player transform để kiểm tra khoảng cách
        if (tuDongTrigger)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                // Thử tìm PlayerController hoặc các component khác có thể là Player
                var playerController = FindObjectOfType<PlayerController>();
                if (playerController != null)
                {
                    playerTransform = playerController.transform;
                }
                else
                {
                    // Tìm bất kỳ GameObject nào có tên chứa "Player"
                    GameObject foundPlayer = GameObject.Find("Player");
                    if (foundPlayer == null)
                    {
                        foundPlayer = GameObject.Find("PlayerController");
                    }
                    if (foundPlayer != null)
                    {
                        playerTransform = foundPlayer.transform;
                    }
                }
            }

            if (playerTransform == null)
            {
                Debug.LogWarning("[ClickToZoom] Không tìm thấy Player! Auto trigger sẽ không hoạt động. Hãy gán playerTransform thủ công trong Inspector.");
            }
        }

        // Đảm bảo có Collider để OnMouseDown hoạt động (nếu dùng click)
        if (canClick && GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"[ClickToZoom] GameObject '{gameObject.name}' không có Collider! OnMouseDown sẽ không hoạt động. Hãy thêm Collider vào.");
        }
    }

    void Update()
    {
        // Nếu dùng auto trigger, kiểm tra khoảng cách mỗi frame
        if (tuDongTrigger && !daKichHoat && playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= banKinhAutoTrigger)
            {
                KichHoatZoom();
            }
        }
    }

    /// <summary>
    /// Xử lý khi người chơi click vào hình nhân gỗ
    /// </summary>
    void OnMouseDown()
    {
        if (!canClick || daKichHoat)
        {
            return;
        }

        // Kiểm tra khoảng cách
        if (Camera.main == null)
        {
            Debug.LogError("[ClickToZoom] Không tìm thấy Camera.main!");
            return;
        }

        float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
        if (dist > khoangCachToiDa)
        {
            Debug.Log($"[ClickToZoom] Quá xa để click! Khoảng cách: {dist:F2}m (Tối đa: {khoangCachToiDa}m)");
            return;
        }

        KichHoatZoom();
    }

    /// <summary>
    /// Hàm này sẽ được gọi bởi PlayerInteraction khi bắn tia trúng
    /// </summary>
    public void TuongTac()
    {
        Debug.Log("Đã bắn tia trúng hình nhân!"); // Dòng này để kiểm tra
        KichHoatZoom();
    }

    /// <summary>
    /// Kích hoạt zoom vào hình nhân (Public để PlayerInteraction2 có thể gọi)
    /// </summary>
    public void KichHoatZoom()
    {
        if (daKichHoat)
        {
            return;
        }

        // Kiểm tra InventoryManager
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[ClickToZoom] InventoryManager.Instance không tồn tại! Hãy tạo GameObject có script InventoryManager.");
            return;
        }

        // Phát hiệu ứng nếu có
        PhatHieuUng();

        // Gọi InventoryManager để kiểm tra và zoom
        InventoryManager.Instance.KiemTraVaZoom();

        // Đánh dấu đã kích hoạt (nếu muốn chỉ cho phép 1 lần)
        // daKichHoat = true; // Uncomment nếu muốn chỉ cho phép click 1 lần
    }

    /// <summary>
    /// Phát hiệu ứng khi click/trigger
    /// </summary>
    void PhatHieuUng()
    {
        // Phát particle effect
        if (hieuUngClick != null)
        {
            GameObject effect = Instantiate(hieuUngClick, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Phát âm thanh
        if (amThanhClick != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(amThanhClick, Camera.main.transform.position);
        }
    }

    /// <summary>
    /// Reset để cho phép click lại (Dùng khi restart level)
    /// </summary>
    public void Reset()
    {
        daKichHoat = false;
    }

    /// <summary>
    /// Vẽ gizmo để debug khoảng cách trong Scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Vẽ khoảng cách click
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, khoangCachToiDa);

        // Vẽ bán kính auto trigger (nếu bật)
        if (tuDongTrigger)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, banKinhAutoTrigger);
        }
    }
}

