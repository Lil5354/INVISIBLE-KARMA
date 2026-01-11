using UnityEngine;

/// <summary>
/// Script tương tác của Player cho Scene 2 (FPS)
/// Bắn tia từ tâm màn hình để tương tác với vật thể
/// Gắn script này vào Main Camera
/// </summary>
public class PlayerInteraction2 : MonoBehaviour
{
    [Header("Cấu hình")]
    public float tamTuongTac = 5f; // Khoảng cách có thể với tới (mét)
    public LayerMask lopTuongTac;  // Chọn layer để tối ưu (hoặc để Everything)

    void Update()
    {
        // Chỉ bấm được khi chuột ĐANG KHÓA (đang ở chế độ FPS)
        // Nếu chuột đang hiện ra (để bấm UI) thì không cho bắn tia lung tung
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.Locked)
        {
            XuLyTuongTac();
        }
    }

    void XuLyTuongTac()
    {
        // 1. Bắn tia từ CHÍNH GIỮA MÀN HÌNH (Tọa độ Viewport 0.5, 0.5)
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // 2. Kiểm tra va chạm
        if (Physics.Raycast(ray, out hit, tamTuongTac, lopTuongTac))
        {
            // --- TRƯỜNG HỢP A: NHẶT ĐỒ (CollectableItem) ---
            // Tìm xem vật bắn trúng có script CollectableItem không
            CollectableItem vatPham = hit.collider.GetComponent<CollectableItem>();
            if (vatPham != null)
            {
                Debug.Log("Đã nhặt: " + hit.collider.name);
                vatPham.NhatCaiNay(); // Gọi hàm xử lý nhặt
                return; // Xong việc rồi thì thoát, không check tiếp
            }

            // --- TRƯỜNG HỢP B: HÌNH NHÂN (ClickToZoom) ---
            // Tìm xem vật bắn trúng có script ClickToZoom không
            ClickToZoom hinhNhan = hit.collider.GetComponent<ClickToZoom>();
            if (hinhNhan != null)
            {
                Debug.Log("Đã bấm vào hình nhân!");
                hinhNhan.KichHoatZoom(); // Gọi hàm xử lý zoom
                return;
            }
        }
    }
}

