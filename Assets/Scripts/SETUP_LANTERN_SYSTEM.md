# 🔦 HƯỚNG DẪN SETUP HỆ THỐNG ĐÈN LỒNG MỚI

## 📋 TỔNG QUAN

Hệ thống đèn lồng mới cho phép player:
- ✅ **Bật/Tắt đèn** bằng phím **F** (đèn gắn trên người)
- ✅ **Quản lý dầu đèn** - đèn sẽ tự tắt khi hết dầu
- ✅ **Vùng sáng bảo vệ** - ma sẽ sợ và rung lắc khi vào vùng sáng
- ✅ **Hiệu ứng nhấp nháy** khi sắp hết dầu

**Khác biệt với hệ thống cũ:**
- ❌ Không cần tìm đèn bên đường để thắp
- ❌ Không cần nhấn E để tương tác
- ✅ Đèn luôn đi theo player
- ✅ Bật/Tắt nhanh chóng bằng phím F

---

## 🚀 BƯỚC 1: Tạo Point Light trên Player

### 1.1. Chọn Player GameObject

1. Trong **Hierarchy**, tìm và chọn **Player** GameObject
2. (Hoặc chọn **CameraHolder** nếu bạn muốn đèn gắn trên camera)

### 1.2. Tạo Point Light

1. Click chuột phải vào Player → **Light** → **Point Light**
2. Đặt tên: **`LanternLight`**

### 1.3. Cấu hình Point Light

Trong Inspector, cấu hình như sau:

**Light Component:**
- **Type:** Point
- **Range:** `6` (Bán kính vùng an toàn - phải khớp với `safeDistance` trong script)
- **Color:** Màu vàng cam `(255, 200, 100)` hoặc `#FFC864` (cho cảm giác lửa đèn lồng)
- **Intensity:** `1.5` đến `2.0`
- **Shadow Type:** Soft Shadows (để bóng đổ mờ ảo, rùng rợn)
- **Enabled:** ❌ **BỎ TÍCH** (Tắt lúc đầu, script sẽ tự bật)

**Transform:**
- **Position:** `(0, 0, 0)` nếu là child của Player
- Hoặc `(0, -0.5, 0)` nếu muốn đèn thấp hơn camera một chút

---

## 🚀 BƯỚC 2: Gắn Script LanternSystem

### 2.1. Add Component

1. Chọn **Player** GameObject
2. Trong Inspector, click **Add Component**
3. Tìm và chọn: **Lantern System**

### 2.2. Cấu hình Script

Trong Inspector, cấu hình các field:

**Cài đặt Đèn:**
- **Lantern Light:** Kéo **LanternLight** (Point Light vừa tạo) vào đây

**Cài đặt Nhiên liệu:**
- **Max Oil:** `100` (Dầu tối đa)
- **Current Oil:** `100` (Dầu ban đầu - sẽ tự set trong Start)
- **Drain Rate:** `5` (Tốn 5 dầu mỗi giây khi đèn bật)

**Cài đặt Vùng An Toàn:**
- **Safe Distance:** `6.0` (Phải khớp với Range của Point Light!)

**Hiệu ứng:**
- **Normal Intensity:** `1.5` (Cường độ đèn bình thường)
- **Low Oil Intensity Min:** `0.5` (Cường độ tối thiểu khi sắp hết dầu)
- **Low Oil Intensity Max:** `1.5` (Cường độ tối đa khi sắp hết dầu)

---

## 🚀 BƯỚC 3: Cập nhật PaperEnemy (Tự động)

Script `PaperEnemy.cs` đã được cập nhật để tự động:
- ✅ Tìm `LanternSystem` trên Player
- ✅ Kiểm tra khoảng cách với player
- ✅ Tự động bị stun khi vào vùng sáng
- ✅ Rung lắc khi bị đèn chiếu

**Bạn không cần làm gì thêm!** Script sẽ tự động hoạt động.

---

## 🎮 ĐIỀU KHIỂN

### Phím điều khiển:
- **F** - Bật/Tắt đèn lồng

### Cơ chế:
1. **Bật đèn:** Nhấn F → Đèn sáng → Ma trong vùng 6m sẽ bị stun
2. **Tắt đèn:** Nhấn F lần nữa → Đèn tắt → Ma tiếp tục đuổi
3. **Hết dầu:** Đèn tự động tắt khi dầu = 0
4. **Sắp hết dầu:** Đèn nhấp nháy khi dầu < 20%

