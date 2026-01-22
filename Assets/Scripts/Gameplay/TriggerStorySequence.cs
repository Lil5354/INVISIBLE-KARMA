using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

// Tạo một khuôn mẫu để chứa dữ liệu cho từng Cảnh
[System.Serializable]
public class StorySceneData
{
    public string tenCanh;        // Tên gợi nhớ (VD: Canh 1, Canh 2)
    public Sprite hinhAnhHienThi; // Ảnh 2D của cảnh đó
    [TextArea(3, 10)]
    public string[] loiThoai;     // Các câu thoại trong cảnh đó
    public AudioClip[] AmThanhLoiThoai; // Audio cho từng câu thoại
}

public class TriggerStorySequence : MonoBehaviour
{
    [Header("--- CẤU HÌNH UI ---")]
    public GameObject panelChuyenCanh; // Cái Panel to chứa tất cả
    public Image imgHienThi;           // Nơi hiện ảnh 2D
    public Text textHienThi;           // Đã đổi từ TMP_Text sang Text thường
    public AudioSource nguonPhatAm;    // AudioSource để phát voice

    [Header("--- CẤU HÌNH CỐT TRUYỆN ---")]
    // Danh sách 5 scene của bạn sẽ nằm ở đây
    public List<StorySceneData> danhSachCacCanh; 
    public string tenSceneTiepTheo = "Chapter2"; // Tên màn chơi sẽ load
    public float tocDoGoChu = 0.04f;

    [Header("--- CẤU HÌNH PLAYER (Kéo Player vào đây) ---")]
    public MonoBehaviour[] scriptsCanKhoa; // Script di chuyển, xoay camera...

    [Header("--- TÙY CHỌN ---")]
    [Tooltip("Chỉ trigger một lần")]
    public bool triggerOnce = true;
    
    [Tooltip("Tự động tìm player scripts nếu mảng rỗng")]
    public bool autoFindPlayerScripts = true;

    [Header("--- CHẾ ĐỘ END GAME (QUAN TRỌNG) ---")]
    [Tooltip("Tích vào đây nếu dùng cho End Game (Tự chạy khi bật Object, không cần va chạm)")]
    public bool tuDongKichHoat = false; // <-- BIẾN MỚI THÊM

    // Biến nội bộ để theo dõi tiến độ
    private bool dangKichHoat = false;
    private bool dangGoChu = false;
    private int indexCanhHienTai = 0;   // Đang ở ảnh số mấy (0 đến 4)
    private int indexThoaiHienTai = 0;  // Đang ở dòng thoại số mấy
    private bool hasTriggered = false;

    // Dùng Awake để tắt UI ngay lập tức khi game load
    void Awake()
    {
        if (panelChuyenCanh != null && !tuDongKichHoat) 
            panelChuyenCanh.SetActive(false);
    }

    // Hàm này chạy mỗi khi GameObject được SetActive(true)
    void OnEnable()
    {
        if (tuDongKichHoat)
        {
            Debug.Log("[TriggerStorySequence] Phát hiện chế độ Tự Động -> Chạy cốt truyện ngay!");
            BatDauCotTruyen();
        }
    }

    void Start()
    {
        // Chỉ thêm Collider nếu KHÔNG PHẢI chế độ tự động
        if (!tuDongKichHoat)
        {
            Collider col = GetComponent<Collider>();
            if (col == null)
            {
                col = gameObject.AddComponent<BoxCollider>();
                col.isTrigger = true;
            }
            else if (!col.isTrigger)
            {
                col.isTrigger = true;
            }
        }
    }

    void Update()
    {
        // Chỉ khi đang kích hoạt mới nhận nút bấm
        if (dangKichHoat && Input.GetMouseButtonDown(0))
        {
            XuLyTiepTheo();
        }
        
        // Phím Space cũng có thể dùng
        if (dangKichHoat && Input.GetKeyDown(KeyCode.Space))
        {
            XuLyTiepTheo();
        }
    }

    void XuLyTiepTheo()
    {
        if (dangGoChu)
        {
            // Nếu chữ đang chạy mà bấm -> Hiện hết luôn
            StopAllCoroutines();
            if (indexCanhHienTai < danhSachCacCanh.Count && 
                indexThoaiHienTai < danhSachCacCanh[indexCanhHienTai].loiThoai.Length)
            {
                textHienThi.text = danhSachCacCanh[indexCanhHienTai].loiThoai[indexThoaiHienTai];
            }
            dangGoChu = false;
        }
        else
        {
            // Chuyển sang dòng thoại tiếp theo
            NextLine();
        }
    }

    // --- HÀM KÍCH HOẠT KHI ĐI QUA CUBE (Dành cho Intro) ---
    void OnTriggerEnter(Collider other)
    {
        // Nếu đang ở chế độ tự động thì bỏ qua va chạm (để tránh lỗi chạy 2 lần)
        if (tuDongKichHoat) return;

        if (other.CompareTag("Player") && !dangKichHoat)
        {
            if (triggerOnce && hasTriggered) return;
            
            Debug.Log($"[TriggerStorySequence] Player đã vào trigger: {gameObject.name}");
            BatDauCotTruyen();
        }
    }

