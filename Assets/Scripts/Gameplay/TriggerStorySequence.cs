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
}

public class TriggerStorySequence : MonoBehaviour
{
    [Header("--- CẤU HÌNH UI ---")]
    public GameObject panelChuyenCanh; // Cái Panel to chứa tất cả
    public Image imgHienThi;           // Nơi hiện ảnh 2D
    public Text textHienThi;           // Đã đổi từ TMP_Text sang Text thường

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

    // Biến nội bộ để theo dõi tiến độ
    private bool dangKichHoat = false;
    private bool dangGoChu = false;
    private int indexCanhHienTai = 0;   // Đang ở ảnh số mấy (0 đến 4)
    private int indexThoaiHienTai = 0;  // Đang ở dòng thoại số mấy
    private bool hasTriggered = false;

    void Start()
    {
        // Ẩn UI lúc bắt đầu
        if (panelChuyenCanh != null) 
            panelChuyenCanh.SetActive(false);
        
        // Đảm bảo có Collider và là Trigger
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogWarning("[TriggerStorySequence] Không có Collider! Đang thêm BoxCollider...");
            col = gameObject.AddComponent<BoxCollider>();
        }
        
        if (!col.isTrigger)
        {
            Debug.LogWarning("[TriggerStorySequence] Collider chưa được set là Trigger! Đang sửa...");
            col.isTrigger = true;
        }
        