---

## 🎨 TÙY CHỈNH

### Thay đổi tốc độ tiêu hao dầu:

Trong `LanternSystem.cs`:
- **Drain Rate:** Tăng lên → Đèn tắt nhanh hơn (khó hơn)
- **Drain Rate:** Giảm xuống → Đèn tắt chậm hơn (dễ hơn)

### Thay đổi vùng bảo vệ:

1. Trong **Point Light** → **Range:** Thay đổi (ví dụ: 8m)
2. Trong **LanternSystem** → **Safe Distance:** Thay đổi cùng giá trị (8.0)

⚠️ **Lưu ý:** Range và Safe Distance phải khớp nhau!

### Thay đổi màu đèn:

Trong **Point Light** → **Color:** Chọn màu khác
- Vàng cam: `#FFC864` (đèn lồng)
- Trắng: `#FFFFFF` (đèn pin)
- Xanh: `#00FFFF` (đèn ma)

---

## 🔧 TROUBLESHOOTING

### Vấn đề: Đèn không bật

**Kiểm tra:**
- ✅ Point Light đã được gán vào field "Lantern Light" chưa?
- ✅ Point Light có bị disable không?
- ✅ Console có lỗi gì không?

**Giải pháp:**
- Kéo Point Light vào field "Lantern Light" trong Inspector
- Đảm bảo Point Light không bị disable

### Vấn đề: Ma không sợ đèn

**Kiểm tra:**
- ✅ `LanternSystem` đã được gắn vào Player chưa?
- ✅ `Safe Distance` trong LanternSystem có khớp với `Range` của Point Light không?
- ✅ Đèn có đang bật không? (Nhấn F)

**Giải pháp:**
- Đảm bảo Range = Safe Distance (ví dụ: cả 2 đều = 6)
- Kiểm tra Console để xem có lỗi không

### Vấn đề: Đèn không tắt khi hết dầu

**Kiểm tra:**
- ✅ `Drain Rate` có > 0 không?
- ✅ `Current Oil` có đang giảm không? (Xem trong Inspector khi Play)

**Giải pháp:**
- Đảm bảo `Drain Rate` > 0
- Kiểm tra script có đang chạy không

### Vấn đề: Ma vẫn đuổi khi đèn bật

**Kiểm tra:**
- ✅ Ma có nằm trong vùng 6m không?
- ✅ `PaperEnemy` script có tìm thấy `LanternSystem` không?

**Giải pháp:**
- Kiểm tra khoảng cách: Ma phải cách player < 6m
- Xem Console để kiểm tra lỗi

---

## 📝 CODE SỬ DỤNG (Cho các script khác)

### Kiểm tra đèn có đang bật:

```csharp
LanternSystem lantern = FindObjectOfType<LanternSystem>();
if (lantern != null && lantern.IsLanternOn())
{
    Debug.Log("Đèn đang bật!");
}
```

### Thêm dầu vào đèn:

```csharp
LanternSystem lantern = FindObjectOfType<LanternSystem>();
if (lantern != null)
{
    lantern.AddOil(50f); // Thêm 50 dầu
}
```

### Kiểm tra vị trí có an toàn không:

```csharp
LanternSystem lantern = FindObjectOfType<LanternSystem>();
if (lantern != null && lantern.IsInSafeZone(somePosition))
{
    Debug.Log("Vị trí này an toàn!");
}
```

---

## ✅ CHECKLIST

Sau khi setup, bạn sẽ có:
- [x] Point Light gắn trên Player
- [x] LanternSystem script gắn vào Player
- [x] Point Light được gán vào field "Lantern Light"
- [x] Range và Safe Distance khớp nhau (ví dụ: cả 2 = 6)
- [x] PaperEnemy tự động tìm LanternSystem (không cần setup thêm)

---

## 🎮 SẴN SÀNG TEST!

Bây giờ bạn có thể:
1. **Play Scene**
2. **Nhấn F** để bật đèn
3. Để ma spawn và đuổi theo
4. Khi ma vào vùng 6m → Ma sẽ rung lắc và dừng lại
5. Tắt đèn → Ma tiếp tục đuổi

**Chúc bạn test vui vẻ! 🔦✨**