    void BatDauCotTruyen()
    {
        if (danhSachCacCanh == null || danhSachCacCanh.Count == 0)
        {
            Debug.LogError("[TriggerStorySequence] Không có dữ liệu cảnh để hiển thị!");
            return;
        }

        // Vô hiệu hóa quái vật ngay khi bắt đầu truyện (để không bị cắn lúc đang đọc)
        VoHieuHoaKeThu(); 

        dangKichHoat = true;
        hasTriggered = true;
        
        if (panelChuyenCanh != null) 
            panelChuyenCanh.SetActive(true);

        // 1. Mở chuột để click
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. Khóa di chuyển nhân vật
        TogglePlayer(false);

        // 3. Load cảnh đầu tiên
        indexCanhHienTai = 0;
        LoadCanh(0);
        
        Debug.Log($"[TriggerStorySequence] Bắt đầu story sequence với {danhSachCacCanh.Count} cảnh");
    }

    void LoadCanh(int index)
    {
        if (index >= danhSachCacCanh.Count) return;
        
        // Reset lại dòng thoại về 0
        indexThoaiHienTai = 0;

        // Đổi Ảnh
        if (imgHienThi != null && danhSachCacCanh[index].hinhAnhHienThi != null)
        {
            imgHienThi.sprite = danhSachCacCanh[index].hinhAnhHienThi;
            
            // Đảm bảo ảnh luôn hiện rõ (tránh trường hợp bị trong suốt)
            var tempColor = imgHienThi.color;
            tempColor.a = 1f;
            imgHienThi.color = tempColor;
        }

        // Bắt đầu chạy dòng thoại đầu tiên
        if (danhSachCacCanh[index].loiThoai != null && danhSachCacCanh[index].loiThoai.Length > 0)
        {
            PhatAmThanh(index, 0);
            StartCoroutine(GoChu(danhSachCacCanh[index].loiThoai[0]));
        }
        else
        {
            // Tự động chuyển cảnh tiếp theo nếu không có thoại
            NextScene();
        }
    }

    void NextLine()
    {
        // --- FIX LỖI: Kiểm tra an toàn xem index cảnh có hợp lệ không ---
        if (indexCanhHienTai >= danhSachCacCanh.Count)
        {
            KetThucSequence();
            return;
        }
        // -------------------------------------------------------------

        indexThoaiHienTai++;

        // KIỂM TRA 1: Còn thoại trong cảnh này không?
        if (danhSachCacCanh[indexCanhHienTai].loiThoai != null &&
            indexThoaiHienTai < danhSachCacCanh[indexCanhHienTai].loiThoai.Length)
        {
            PhatAmThanh(indexCanhHienTai, indexThoaiHienTai);
            StartCoroutine(GoChu(danhSachCacCanh[indexCanhHienTai].loiThoai[indexThoaiHienTai]));
        }
        else
        {
            // Hết thoại cảnh này -> Sang Cảnh (Ảnh) Tiếp Theo
            NextScene();
        }
    }

    void NextScene()
    {
        indexCanhHienTai++;

        // KIỂM TRA 2: Còn Cảnh (Ảnh) nào nữa không?
        if (indexCanhHienTai < danhSachCacCanh.Count)
        {
            LoadCanh(indexCanhHienTai);
        }
        else
        {
            // HẾT SẠCH CẢNH -> CHUYỂN LEVEL HOẶC END GAME
            KetThucSequence();
        }
    }

    IEnumerator GoChu(string cauNoi)
    {
        dangGoChu = true;
        
        if (textHienThi != null) 
            textHienThi.text = "";
        
        foreach (char c in cauNoi.ToCharArray())
        {
            if (textHienThi != null) 
                textHienThi.text += c;
            yield return new WaitForSeconds(tocDoGoChu);
        }
        
        dangGoChu = false;
    }

    void KetThucSequence()
    {
        // --- FIX LỖI: Ngắt input ngay lập tức ---
        dangKichHoat = false;
        // ----------------------------------------

        Debug.Log("[TriggerStorySequence] KẾT THÚC CỐT TRUYỆN.");

        if (!string.IsNullOrEmpty(tenSceneTiepTheo) && tenSceneTiepTheo != "Chapter2")
        {
            SceneManager.LoadScene(tenSceneTiepTheo);
        }
        else
        {
            Debug.Log("Dừng tại màn hình kết thúc.");
            // Tùy chọn: Hiện lại con trỏ chuột nếu cần
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void TogglePlayer(bool trangThai)
    {
        foreach (var script in scriptsCanKhoa)
        {
            if (script != null) 
                script.enabled = trangThai;
        }

        if (!trangThai && autoFindPlayerScripts && (scriptsCanKhoa == null || scriptsCanKhoa.Length == 0))
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                // Tắt các script điều khiển phổ biến
                MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
                foreach (var s in scripts)
                {
                    // Tắt MouseLook, PlayerController, FirstPersonController...
                    if (s.GetType().Name.Contains("Controller") || 
                        s.GetType().Name.Contains("Mouse") || 
                        s.GetType().Name.Contains("Look"))
                    {
                        s.enabled = false;
                    }
                }
            }
        }
    }

    void VoHieuHoaKeThu()
    {
        GameObject[] luQuaiVat = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject quai in luQuaiVat)
        {
            quai.SetActive(false);
        }
        
        Debug.Log($"[TriggerStorySequence] Đã vô hiệu hóa {luQuaiVat.Length} con quái!");
    }

    void PhatAmThanh(int indexCanh, int indexThoai)
    {
        if (nguonPhatAm == null) return;
        if (danhSachCacCanh[indexCanh].AmThanhLoiThoai == null) return;
        if (indexThoai >= danhSachCacCanh[indexCanh].AmThanhLoiThoai.Length) return;
        
        AudioClip clip = danhSachCacCanh[indexCanh].AmThanhLoiThoai[indexThoai];
        if (clip != null)
        {
            nguonPhatAm.clip = clip;
            nguonPhatAm.Play();
        }
    }
}