        // Kiểm tra dữ liệu
        if (danhSachCacCanh == null || danhSachCacCanh.Count == 0)
        {
            Debug.LogWarning("[TriggerStorySequence] Danh sách cảnh trống! Hãy thêm StorySceneData vào Inspector.");
        }
    }

    void Update()
    {
        // Chỉ khi đang kích hoạt mới nhận nút bấm
        if (dangKichHoat && Input.GetMouseButtonDown(0))
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
        
        // Phím Space cũng có thể dùng
        if (dangKichHoat && Input.GetKeyDown(KeyCode.Space))
        {
            if (dangGoChu)
            {
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
                NextLine();
            }
        }
    }

    // --- HÀM KÍCH HOẠT KHI ĐI QUA CUBE ---
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !dangKichHoat)
        {
            // Kiểm tra đã trigger chưa
            if (triggerOnce && hasTriggered)
            {
                return;
            }
            
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
        
        dangKichHoat = true;
        hasTriggered = true;
        panelChuyenCanh.SetActive(true);

        // 1. Mở chuột để click
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. Khóa di chuyển nhân vật
        Debug.Log("[TriggerStorySequence] Đang khóa player control...");
        TogglePlayer(false);

        // 3. Load cảnh đầu tiên
        indexCanhHienTai = 0;
        LoadCanh(0);
        
        Debug.Log($"[TriggerStorySequence] Bắt đầu story sequence với {danhSachCacCanh.Count} cảnh");
    }

    void LoadCanh(int index)
    {
        if (index >= danhSachCacCanh.Count)
        {
            Debug.LogError($"[TriggerStorySequence] Index cảnh {index} vượt quá số lượng cảnh ({danhSachCacCanh.Count})!");
            return;
        }
        
        // Reset lại dòng thoại về 0
        indexThoaiHienTai = 0;

        // Đổi Ảnh
        if (imgHienThi != null && danhSachCacCanh[index].hinhAnhHienThi != null)
        {
            imgHienThi.sprite = danhSachCacCanh[index].hinhAnhHienThi;
            Debug.Log($"[TriggerStorySequence] Đã load ảnh cho cảnh: {danhSachCacCanh[index].tenCanh}");
        }
        else if (imgHienThi == null)
        {
            Debug.LogWarning("[TriggerStorySequence] imgHienThi chưa được gán!");
        }
        else if (danhSachCacCanh[index].hinhAnhHienThi == null)
        {
            Debug.LogWarning($"[TriggerStorySequence] Cảnh '{danhSachCacCanh[index].tenCanh}' không có ảnh!");
        }

        // Bắt đầu chạy dòng thoại đầu tiên của cảnh này
        if (danhSachCacCanh[index].loiThoai != null && danhSachCacCanh[index].loiThoai.Length > 0)
        {
            StartCoroutine(GoChu(danhSachCacCanh[index].loiThoai[0]));
        }
        else
        {
            Debug.LogWarning($"[TriggerStorySequence] Cảnh '{danhSachCacCanh[index].tenCanh}' không có thoại!");
            // Tự động chuyển cảnh tiếp theo nếu không có thoại
            NextScene();
        }
    }

    void NextLine()
    {
        indexThoaiHienTai++;
        
        // KIỂM TRA 1: Còn thoại trong cảnh này không?
        if (indexThoaiHienTai < danhSachCacCanh[indexCanhHienTai].loiThoai.Length)
        {
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
            // HẾT SẠCH CẢNH -> CHUYỂN LEVEL
            LoadMapMoi();
        }
    }

    IEnumerator GoChu(string cauNoi)
    {
        dangGoChu = true;
        textHienThi.text = "";
        
        foreach (char c in cauNoi.ToCharArray())
        {
            textHienThi.text += c;
            yield return new WaitForSeconds(tocDoGoChu);
        }
        
        dangGoChu = false;
    }

    void LoadMapMoi()
    {
        Debug.Log("[TriggerStorySequence] Kết thúc cốt truyện. Chuyển sang: " + tenSceneTiepTheo);
        
        // Kiểm tra scene có tồn tại không
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == tenSceneTiepTheo)
            {
                sceneExists = true;
                break;
            }
        }
        
        if (!sceneExists)
        {
            Debug.LogError($"[TriggerStorySequence] ❌ KHÔNG TÌM THẤY SCENE '{tenSceneTiepTheo}' TRONG BUILD SETTINGS!");
            Debug.LogError($"[TriggerStorySequence] Vui lòng thêm scene vào File -> Build Settings");
            
            // Fallback: Kết thúc story và quay lại gameplay
            KetThucStory();
            return;
        }
        
        SceneManager.LoadScene(tenSceneTiepTheo);
    }
    
    /// <summary>
    /// Kết thúc story và quay lại gameplay (không chuyển scene)
    /// </summary>
    void KetThucStory()
    {
        panelChuyenCanh.SetActive(false);
        dangKichHoat = false;

        // Khóa chuột lại cho FPS
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Bật lại player control
        Debug.Log("[TriggerStorySequence] Đang bật lại player control...");
        TogglePlayer(true);

        Debug.Log("[TriggerStorySequence] Đã kết thúc story sequence. Tiếp tục gameplay!");
    }

    void TogglePlayer(bool trangThai)
    {
        Debug.Log($"[TriggerStorySequence] TogglePlayer({trangThai}) - Số script: {scriptsCanKhoa.Length}");
        
        foreach (var script in scriptsCanKhoa)
        {
            if (script != null) 
            {
                script.enabled = trangThai;
                Debug.Log($"[TriggerStorySequence] {script.GetType().Name}: enabled = {script.enabled}");
            }
            else
            {
                Debug.LogWarning("[TriggerStorySequence] Có script NULL trong mảng scriptsCanKhoa!");
            }
        }
        
        // Auto-find player scripts nếu mảng rỗng
        if (trangThai && scriptsCanKhoa.Length == 0 && autoFindPlayerScripts)
        {
            Debug.LogWarning("[TriggerStorySequence] Mảng scriptsCanKhoa rỗng! Đang tự động tìm player scripts...");
            AutoEnablePlayerScripts();
        }
    }
    
    /// <summary>
    /// Tự động tìm và bật lại các script player chính
    /// </summary>
    void AutoEnablePlayerScripts()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[TriggerStorySequence] Không tìm thấy Player với tag 'Player'!");
            return;
        }
        
        // Tìm và bật các script player phổ biến
        FirstPersonController fps = player.GetComponent<FirstPersonController>();
        if (fps != null)
        {
            fps.enabled = true;
            Debug.Log("[TriggerStorySequence] Đã bật FirstPersonController");
        }
        
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.enabled = true;
            Debug.Log("[TriggerStorySequence] Đã bật PlayerController");
        }
        
        MouseLook ml = player.GetComponent<MouseLook>();
        if (ml != null)
        {
            ml.enabled = true;
            Debug.Log("[TriggerStorySequence] Đã bật MouseLook");
        }
        
        // Tìm MouseLook trong children (thường ở Camera)
        MouseLook[] mouseLooks = player.GetComponentsInChildren<MouseLook>();
        foreach (var mouseLook in mouseLooks)
        {
            mouseLook.enabled = true;
            Debug.Log($"[TriggerStorySequence] Đã bật MouseLook trên {mouseLook.gameObject.name}");
        }
        
        Debug.Log("[TriggerStorySequence] Đã hoàn thành auto-enable player scripts");
    }
    
    /// <summary>
    /// Skip toàn bộ story và chuyển scene ngay (có thể gọi từ UI button)
    /// </summary>
    public void SkipStory()
    {
        StopAllCoroutines();
        LoadMapMoi();
    }
    
    /// <summary>
    /// Reset trigger để có thể kích hoạt lại
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
        dangKichHoat = false;
        Debug.Log("[TriggerStorySequence] Đã reset trigger");
    }
    
    /// <summary>
    /// Vẽ gizmo để debug trong Scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = hasTriggered ? Color.red : Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            if (col is BoxCollider)
            {
                BoxCollider box = col as BoxCollider;
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider)
            {
                SphereCollider sphere = col as SphereCollider;
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
        }
    }
    
    // Hàm này sẽ tìm tất cả quái và tắt chúng đi
    void VoHieuHoaKeThu()
    {
        // Tìm tất cả object có Tag là "Enemy"
        GameObject[] luQuaiVat = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject quai in luQuaiVat)
        {
            // Tắt nó đi (Nó sẽ biến mất và không gây damage được nữa)
            quai.SetActive(false);
        }
        
        Debug.Log("Đã vô hiệu hóa " + luQuaiVat.Length + " con quái!");
    }
}