using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StoryTransition : MonoBehaviour
{
    [Header("Cấu hình UI")]
    public GameObject panelChuyenCanh; 
    public TMP_Text textHienThi;       

    [Header("Cấu hình Nội Dung")]
    [TextArea(3, 10)] 
    public string[] cacCauThoai;       
    public float tocDoGoChu = 0.05f;   

    [Header("CẤU HÌNH NGƯỜI CHƠI (BẮT BUỘC)")]
    // Kéo thả toàn bộ các Script điều khiển nhân vật vào đây 
    // (Ví dụ: PlayerMovement, MouseLook, FirstPersonController...)
    public MonoBehaviour[] cacScriptCanKhoa; 

    private bool dangHoatDong = false;
    private int indexCauThoai = 0;

    void Start()
    {
        if(panelChuyenCanh != null) 
            panelChuyenCanh.SetActive(false);
    }

    void Update()
    {
        if (dangHoatDong && Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    public void BatDauChuyenCanh()
    {
        if (dangHoatDong) return; 
        
        dangHoatDong = true;
        panelChuyenCanh.SetActive(true); 

        // 1. MỞ KHÓA CHUỘT ĐỂ CLICK
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. TẠM DỪNG NGƯỜI CHƠI (Để không đi lung tung lúc đang đọc)
        TogglePlayerControl(false);

        indexCauThoai = 0;
        StartCoroutine(GoChu(cacCauThoai[indexCauThoai]));
    }

    void NextLine()
    {
        StopAllCoroutines(); 
        indexCauThoai++;
        if (indexCauThoai < cacCauThoai.Length)
        {
            StartCoroutine(GoChu(cacCauThoai[indexCauThoai]));
        }
        else
        {
            KetThucHoiThoai();
        }
    }

    IEnumerator GoChu(string cauNoi)
    {
        textHienThi.text = ""; 
        foreach (char c in cauNoi.ToCharArray())
        {
            textHienThi.text += c;
            yield return new WaitForSeconds(tocDoGoChu);
        }
    }

    void KetThucHoiThoai()
    {
        panelChuyenCanh.SetActive(false);
        dangHoatDong = false;

        // 3. KHÓA CHUỘT LẠI (QUAN TRỌNG: Để xoay camera được)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 4. MỞ LẠI NGƯỜI CHƠI (Cho phép đi lại)
        Debug.Log("[StoryTransition] Đang bật lại player control...");
        TogglePlayerControl(true);

        Debug.Log("Đã xong thoại. Tiếp tục chơi!");
    }

    // Hàm bật/tắt các script của nhân vật
    void TogglePlayerControl(bool trangThai)
    {
        Debug.Log($"[StoryTransition] TogglePlayerControl({trangThai}) - Số script: {cacScriptCanKhoa.Length}");
        
        foreach (var script in cacScriptCanKhoa)
        {
            if (script != null) 
            {
                script.enabled = trangThai;
                Debug.Log($"[StoryTransition] {script.GetType().Name}: enabled = {script.enabled}");
            }
            else
            {
                Debug.LogWarning("[StoryTransition] Có script NULL trong mảng cacScriptCanKhoa!");
            }
        }
        
        // Kiểm tra thêm: Tự động tìm và bật các script player chính nếu cần
        if (trangThai && cacScriptCanKhoa.Length == 0)
        {
            Debug.LogWarning("[StoryTransition] Mảng cacScriptCanKhoa rỗng! Đang tự động tìm player scripts...");
            AutoEnablePlayerScripts();
        }
    }
    
    /// <summary>
    /// Tự động tìm và bật lại các script player chính (backup method)
    /// </summary>
    void AutoEnablePlayerScripts()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[StoryTransition] Không tìm thấy Player với tag 'Player'!");
            return;
        }
        
        // Tìm và bật các script player phổ biến
        FirstPersonController fps = player.GetComponent<FirstPersonController>();
        if (fps != null)
        {
            fps.enabled = true;
            Debug.Log("[StoryTransition] Đã bật FirstPersonController");
        }
        
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.enabled = true;
            Debug.Log("[StoryTransition] Đã bật PlayerController");
        }
        
        MouseLook ml = player.GetComponent<MouseLook>();
        if (ml != null)
        {
            ml.enabled = true;
            Debug.Log("[StoryTransition] Đã bật MouseLook");
        }
        
        // Tìm MouseLook trong children (thường ở Camera)
        MouseLook[] mouseLooks = player.GetComponentsInChildren<MouseLook>();
        foreach (var mouseLook in mouseLooks)
        {
            mouseLook.enabled = true;
            Debug.Log($"[StoryTransition] Đã bật MouseLook trên {mouseLook.gameObject.name}");
        }
        
        Debug.Log("[StoryTransition] Đã hoàn thành auto-enable player scripts");
    }
}