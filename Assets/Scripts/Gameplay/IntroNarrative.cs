using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class DongThoai
{
    [TextArea(3, 10)]
    public string noiDung;     
    public AudioClip giongDoc; 
    public float thoiGianCho = 1f; 
}

public class IntroNarrative : MonoBehaviour
{
    [Header("--- UI HIỂN THỊ ---")]
    public GameObject manHinhDen;     
    public Text textHienThi;      // Đã đổi từ TMP_Text sang Text thường
    public AudioSource nguonPhatAm;   

    [Header("--- KẾT NỐI INTRO CŨ (KÉO CẢ 3 CÁI VÀO ĐÂY) ---")]
    // Thay đổi từ GameObject đơn lẻ thành Mảng (Array) để chứa nhiều thứ
    public GameObject[] cacThanhPhanIntroCu; 

    [Header("--- CẤU HÌNH ---")]
    public float tocDoGoChu = 0.05f;
    public DongThoai[] danhSachLoiDan;

    private bool nguoiChoiMuonSkip = false;

    void Start()
    {
        // 1. TẮT TẤT CẢ thành phần của Intro cũ (Nền, Panel, Script...)
        ToggleIntroCu(false);

        manHinhDen.SetActive(true);
        StartCoroutine(PhatLoiDan());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            nguoiChoiMuonSkip = true;
        }
    }

    // Hàm phụ trợ để Bật/Tắt danh sách
    void ToggleIntroCu(bool trangThai)
    {
        if (cacThanhPhanIntroCu != null)
        {
            foreach (GameObject obj in cacThanhPhanIntroCu)
            {
                if (obj != null) 
                    obj.SetActive(trangThai);
            }
        }
    }

    IEnumerator PhatLoiDan()
    {
        foreach (DongThoai dong in danhSachLoiDan)
        {
            textHienThi.text = "";
            nguoiChoiMuonSkip = false;

            if (dong.giongDoc != null)
            {
                nguonPhatAm.clip = dong.giongDoc;
                nguonPhatAm.Play();
            }

            foreach (char c in dong.noiDung.ToCharArray())
            {
                if (nguoiChoiMuonSkip)
                {
                    textHienThi.text = dong.noiDung;
                    break; 
                }
                
                textHienThi.text += c;
                yield return new WaitForSeconds(tocDoGoChu);
            }

            float thoiGianDaTroi = 0f;
            float thoiGianCanCho = (dong.giongDoc != null) 
                ? (dong.giongDoc.length - (dong.noiDung.Length * tocDoGoChu)) 
                : dong.thoiGianCho;

            if (thoiGianCanCho < 0) 
                thoiGianCanCho = 0.5f;

            nguoiChoiMuonSkip = false; 

            while (thoiGianDaTroi < thoiGianCanCho)
            {
                if (nguoiChoiMuonSkip)
                {
                    if (nguonPhatAm.isPlaying) 
                        nguonPhatAm.Stop(); 
                    break;
                }
                
                thoiGianDaTroi += Time.deltaTime;
                yield return null;
            }
        }

        KetThucMamboDau();
    }

    void KetThucMamboDau()
    {
        // Hủy diệt màn hình đen
        if (manHinhDen != null) 
            Destroy(manHinhDen); 

        // 2. BẬT LẠI TẤT CẢ thành phần Intro cũ
        ToggleIntroCu(true);
        
        Debug.Log("[IntroNarrative] Đã kết thúc và bật lại Intro cũ!");
    }
}
