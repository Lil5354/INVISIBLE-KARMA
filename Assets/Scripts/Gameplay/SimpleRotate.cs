using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    [Header("Cấu hình")]
    public float tocDo = 100f; // Tốc độ xoay
    public GameObject cameraSoi; // Kéo Camera Soi vào đây

    void Update()
    {
        // 1. Kiểm tra an toàn: Chỉ xoay khi Camera Soi đang bật
        if (cameraSoi == null || !cameraSoi.activeSelf) return;

        // 2. Xoay TRÁI / PHẢI (Quay quanh trục Y thẳng đứng - Space.World để luôn xoay chuẩn)
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up, -tocDo * Time.deltaTime, Space.World);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up, tocDo * Time.deltaTime, Space.World);
        }

        // 3. Xoay LÊN / XUỐNG (Quay quanh trục X ngang - Space.Self để gật theo hướng mặt)
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(Vector3.right, tocDo * Time.deltaTime, Space.Self);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(Vector3.right, -tocDo * Time.deltaTime, Space.Self);
        }
    }
}
