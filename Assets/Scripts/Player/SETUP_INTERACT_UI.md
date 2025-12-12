# 🎯 HƯỚNG DẪN SETUP UI "NHẤN E" CHO ĐÈN

## ✅ Bước 1: Tạo Canvas (Nếu chưa có)

1. Trong Hierarchy, **chuột phải** → **UI** → **Canvas**
2. Canvas sẽ tự động tạo với:
   - **Canvas** (component chính)
   - **EventSystem** (để xử lý input)
   - **GraphicRaycaster** (để raycast UI)

## ✅ Bước 2: Tạo Text "Nhấn E"

1. **Chuột phải** vào **Canvas** trong Hierarchy
2. Chọn **UI** → **Text - TextMeshPro** (hoặc **Text - Legacy** nếu không có TextMeshPro)
3. Đặt tên: **"InteractPrompt"** hoặc **"PressE"**

## ✅ Bước 3: Cấu hình Text

Chọn Text vừa tạo, trong Inspector:

**Rect Transform:**
- **Anchor Presets**: Nhấn **Alt + Shift** và chọn **Bottom-Center** (hoặc **Middle-Center**)
- **Pos Y**: `100` (cách dưới màn hình 100px)
- **Width**: `400`
- **Height**: `50`

**Text Component:**
- **Text**: `Nhấn [E] để thắp đèn`
- **Font Size**: `24` hoặc `30`
- **Alignment**: **Center** (căn giữa)
- **Color**: **Trắng** hoặc **Vàng** (dễ nhìn)
- **Font Style**: **Bold** (đậm)

**Tùy chọn thêm:**
- Thêm **Outline** hoặc **Shadow** để text nổi bật hơn:
  - Add Component → **Outline** (hoặc **Shadow**)
  - **Effect Color**: Đen
  - **Effect Distance**: (2, -2)

## ✅ Bước 4: Gắn vào PlayerInteract Script

1. Chọn **Player** trong Hierarchy
2. Trong Inspector, tìm component **Player Interact (Script)**
3. Có 2 cách gắn:

   **Cách 1: Gắn GameObject (Khuyên dùng)**
   - Kéo **InteractPrompt** (GameObject Text) từ Hierarchy vào ô **Interact Prompt UI**

   **Cách 2: Gắn Text Component trực tiếp**
   - Kéo **InteractPrompt** vào ô **Interact Prompt Text**

   ⚠️ **Lưu ý**: Chỉ cần gắn 1 trong 2, không cần cả 2!

## ✅ Bước 5: Tùy chỉnh Text (Tùy chọn)

Trong **Player Interact (Script)**, bạn có thể thay đổi:
- **Interact Text**: Text hiển thị khi chưa thắp đèn (mặc định: "Nhấn [E] để thắp đèn")
- **Interact Text Lit**: Text hiển thị sau khi đã thắp đèn (mặc định: "Đèn đã sáng")

## 🎮 Test

1. Bấm **Play**
2. Đi đến gần đèn
3. Nhìn vào đèn → Text "Nhấn [E] để thắp đèn" sẽ hiện ra ở giữa màn hình
4. Bấm **E** → Đèn sáng, text có thể đổi thành "Đèn đã sáng" hoặc ẩn đi

## 🐛 Troubleshooting

**Text không hiện:**
- Kiểm tra Canvas có **Render Mode** = **Screen Space - Overlay** (mặc định)
- Kiểm tra Text có **Color Alpha** > 0 không
- Kiểm tra đã gắn GameObject/Text vào script chưa

**Text bị che:**
- Kiểm tra **Canvas** có **Sort Order** cao hơn các Canvas khác không
- Kiểm tra Text có nằm trong **Canvas** không

**Text hiện nhưng không đúng vị trí:**
- Điều chỉnh **Rect Transform** của Text
- Thử đổi **Anchor Presets** để căn chỉnh

---

**Xong rồi!** Bây giờ người chơi sẽ thấy hướng dẫn rõ ràng khi đến gần đèn! 🎉








