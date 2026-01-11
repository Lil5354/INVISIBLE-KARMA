using UnityEngine;

/// <summary>
/// Script cho vật phẩm có thể nhặt được trên sàn
/// Khi người chơi click vào, vật phẩm sẽ được thêm vào túi đồ
/// </summary>
public class CollectableItem : MonoBehaviour
{
    [Header("ID của món đồ này")]
    [Tooltip("0: Áo Trước, 1: Áo Sau, 2: Váy, 3: Khăn")]
    public int idMonDo;

    [Header("Cài đặt tương tác")]
    [Tooltip("Khoảng cách tối đa để có thể nhặt (mét)")]
    public float khoangCachToiDa = 10f;

    [Header("Hiệu ứng (Tùy chọn)")]
    public GameObject hieuUngNhat; // Particle effect khi nhặt
    public AudioClip amThanhNhat;  // Âm thanh khi nhặt

    private bool daNhat = false; // Tránh nhặt 2 lần

    void Start()
    {
        // Đảm bảo vật phẩm có Collider để OnMouseDown hoạt động
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"[CollectableItem] GameObject '{gameObject.name}' không có Collider! OnMouseDown sẽ không hoạt động.");
        }
    }

    /// <summary>
    /// Hàm này sẽ được gọi bởi PlayerInteraction2 khi bắn tia trúng (Scene 2 - FPS)
    /// </summary>
    public void NhatCaiNay()
    {
        if (daNhat)
        {
            return; // Đã nhặt rồi, không cho nhặt lại
        }

        // Kiểm tra InventoryManager có tồn tại không
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[CollectableItem] InventoryManager.Instance không tồn tại! Hãy tạo GameObject có script InventoryManager.");
            return;
        }

        // Gọi Manager để mở khóa nút bấm tương ứng
        InventoryManager.Instance.NhatDo(idMonDo);

        // Phát hiệu ứng nếu có
        PhatHieuUng();

        // Đánh dấu đã nhặt
        daNhat = true;

        // Biến mất vật thể trên sàn
        Destroy(gameObject, 0.1f); // Delay nhỏ để hiệu ứng kịp chạy
    }

    /// <summary>
    /// Hàm này sẽ được gọi bởi PlayerInteraction khi bắn tia trúng (Scene 1 - Point & Click)
    /// </summary>
    public void BiNhat()
    {
        if (daNhat)
        {
            return; // Đã nhặt rồi, không cho nhặt lại
        }

        // Kiểm tra khoảng cách (để không bấm xuyên tường từ xa)
        if (Camera.main == null)
        {
            Debug.LogError("[CollectableItem] Không tìm thấy Camera.main!");
            return;
        }

        float dist = Vector3.Distance(transform.position, Camera.main.transform.position);
        if (dist > khoangCachToiDa)
        {
            Debug.Log($"[CollectableItem] Quá xa để nhặt! Khoảng cách: {dist:F2}m (Tối đa: {khoangCachToiDa}m)");
            return;
        }

        // Kiểm tra InventoryManager có tồn tại không
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[CollectableItem] InventoryManager.Instance không tồn tại! Hãy tạo GameObject có script InventoryManager.");
            return;
        }

        // Gọi Manager để mở khóa nút bấm tương ứng
        InventoryManager.Instance.NhatDo(idMonDo);

        // Phát hiệu ứng nếu có
        PhatHieuUng();

        // Đánh dấu đã nhặt
        daNhat = true;

        // Biến mất vật thể trên sàn
        Destroy(gameObject, 0.1f); // Delay nhỏ để hiệu ứng kịp chạy
    }

    /// <summary>
    /// Xử lý khi người chơi click vào vật phẩm (Giữ lại để tương thích ngược)
    /// </summary>
    void OnMouseDown()
    {
        BiNhat();
    }

    /// <summary>
    /// Phát hiệu ứng khi nhặt vật phẩm
    /// </summary>
    void PhatHieuUng()
    {
        // Phát particle effect
        if (hieuUngNhat != null)
        {
            GameObject effect = Instantiate(hieuUngNhat, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // Tự hủy sau 2 giây
        }

        // Phát âm thanh
        if (amThanhNhat != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(amThanhNhat, Camera.main.transform.position);
        }
    }

    /// <summary>
    /// Vẽ gizmo để debug khoảng cách trong Scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, khoangCachToiDa);
    }
}

