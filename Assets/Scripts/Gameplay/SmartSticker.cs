using UnityEngine;

/// <summary>
/// Script "Tắc Kè Hoa" - Quản lý việc thay đổi Material cho 1 Quad duy nhất
/// Khi chọn đồ khác nhau, Quad sẽ tự động đổi Material tương ứng
/// </summary>
public class SmartSticker : MonoBehaviour
{
    [Header("Cài đặt chung")]
    public Camera mainCam;
    public LayerMask layerConRoi; // Layer của con rối
    public GameObject ghostQuad;  // Kéo cái Quad duy nhất vào đây
    public MeshRenderer ghostRenderer; // Kéo chính cái Quad vào đây (nó có component MeshRenderer)

    [Header("Kho Đồ (Kéo 4 Material vào đây theo thứ tự)")]
    [Tooltip("0: Áo Trước, 1: Áo Sau, 2: Váy, 3: Khăn")]
    public Material[] danhSachHinhAnh; 

    [Header("Cài đặt logic")]
    public float khoangCachHo = 0.01f;

    private int idDangCam = -1; // -1 là chưa cầm gì
    private bool dangKeo = false;

    void Start()
    {
        // Tự động tìm Camera nếu chưa gán
        if (mainCam == null)
        {
            mainCam = Camera.main;
            if (mainCam == null)
            {
                mainCam = FindObjectOfType<Camera>();
            }
        }

        // Tự động lấy MeshRenderer từ ghostQuad nếu chưa gán
        if (ghostRenderer == null && ghostQuad != null)
        {
            ghostRenderer = ghostQuad.GetComponent<MeshRenderer>();
        }

        // Ẩn Quad lúc đầu
        if (ghostQuad != null)
        {
            ghostQuad.SetActive(false);
        }
    }

    /// <summary>
    /// Hàm này gọi từ UI (khi bấm vào icon trong túi đồ)
    /// </summary>
    /// <param name="idMonDo">0: Áo Trước, 1: Áo Sau, 2: Váy, 3: Khăn</param>
    public void BatDauCamDo(int idMonDo)
    {
        // 1. Lưu lại ID món đồ đang cầm
        idDangCam = idMonDo;

        // 2. Thay áo cho cái Quad (QUAN TRỌNG NHẤT)
        if (idMonDo >= 0 && idMonDo < danhSachHinhAnh.Length && danhSachHinhAnh[idMonDo] != null)
        {
            if (ghostRenderer != null)
            {
                ghostRenderer.material = danhSachHinhAnh[idMonDo];
                Debug.Log($"[SmartSticker] Đã đổi Material sang món đồ ID: {idMonDo}");
            }
            else
            {
                Debug.LogError("[SmartSticker] ghostRenderer chưa được gán!");
            }
        }
        else
        {
            Debug.LogWarning($"[SmartSticker] ID món đồ không hợp lệ hoặc Material chưa được gán: {idMonDo}");
            return;
        }

        // 3. Hiện cái Quad lên và bắt đầu dính chuột
        if (ghostQuad != null)
        {
            ghostQuad.SetActive(true);
            dangKeo = true;
        }
        else
        {
            Debug.LogError("[SmartSticker] ghostQuad chưa được gán!");
        }
    }

    void Update()
    {
        if (dangKeo)
        {
            XuLyTruotTrenDa();

            if (Input.GetMouseButtonDown(0)) // Bấm chuột để dán
            {
                ThuDanGiay();
            }

            // Nhấn ESC hoặc chuột phải để hủy
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                ketThucKeo();
            }
        }
    }

    /// <summary>
    /// Xử lý việc trượt Quad trên da con rối
    /// </summary>
    void XuLyTruotTrenDa()
    {
        if (mainCam == null || ghostQuad == null)
        {
            return;
        }

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, layerConRoi))
        {
            // Dính Quad vào bề mặt da
            ghostQuad.transform.position = hit.point + (hit.normal * khoangCachHo);
            ghostQuad.transform.rotation = Quaternion.LookRotation(-hit.normal);
        }
    }

    /// <summary>
    /// Xử lý việc dán giấy (khi click chuột)
    /// </summary>
    void ThuDanGiay()
    {
        if (mainCam == null)
        {
            return;
        }

        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, layerConRoi))
        {
            string tagDung = "";
            
            // Quy định luật chơi (ID nào tương ứng Tag nào)
            switch (idDangCam)
            {
                case 0: tagDung = "Zone_NgucTruoc"; break;
                case 1: tagDung = "Zone_NgucSau"; break;
                case 2: tagDung = "Zone_Chan"; break; // Váy
                case 3: tagDung = "Zone_Dau"; break;  // Khăn
                default:
                    Debug.LogWarning($"[SmartSticker] ID món đồ không hợp lệ: {idDangCam}");
                    return;
            }

            // Kiểm tra Tag
            if (hit.collider.CompareTag(tagDung))
            {
                Debug.Log($"[SmartSticker] ✅ Dán đúng món số: {idDangCam} vào vùng: {tagDung}");
                
                // GỌI HÀM GHI NHẬN BÊN QUẢN LÝ (Trọng tài)
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.GhiNhanDanDung(idDangCam);
                }
                else
                {
                    Debug.LogError("[SmartSticker] InventoryManager.Instance không tồn tại! Hãy tạo GameObject có script InventoryManager.");
                }
                
                // Trigger event để các script khác có thể lắng nghe
                OnStickerPlacedCorrectly?.Invoke(idDangCam, tagDung);
                
                // Dán xong thì tắt chế độ cầm đồ (ẩn Quad đi)
                ketThucKeo();
            }
            else
            {
                Debug.Log($"[SmartSticker] ❌ Sai vị trí rồi! Cần dán vào: {tagDung}, nhưng click vào: {hit.collider.tag}");
                // Có thể thêm âm thanh "Sai rồi"
                OnStickerPlacedIncorrectly?.Invoke(idDangCam, hit.collider.tag);
            }
        }
    }

    /// <summary>
    /// Kết thúc kéo và ẩn Quad
    /// </summary>
    void ketThucKeo()
    {
        dangKeo = false;
        if (ghostQuad != null)
        {
            ghostQuad.SetActive(false); // Ẩn cái Quad đi chờ lần sau dùng tiếp
        }
        idDangCam = -1;
        Debug.Log("[SmartSticker] Đã kết thúc kéo");
    }

    // Events để các script khác có thể lắng nghe
    public delegate void StickerPlacedEvent(int itemID, string zoneTag);
    public static event StickerPlacedEvent OnStickerPlacedCorrectly;
    public static event StickerPlacedEvent OnStickerPlacedIncorrectly;
}

