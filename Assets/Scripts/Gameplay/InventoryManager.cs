using TMPro; // <--- Thêm dòng này để dùng TextMeshPro
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Video;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI & Gameplay")]
    public GameObject[] nutBamUI; // Nút trong túi
    public GameObject[] doThat3D; // Đồ thật trên người (0:Trước, 1:Sau, 2:Váy, 3:Khăn)

    [Header("Chiến Thắng & Thua")]
    public GameObject manHinhVideo;
    public VideoPlayer videoPlayer;
    public GameObject puzzleCamera; // Camera soi

    [Header("Sinh Mệnh (Lose Game)")]
    public TextMeshProUGUI textSinhMenh; // Kéo TextMeshPro vào đây
    public float thoiGianToiDa = 180f; // 3 phút = 180 giây
    private float sinhMenh = 100f;
    private float tocDoGiam; // Tính tự động
    private bool gameKetThuc = false;

    [Header("Màn hình Thua")]
    public GameObject manHinhThua; // Panel hiện khi thua (tùy chọn)

    // --- LOGIC THỨ TỰ ---
    private int buocCanLam = 0;
    private int soLanSai = 0;
    private int gioiHanSai = 3;

    // Biến đếm số đồ đã nhặt
    private int soDoDaNhat = 0;
    private bool dangOCheDoGhepDo = false;

    [Header("Cấu hình Gameplay")]
    public GameObject playerFPS;
    public GameObject uiInventory;

    void Awake()
    {
        Instance = this;
        // Tắt hết đồ thật lúc đầu
        if (doThat3D != null)
        {
            foreach (var do3D in doThat3D)
            {
                if (do3D != null) do3D.SetActive(false);
            }
        }
    }

    void Start()
    {
        // Tính tốc độ giảm: 100 điểm / 180 giây
        tocDoGiam = 100f / thoiGianToiDa;
        sinhMenh = 100f;
        gameKetThuc = false;

        // Tắt màn hình thua nếu có
        if (manHinhThua != null) manHinhThua.SetActive(false);

        // Cập nhật UI lần đầu
        CapNhatUISinhMenh();
    }

    void Update()
    {
        // Nếu game đã kết thúc thì không làm gì
        if (gameKetThuc) return;

        // Giảm sinh mệnh theo thời gian
        sinhMenh -= tocDoGiam * Time.deltaTime;

        // Clamp để không âm
        if (sinhMenh < 0) sinhMenh = 0;

        // Cập nhật UI
        CapNhatUISinhMenh();

        // Kiểm tra thua
        if (sinhMenh <= 0)
        {
            LoseGame();
        }
    }

    void CapNhatUISinhMenh()
    {
        if (textSinhMenh != null)
        {
            textSinhMenh.text = "Sinh mệnh: " + Mathf.CeilToInt(sinhMenh).ToString();
        }
    }

    // --- HÀM NHẶT ĐỒ ---
    public void NhatDo(int id)
    {
        if (gameKetThuc) return;

        if (id < 0 || nutBamUI == null || id >= nutBamUI.Length)
        {
            Debug.LogError($"[InventoryManager] ID món đồ không hợp lệ: {id}");
            return;
        }

        if (nutBamUI[id] != null && nutBamUI[id].activeSelf)
        {
            Debug.LogWarning($"[InventoryManager] Món đồ ID {id} đã được nhặt rồi!");
            return;
        }

        if (nutBamUI[id] != null)
        {
            nutBamUI[id].SetActive(true);
            Debug.Log($"[InventoryManager] ✅ Đã nhặt món số: {id}");
        }

        soDoDaNhat++;
        Debug.Log($"[InventoryManager] Tổng số đồ đã nhặt: {soDoDaNhat}/4");
    }

    // --- HÀM KIỂM TRA VÀ ZOOM ---
    public void KiemTraVaZoom()
    {
        if (gameKetThuc) return;
        if (dangOCheDoGhepDo) return;

        if (soDoDaNhat >= 4)
        {
            VaoCheDoGhepDo();
        }
        else
        {
            int soDoConThieu = 4 - soDoDaNhat;
            Debug.Log($"[InventoryManager] ❌ Chưa đủ đồ! Cần tìm thêm {soDoConThieu} món.");
        }
    }

    void VaoCheDoGhepDo()
    {
        Debug.Log("[InventoryManager] 🎯 Vào chế độ giải đố!");
        if (playerFPS != null) playerFPS.SetActive(false);
        if (puzzleCamera != null) puzzleCamera.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        dangOCheDoGhepDo = true;
    }

    // --- HÀM XỬ LÝ DÁN ĐỒ ---
    public void XuLyDanDo(int idMonDo)
    {
        if (gameKetThuc) return;

        Debug.Log($"Bạn vừa dán món số {idMonDo}. Game đang đợi món số {buocCanLam}");

        if (idMonDo == buocCanLam)
        {
            Debug.Log(">>> CHÍNH XÁC! Bước " + buocCanLam + " hoàn thành.");

            if (doThat3D[idMonDo] != null)
            {
                doThat3D[idMonDo].SetActive(true);
            }

            if (nutBamUI[idMonDo] != null)
            {
                nutBamUI[idMonDo].SetActive(false);
            }

            buocCanLam++;

            if (buocCanLam >= 4)
            {
                WinGame();
            }
        }
        else
        {
            XuLyDanSai();
        }
    }

    void XuLyDanSai()
    {
        soLanSai++;
        Debug.LogWarning($"!!! SAI THỨ TỰ! Bạn còn {gioiHanSai - soLanSai} mạng.");

        if (soLanSai >= gioiHanSai)
        {
            ResetPuzzle();
        }
    }

    void ResetPuzzle()
    {
        Debug.LogError("THUA CUỘC! LÀM LẠI TỪ ĐẦU!");

        buocCanLam = 0;
        soLanSai = 0;

        foreach (var do3D in doThat3D)
        {
            if (do3D != null) do3D.SetActive(false);
        }

        foreach (var nut in nutBamUI)
        {
            if (nut != null) nut.SetActive(true);
        }
    }

    void WinGame()
    {
        Debug.Log("===== WIN GAME! PHÁT VIDEO! =====");
        gameKetThuc = true;

        if (puzzleCamera != null) puzzleCamera.SetActive(false);

        if (manHinhVideo != null)
        {
            manHinhVideo.SetActive(true);
            if (videoPlayer != null) videoPlayer.Play();
        }
    }

    void LoseGame()
    {
        Debug.LogError("===== LOSE GAME! HẾT SINH MỆNH! =====");
        gameKetThuc = true;

        // Hiện màn hình thua nếu có
        if (manHinhThua != null)
        {
            manHinhThua.SetActive(true);
        }

        // Tắt camera soi nếu đang bật
        if (puzzleCamera != null) puzzleCamera.SetActive(false);

        // Hiện con trỏ chuột
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Tắt player
        if (playerFPS != null) playerFPS.SetActive(false);
    }

    // Hàm public để restart (gọi từ nút Restart nếu có)
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
