using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [Header("Cài đặt")]
    public float tocDoXoay = 500f;
    public bool daoNguocHuong = true;

    [Header("BẮT BUỘC: Kéo Camera Soi Hình Nhân vào đây")]
    public GameObject cameraSoiHinhNhan; // Biến "Cái Phanh"

    void Update()
    {
        // --- ĐOẠN KIỂM TRA MỚI ---
        // Nếu chưa gán Camera HOẶC Camera đang tắt -> Thì không làm gì cả
        if (cameraSoiHinhNhan == null || !cameraSoiHinhNhan.activeSelf)
        {
            return;
        }

        // --- LOGIC XOAY (Chỉ chạy khi Camera đang bật) ---

        // Dùng CHUỘT PHẢI (Right Click) để xoay
        if (Input.GetMouseButton(1))
        {
            float rotX = Input.GetAxis("Mouse X") * tocDoXoay * Time.deltaTime;

            if (daoNguocHuong) rotX = -rotX;

            // Xoay quanh trục Y
            transform.Rotate(Vector3.up, rotX);
        }

        // Hỗ trợ phím A/D
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, tocDoXoay * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, -tocDoXoay * Time.deltaTime);
        }
    }
}
