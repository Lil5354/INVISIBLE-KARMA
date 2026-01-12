using UnityEngine;

public class ClickToShowHint : MonoBehaviour
{
    [Header("Cấu hình")]
    public GameObject cameraGoiY; // Kéo cameragoiy vào đây
    public GameObject playerFPS; // Kéo Player vào đây (tùy chọn)
    public float khoangCachToiDa = 5f; // Khoảng cách tối đa để click

    [Header("Tùy chọn")]
    public KeyCode phimThoat = KeyCode.Escape; // Phím để thoát
    public bool tatPlayerKhiXem = true; // Tắt player khi xem gợi ý

    private bool dangXemGoiY = false;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;

        // Tắt camera gợi ý lúc đầu
        if (cameraGoiY != null)
        {
            cameraGoiY.SetActive(false);
        }
    }

    void Update()
    {
        // Nếu đang xem gợi ý, bấm ESC để thoát
        if (dangXemGoiY && Input.GetKeyDown(phimThoat))
        {
            ThoatXemGoiY();
        }
    }

    // Gọi khi click vào object này (cần có Collider)
    void OnMouseDown()
    {
        if (dangXemGoiY) return;

        // Kiểm tra khoảng cách
        if (mainCam != null)
        {
            float dist = Vector3.Distance(transform.position, mainCam.transform.position);
            if (dist > khoangCachToiDa)
            {
                Debug.Log($"[ClickToShowHint] Quá xa! Khoảng cách: {dist:F2}m (Tối đa: {khoangCachToiDa}m)");
                return;
            }
        }

        BatCameraGoiY();
    }

    // Hàm public để gọi từ Raycast (PlayerInteraction)
    public void TuongTac()
    {
        if (!dangXemGoiY)
        {
            BatCameraGoiY();
        }
    }

    void BatCameraGoiY()
    {
        Debug.Log("[ClickToShowHint] Bật camera gợi ý!");
        dangXemGoiY = true;

        // Bật camera gợi ý
        if (cameraGoiY != null)
        {
            cameraGoiY.SetActive(true);
        }

        // Tắt player nếu cần
        if (tatPlayerKhiXem && playerFPS != null)
        {
            playerFPS.SetActive(false);
        }

        // Hiện con trỏ chuột
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ThoatXemGoiY()
    {
        Debug.Log("[ClickToShowHint] Thoát xem gợi ý!");
        dangXemGoiY = false;

        // Tắt camera gợi ý
        if (cameraGoiY != null)
        {
            cameraGoiY.SetActive(false);
        }

        // Bật lại player
        if (tatPlayerKhiXem && playerFPS != null)
        {
            playerFPS.SetActive(true);
        }

        // Ẩn con trỏ chuột
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Hàm public để gọi từ UI Button nếu cần
    public void ToggleGoiY()
    {
        if (dangXemGoiY)
        {
            ThoatXemGoiY();
        }
        else
        {
            BatCameraGoiY();
        }
    }
}
