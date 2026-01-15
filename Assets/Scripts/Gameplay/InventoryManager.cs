using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // SỬA LỖI Ở ĐÂY: Đổi 'instance' thành 'Instance' (viết hoa chữ I)
    public static InventoryManager Instance; 

    [Header("--- UI & Vật phẩm ---")]
    [Tooltip("Kéo các nút bấm trong Inventory vào đây")]
    public Button[] NutBamUI;       
    
    [Tooltip("Kéo các Mesh quần áo trên người hình nhân vào đây")]
    public GameObject[] DoThat3D;   

    [Header("--- Cấu hình Kết thúc (Mới) ---")]
    [Tooltip("Kéo GameObject chứa script TriggerStorySequence (Ending) vào đây")]
    public GameObject StoryEndingObject; 
    
    [Tooltip("Camera soi cận cảnh hình nhân")]
    public GameObject PuzzleCamera;     

    [Header("--- Sinh Mệnh & Thua ---")]
    public Text TextSinhMenh; // Đã đổi từ TextMeshProUGUI sang Text thường
    public float ThoiGianToiDa = 180;
    
    [Tooltip("Kéo màn hình You Lose vào đây")]
    public GameObject ManHinhThua;

    [Header("--- Cấu hình Khác ---")]
    public GameObject PlayerFPS;    
    public GameObject UiInventory;  

    // Biến nội bộ
    private float thoiGianHienTai;
    private bool[] daMacDung; 
    private bool isGameActive = true;

    void Awake()
    {
        // Gán Instance bằng this (viết hoa chữ I)
        Instance = this;

        // Tự động khởi tạo mảng
        if (NutBamUI != null)
            daMacDung = new bool[NutBamUI.Length];
        else
            daMacDung = new bool[4]; 

        thoiGianHienTai = ThoiGianToiDa;

        // Đảm bảo Story Ending luôn TẮT khi bắt đầu game
        if (StoryEndingObject != null)
        {
            StoryEndingObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!isGameActive) return;

        if (thoiGianHienTai > 0)
        {
            thoiGianHienTai -= Time.deltaTime;
            if (TextSinhMenh != null)
                TextSinhMenh.text = "Sinh mệnh: " + Mathf.RoundToInt(thoiGianHienTai).ToString();
        }
        else
        {
            TriggerLoseGame();
        }
    }

    // Hàm này được gọi từ các script CollectableItem
    public void CheckItem(int itemID)
    {
        if (!isGameActive) return;

        // Hiện đồ 3D
        if (DoThat3D != null && itemID >= 0 && itemID < DoThat3D.Length)
        {
            if (DoThat3D[itemID] != null) 
                DoThat3D[itemID].SetActive(true);
        }

        // Khóa nút bấm
        if (NutBamUI != null && itemID >= 0 && itemID < NutBamUI.Length)
        {
            if (NutBamUI[itemID] != null) 
                NutBamUI[itemID].interactable = false;
        }

        // Đánh dấu hoàn thành
        if (daMacDung != null && itemID < daMacDung.Length)
        {
            daMacDung[itemID] = true;
        }

        CheckWinCondition();
    }

    // Hàm này được gọi từ CollectableItem khi nhặt đồ
    public void NhatDo(int idMonDo)
    {
        if (!isGameActive) return;

        Debug.Log($"[InventoryManager] Đã nhặt món đồ ID: {idMonDo}");

        // Bật nút bấm tương ứng trong UI
        if (NutBamUI != null && idMonDo >= 0 && idMonDo < NutBamUI.Length)
        {
            if (NutBamUI[idMonDo] != null)
            {
                NutBamUI[idMonDo].gameObject.SetActive(true);
                NutBamUI[idMonDo].interactable = true;
            }
        }
    }

    // Hàm này được gọi từ SmartSticker khi dán đồ đúng vị trí
    public void XuLyDanDo(int idMonDo)
    {
        if (!isGameActive) return;

        Debug.Log($"[InventoryManager] Đã dán đúng món đồ ID: {idMonDo}");

        // Hiện đồ 3D trên hình nhân
        if (DoThat3D != null && idMonDo >= 0 && idMonDo < DoThat3D.Length)
        {
            if (DoThat3D[idMonDo] != null)
                DoThat3D[idMonDo].SetActive(true);
        }

        // Tắt nút bấm (đã dùng xong)
        if (NutBamUI != null && idMonDo >= 0 && idMonDo < NutBamUI.Length)
        {
            if (NutBamUI[idMonDo] != null)
                NutBamUI[idMonDo].gameObject.SetActive(false);
        }

        // Đánh dấu hoàn thành
        if (daMacDung != null && idMonDo < daMacDung.Length)
        {
            daMacDung[idMonDo] = true;
        }

        CheckWinCondition();
    }

    // Hàm này được gọi từ ClickToZoom khi click vào hình nhân
    public void KiemTraVaZoom()
    {
        if (!isGameActive) return;

        Debug.Log("[InventoryManager] Đang kiểm tra để zoom vào hình nhân...");

        // Kiểm tra xem đã nhặt đủ đồ chưa
        bool daNhatDuDo = true;
        if (NutBamUI != null)
        {
            foreach (var nut in NutBamUI)
            {
                if (nut != null && !nut.gameObject.activeSelf)
                {
                    daNhatDuDo = false;
                    break;
                }
            }
        }

        if (daNhatDuDo)
        {
            // Đã nhặt đủ đồ -> Chuyển sang chế độ puzzle
            Debug.Log("[InventoryManager] Đã nhặt đủ đồ! Chuyển sang chế độ puzzle...");
            
            // Tắt player FPS
            if (PlayerFPS != null)
                PlayerFPS.SetActive(false);

            // Bật camera puzzle
            if (PuzzleCamera != null)
                PuzzleCamera.SetActive(true);

            // Mở khóa chuột
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.Log("[InventoryManager] Chưa nhặt đủ đồ! Hãy tìm thêm vật phẩm.");
        }
    }

    void CheckWinCondition()
    {
        bool isWin = true;
        
        if (daMacDung != null)
        {
            for (int i = 0; i < daMacDung.Length; i++)
            {
                if (!daMacDung[i])
                {
                    isWin = false;
                    break;
                }
            }
        }

        if (isWin)
        {
            TriggerWinGame();
        }
    }

    void TriggerWinGame()
    {
        isGameActive = false;
        Debug.Log("CHIẾN THẮNG! Bắt đầu chạy Story Ending...");

        if (UiInventory != null) 
            UiInventory.SetActive(false);
        
        if (TextSinhMenh != null) 
            TextSinhMenh.gameObject.SetActive(false);

        // KÍCH HOẠT STORY ENDING
        if (StoryEndingObject != null)
        {
            StoryEndingObject.SetActive(true); 
        }
    }

    public void TriggerLoseGame()
    {
        isGameActive = false;
        
        if (ManHinhThua != null) 
            ManHinhThua.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
