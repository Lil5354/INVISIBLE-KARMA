using UnityEngine;

public class SmartSticker : MonoBehaviour
{
    [Header("Cài đặt chung")]
    public Camera mainCam;
    public LayerMask layerConRoi;
    
    // --- THAY ĐỔI Ở ĐÂY ---
    [Header("Danh sách 4 Mảnh Giấy Nháp (Ghost Objects)")]
    // Kéo 4 cái Quad riêng biệt vào đây theo đúng thứ tự:
    // 0: Áo Trước, 1: Áo Sau, 2: Váy, 3: Khăn
    public GameObject[] ghostQuads;
    // ---------------------

    [Header("Cài đặt logic")]
    public float khoangCachHo = 0.02f; // Nhích ra khỏi da một chút

    private int idDangCam = -1; // -1 là chưa cầm gì
    private bool dangKeo = false;

    // Hàm gọi từ UI
    public void BatDauCamDo(int idMonDo)
    {
        Debug.Log($"[SmartSticker] Nút UI gọi BatDauCamDo với ID = {idMonDo}");
        
        // 1. Tắt hết các Ghost cũ đi (đề phòng)
        TatHetGhost();

        // 2. Kiểm tra ID hợp lệ
        if (idMonDo >= 0 && idMonDo < ghostQuads.Length)
        {
            idDangCam = idMonDo;
            // 3. Bật đúng cái Quad tương ứng lên
            if(ghostQuads[idDangCam] != null)
            {
                ghostQuads[idDangCam].SetActive(true);
                dangKeo = true;
            }
        }
    }

    void Update()
    {
        if (dangKeo && idDangCam != -1)
        {
            XuLyTruotTrenDa();

            // Bấm chuột trái để dán
            if (Input.GetMouseButtonDown(0)) {
                ThuDanGiay();
            }

            // Bấm chuột phải hoặc ESC để hủy
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                ketThucKeo();
            }
        }
    }

    void XuLyTruotTrenDa()
    {
        // Dùng tâm màn hình (nếu bạn dùng tâm ngắm) hoặc vị trí chuột
        // Nếu dùng chuột Windows:
        // Ray ray = mainCam.ScreenPointToRay(Input.mousePosition); 

        // Nếu dùng Tâm Ngắm giữa màn hình (như bạn đã setup ở bài trước):
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10f, layerConRoi))
        {
            // Di chuyển cái Quad ĐANG CẦM (theo idDangCam)
            GameObject currentGhost = ghostQuads[idDangCam];
            if (currentGhost != null)
            {
                currentGhost.transform.position = hit.point + (hit.normal * khoangCachHo);
                currentGhost.transform.rotation = Quaternion.LookRotation(-hit.normal);
            }
        }
    }

    void ThuDanGiay()
    {
        // Bắn tia kiểm tra lại lần nữa để xác định vùng dán
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, layerConRoi))
        {
            string tagDung = "";
            switch (idDangCam)
            {
                case 0: tagDung = "Zone_NgucTruoc"; break;
                case 1: tagDung = "Zone_NgucSau"; break;
                case 2: tagDung = "Zone_Chan"; break; 
                case 3: tagDung = "Zone_Dau"; break; 
            }

            // DEBUG: Hiện tag của zone đang dán vào
            Debug.Log($"[SmartSticker] Đang cầm món ID={idDangCam}, cần tag '{tagDung}', zone hiện tại có tag '{hit.collider.tag}'");

            // Kiểm tra Tag (Zone_NgucTruoc, Zone_NgucSau...)
            if (hit.collider.CompareTag(tagDung))
            {
                Debug.Log($"[SmartSticker] ✅ Dán ĐÚNG vị trí! Tag khớp: {tagDung}");
                // GỌI HÀM MỚI BÊN MANAGER
                InventoryManager.Instance.XuLyDanDo(idDangCam);
                // Tắt miếng giấy dán nháp đi
                ketThucKeo();
            }
            else
            {
                Debug.Log($"[SmartSticker] ❌ Dán SAI vị trí! Cần '{tagDung}' nhưng đang dán vào '{hit.collider.tag}'");
            }
        }
    }

    void ketThucKeo()
    {
        dangKeo = false;
        TatHetGhost();
        idDangCam = -1;
    }

    void TatHetGhost()
    {
        // Chạy vòng lặp tắt sạch sẽ cả 4 cái
        foreach(var g in ghostQuads)
        {
            if(g != null) g.SetActive(false);
        }
    }
}
