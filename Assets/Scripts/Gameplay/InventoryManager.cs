using UnityEngine;
using UnityEngine.UI; // Để dùng Button
using UnityEngine.Video; // Để dùng Video

/// <summary>
/// Script quản lý túi đồ và chuyển đổi giữa chế độ đi lại và chế độ giải đố
/// Quản lý việc hiện/ẩn nút bấm trong túi đồ và kiểm tra điều kiện để zoom vào hình nhân
/// </summary>
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Danh sách 4 Nút trong Canvas")]
    [Tooltip("Kéo 4 cái nút (đang bị ẩn) vào đây theo đúng thứ tự ID 0->3")]
    public GameObject[] nutBamUI;

    [Header("Cấu hình Gameplay")]
    [Tooltip("Nhân vật đi lại (FPS Controller)")]
    public GameObject playerFPS;

    [Tooltip("Camera soi hình nhân (Camera giải đố)")]
    public GameObject puzzleCamera;

    [Tooltip("Cái Panel chứa túi đồ (để bật tắt nếu cần)")]
    public GameObject uiInventory;

    [Header("Cài đặt Debug")]
    [Tooltip("Hiển thị thông báo khi chưa đủ đồ")]
    public bool hienThongBaoKhiThieuDo = true;

    [Tooltip("Text UI để hiển thị thông báo (Tùy chọn)")]
    public Text textThongBao;

    [Header("Chiến Thắng")]
    [Tooltip("Cái RawImage hoặc GameObject chứa màn hình chiếu phim")]
    public GameObject manHinhVideo;

    [Tooltip("Trình phát video (VideoPlayer component)")]
    public VideoPlayer videoPlayer;

    [Tooltip("Âm thanh khi thắng (Tùy chọn)")]
    public AudioClip amThanhChienThang;

    // Biến đếm số lượng
    private int soDoDaNhat = 0; // Để mở khóa Zoom (đã nhặt được bao nhiêu món)
    private int soDoDaDanDung = 0; // Để tính thắng game (đã dán đúng bao nhiêu món)
    private bool dangOCheDoGhepDo = false;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("[InventoryManager] Đã có Instance khác! Đang xóa duplicate...");
            Destroy(gameObject);
            return;
        }

        // Đảm bảo lúc đầu tắt camera soi đi
        if (puzzleCamera != null)
        {
            puzzleCamera.SetActive(false);
        }

        // Đảm bảo các nút UI ban đầu bị ẩn
        AnTatCaNutBam();

        // Đảm bảo màn hình video bị tắt lúc đầu
        if (manHinhVideo != null)
        {
            manHinhVideo.SetActive(false);
        }

        // Đảm bảo video player được setup
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
        }
    }

    void Start()
    {
        // Đảm bảo player FPS được bật lúc đầu
        if (playerFPS != null && !playerFPS.activeSelf)
        {
            playerFPS.SetActive(true);
        }

        // Hiển thị túi đồ nếu cần
        if (uiInventory != null && !uiInventory.activeSelf)
        {
            uiInventory.SetActive(true);
        }
    }

    /// <summary>
    /// Ẩn tất cả nút bấm khi bắt đầu
    /// </summary>
    void AnTatCaNutBam()
    {
        if (nutBamUI != null)
        {
            for (int i = 0; i < nutBamUI.Length; i++)
            {
                if (nutBamUI[i] != null)
                {
                    nutBamUI[i].SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Hàm này được gọi từ script CollectableItem khi người chơi nhặt đồ
    /// </summary>
    /// <param name="id">ID của món đồ (0: Áo Trước, 1: Áo Sau, 2: Váy, 3: Khăn)</param>
    public void NhatDo(int id)
    {
        // 1. Kiểm tra ID hợp lệ
        if (id < 0 || id >= nutBamUI.Length)
        {
            Debug.LogError($"[InventoryManager] ID món đồ không hợp lệ: {id} (Phải từ 0 đến {nutBamUI.Length - 1})");
            return;
        }

        // 2. Kiểm tra nút đã được kích hoạt chưa (tránh nhặt trùng)
        if (nutBamUI[id] != null && nutBamUI[id].activeSelf)
        {
            Debug.LogWarning($"[InventoryManager] Món đồ ID {id} đã được nhặt rồi!");
            return;
        }

        // 3. Hiện nút bấm tương ứng lên
        if (nutBamUI[id] != null)
        {
            nutBamUI[id].SetActive(true);
            Debug.Log($"[InventoryManager] ✅ Đã nhặt món số: {id} - Nút UI đã được hiện!");
        }
        else
        {
            Debug.LogError($"[InventoryManager] Nút bấm ID {id} chưa được gán trong Inspector!");
            return;
        }

        // 4. Tăng đếm số lượng
        soDoDaNhat++;
        Debug.Log($"[InventoryManager] Tổng số đồ đã nhặt: {soDoDaNhat}/4");
    }

    /// <summary>
    /// Hàm này gọi khi bấm vào Hình Nhân Gỗ 3D
    /// Kiểm tra xem đã nhặt đủ đồ để cho phép Zoom vào hình nhân chưa
    /// </summary>
    public void KiemTraVaZoom()
    {
        if (dangOCheDoGhepDo)
        {
            Debug.Log("[InventoryManager] Đang ở chế độ giải đố rồi!");
            return;
        }

        if (soDoDaNhat >= 4)
        {
            VaoCheDoGhepDo();
        }
        else
        {
            int soDoConThieu = 4 - soDoDaNhat;
            string thongBao = $"Chưa đủ đồ! Cần tìm thêm {soDoConThieu} món.";
            Debug.Log($"[InventoryManager] ❌ {thongBao}");

            if (hienThongBaoKhiThieuDo)
            {
                HienThongBao(thongBao, 2f); // Hiện thông báo 2 giây
            }
        }
    }

    /// <summary>
    /// Chuyển sang chế độ giải đố (Zoom vào hình nhân)
    /// </summary>
    void VaoCheDoGhepDo()
    {
        Debug.Log("[InventoryManager] 🎯 Vào chế độ giải đố!");

        // Tắt người chơi
        if (playerFPS != null)
        {
            playerFPS.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[InventoryManager] playerFPS chưa được gán!");
        }

        // Bật camera soi
        if (puzzleCamera != null)
        {
            puzzleCamera.SetActive(true);
        }
        else
        {
            Debug.LogError("[InventoryManager] puzzleCamera chưa được gán!");
        }

        // Hiện con trỏ chuột để thao tác
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Đánh dấu đang ở chế độ giải đố
        dangOCheDoGhepDo = true;

        Debug.Log("[InventoryManager] ✅ Đã chuyển sang chế độ giải đố!");
    }

    /// <summary>
    /// Thoát khỏi chế độ giải đố (Quay lại chế độ đi lại)
    /// </summary>
    public void ThoatCheDoGhepDo()
    {
        if (!dangOCheDoGhepDo)
        {
            return;
        }

        Debug.Log("[InventoryManager] Thoát chế độ giải đố!");

        // Bật lại người chơi
        if (playerFPS != null)
        {
            playerFPS.SetActive(true);
        }

        // Tắt camera soi
        if (puzzleCamera != null)
        {
            puzzleCamera.SetActive(false);
        }

        // Ẩn con trỏ chuột
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Đánh dấu không còn ở chế độ giải đố
        dangOCheDoGhepDo = false;
    }

    /// <summary>
    /// Hiển thị thông báo trên màn hình
    /// </summary>
    void HienThongBao(string noiDung, float thoiGian)
    {
        if (textThongBao != null)
        {
            textThongBao.text = noiDung;
            textThongBao.gameObject.SetActive(true);
            Invoke(nameof(AnThongBao), thoiGian);
        }
        else
        {
            // Nếu không có Text UI, chỉ log ra console
            Debug.Log($"[THÔNG BÁO] {noiDung}");
        }
    }

    /// <summary>
    /// Ẩn thông báo
    /// </summary>
    void AnThongBao()
    {
        if (textThongBao != null)
        {
            textThongBao.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Kiểm tra xem có đang ở chế độ giải đố không
    /// </summary>
    public bool IsInPuzzleMode()
    {
        return dangOCheDoGhepDo;
    }

    /// <summary>
    /// Lấy số lượng đồ đã nhặt
    /// </summary>
    public int GetSoDoDaNhat()
    {
        return soDoDaNhat;
    }

    /// <summary>
    /// Reset túi đồ (Dùng cho khi restart level)
    /// </summary>
    public void ResetInventory()
    {
        soDoDaNhat = 0;
        soDoDaDanDung = 0;
        AnTatCaNutBam();
        dangOCheDoGhepDo = false;

        // Tắt màn hình video nếu đang bật
        if (manHinhVideo != null)
        {
            manHinhVideo.SetActive(false);
        }

        // Dừng video nếu đang phát
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }

        Debug.Log("[InventoryManager] Đã reset túi đồ!");
    }

    /// <summary>
    /// Hàm này được gọi từ SmartSticker khi người chơi dán đúng một món đồ
    /// </summary>
    /// <param name="idMonDo">ID của món đồ đã dán đúng (0: Áo Trước, 1: Áo Sau, 2: Váy, 3: Khăn)</param>
    public void GhiNhanDanDung(int idMonDo)
    {
        // 1. Kiểm tra ID hợp lệ
        if (idMonDo < 0 || idMonDo >= nutBamUI.Length)
        {
            Debug.LogError($"[InventoryManager] ID món đồ không hợp lệ: {idMonDo}");
            return;
        }

        // 2. Tăng điểm
        soDoDaDanDung++;
        Debug.Log($"[InventoryManager] ✅ Đã dán đúng món số: {idMonDo} - Tổng: {soDoDaDanDung}/4");

        // 3. Tắt cái nút UI của món đó đi (Dán xong rồi thì cất đi khỏi túi đồ)
        if (nutBamUI[idMonDo] != null)
        {
            nutBamUI[idMonDo].SetActive(false);
            Debug.Log($"[InventoryManager] Đã ẩn nút UI của món ID: {idMonDo}");
        }

        // 4. Kiểm tra thắng (Đủ 4 cái)
        if (soDoDaDanDung >= 4)
        {
            Debug.Log("[InventoryManager] 🎉 ĐÃ DÁN ĐỦ 4 MÓN! THẮNG GAME!");
            ChienThang();
        }
    }

    /// <summary>
    /// Hàm xử lý khi người chơi thắng game
    /// Tắt camera giải đố, bật màn hình video và phát video
    /// </summary>
    void ChienThang()
    {
        Debug.Log("[InventoryManager] 🏆 CHIẾN THẮNG! PHÁT VIDEO!");

        // Tắt camera soi đi
        if (puzzleCamera != null)
        {
            puzzleCamera.SetActive(false);
        }

        // Bật màn hình video lên
        if (manHinhVideo != null)
        {
            manHinhVideo.SetActive(true);
            Debug.Log("[InventoryManager] Đã bật màn hình video");
        }
        else
        {
            Debug.LogWarning("[InventoryManager] manHinhVideo chưa được gán! Không thể hiển thị video.");
        }

        // Phát video
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            Debug.Log("[InventoryManager] Đã bắt đầu phát video");
        }
        else
        {
            Debug.LogWarning("[InventoryManager] videoPlayer chưa được gán! Không thể phát video.");
        }

        // Phát âm thanh thắng nếu có
        if (amThanhChienThang != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(amThanhChienThang, Camera.main.transform.position);
        }

        // Có thể thêm logic khác ở đây như:
        // - Hiển thị UI "YOU WIN!"
        // - Tắt toàn bộ gameplay
        // - Chuyển scene sau khi video xong
    }

    /// <summary>
    /// Lấy số lượng đồ đã dán đúng
    /// </summary>
    public int GetSoDoDaDanDung()
    {
        return soDoDaDanDung;
    }
}

