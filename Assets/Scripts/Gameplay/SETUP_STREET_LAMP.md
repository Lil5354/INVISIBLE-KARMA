# 🔦 HƯỚNG DẪN SETUP ĐÈN TĨNH TRÊN ĐƯỜNG (3 ĐÈN)

## 📋 TỔNG QUAN

Hệ thống đèn tĩnh cho phép:
- ✅ Player tương tác bằng phím **E** để bật/tắt đèn
- ✅ Đèn tĩnh bảo vệ player khỏi enemy (giống đèn lồng)
- ✅ Tích hợp với hệ thống stress (giảm stress khi vào vùng an toàn)
- ✅ Tự động tạo vùng an toàn (Sphere Collider)

---

## 🚀 BƯỚC 1: Setup Player Interaction

### 1.1. Gắn Script vào Camera

1. **Chọn Main Camera** (trong Player → CameraHolder)
2. Trong Inspector, click **Add Component**
3. Tìm và chọn: **Player Interaction**

### 1.2. Cấu hình PlayerInteraction

Trong Inspector, cấu hình:

**Cài đặt Tương tác:**
- **Interact Range:** `3.0` (Khoảng cách có thể tương tác - 3 mét)
- **Interactable Layer:** Chọn **Everything** (hoặc tạo Layer riêng "Interactable")
- **Interact Key:** `E` (phím tương tác)

**UI Hiển thị:**
- **Show Interact Prompt:** ✅ (Hiển thị "Nhấn E để tương tác")
- **Show Debug Ray:** ✅ (Hiển thị tia ray trong Scene view)

---

## 🚀 BƯỚC 2: Setup Đèn Tĩnh (StreetLamp)

### 2.1. Tạo Layer "Interactable" (Tùy chọn - Để tối ưu)

1. Trong Unity, góc trên bên phải → **Layers** → **Edit Layers...**
2. Tại ô trống (ví dụ User Layer 6), gõ: **Interactable**
3. Click **Save**

### 2.2. Setup Cây Đèn

1. **Chọn cây đèn** trong Hierarchy (hoặc tạo mới)
2. **Đổi Layer** thành **Interactable** (nếu đã tạo)
3. **Thêm Collider:**
   - Click **Add Component** → **Box Collider** (hoặc **Sphere Collider**)
   - **Chỉnh kích thước** Collider to ra bao quanh cái lồng đèn
   - ⚠️ **Quan trọng:** Collider phải đủ lớn để player dễ trỏ chuột trúng!

4. **Gắn Script:**
   - Click **Add Component** → **Street Lamp**
   - Kéo **Point Light** của đèn vào ô **Lamp Light**
   - (Tùy chọn) Kéo **Fire Particle** vào ô **Fire Particle**

### 2.3. Cấu hình StreetLamp

Trong Inspector, cấu hình:

**Cài đặt Đèn:**
- **Lamp Light:** Kéo Point Light vào đây
- **Fire Particle:** (Tùy chọn) Hiệu ứng lửa
- **Is On:** ❌ (Tắt lúc đầu, player phải thắp)

**Vùng An Toàn:**
- **Protection Radius:** `8.0` (Bán kính bảo vệ - 8 mét)
- **Safe Zone Trigger:** (Tự động tạo nếu chưa có)

**Âm thanh:**
- **Audio Source:** (Tùy chọn) AudioSource component
- **Light On SFX:** (Tùy chọn) Âm thanh khi bật đèn
- **Light Off SFX:** (Tùy chọn) Âm thanh khi tắt đèn

**Tự động thắp sáng:**
- **Auto Light On Start:** ❌ (Nếu false, player phải thắp bằng E)

---

## 🚀 BƯỚC 3: Setup Point Light cho Đèn

1. **Chọn Point Light** (child của cây đèn)
2. Trong Inspector, cấu hình:

**Light Component:**
- **Type:** Point
- **Range:** `8` (Phải khớp với Protection Radius!)
- **Color:** Vàng cam `(255, 200, 100)` hoặc `#FFC864`
- **Intensity:** `1.5` đến `2.0`
- **Shadow Type:** Soft Shadows
- **Enabled:** ❌ (Tắt lúc đầu, script sẽ tự bật)

---

## 🚀 BƯỚC 4: Tạo 3 Đèn Tĩnh

Lặp lại **Bước 2** và **Bước 3** cho **3 cây đèn**:
1. Đèn 1 (đầu đường)
2. Đèn 2 (giữa đường)
3. Đèn 3 (cuối đường)

**Tip:** Tạo Prefab để dễ setup:
1. Setup 1 đèn hoàn chỉnh
2. Kéo từ Hierarchy vào Project → Tạo Prefab
3. Kéo Prefab vào scene 2 lần nữa → Có 3 đèn!

---

## 🎮 ĐIỀU KHIỂN

### Player:
- **E** - Tương tác với đèn (bật/tắt)
- **F** - Bật/tắt đèn lồng của player

### Cơ chế:
1. **Nhìn vào đèn** → Hiển thị "Nhấn E để bật/tắt đèn"
2. **Nhấn E** → Đèn bật/tắt
3. **Ma vào vùng 8m** → Ma bị stun, không thể vào sâu hơn
4. **Player vào vùng 8m** → Stress giảm (nếu có StressManager)

---

## 🔍 DEBUG

### Scene View:
1. **Chọn đèn** → Sẽ thấy **sphere màu vàng** (vùng bảo vệ khi đèn bật)
2. **Chọn Main Camera** → Sẽ thấy **tia màu đỏ/xanh** (tia raycast)
   - **Màu xanh** = Đang nhìn vào đèn (có thể tương tác)
   - **Màu đỏ** = Không nhìn vào đèn

### Console Logs:
```
Đèn đã được bật!
PaperEnemy: Ma đã vào vùng an toàn! Khoảng cách: 7.23m
```

---

## 🐛 TROUBLESHOOTING

### Vấn đề 1: Nhấn E không có phản hồi

**Kiểm tra:**
1. ✅ **PlayerInteraction** script có gắn vào Main Camera không?
2. ✅ **Collider** của đèn có đủ lớn không?
3. ✅ **Interact Range** có đủ xa không? (thử tăng lên 5.0)
4. ✅ **Layer** có đúng không? (nếu dùng Layer riêng)

**Giải pháp:**
- Kiểm tra Console để xem có lỗi không
- Tăng **Interact Range** lên 5.0
- Đảm bảo Collider không bị che bởi Collider khác

### Vấn đề 2: Tia raycast không trúng đèn

**Kiểm tra:**
1. ✅ Collider có **Is Trigger = false** không? (Phải là false!)
2. ✅ Collider có bị Collider khác che không?
3. ✅ Đèn có nằm trong **Interactable Layer** không?

**Giải pháp:**
- Kiểm tra Collider → **Is Trigger** = ❌
- Tăng kích thước Collider
- Kiểm tra Scene view → Xem tia ray có đi qua đèn không

### Vấn đề 3: Ma vẫn vào được vùng an toàn

**Kiểm tra:**
1. ✅ Đèn có đang **bật** không? (Is On = ✅)
2. ✅ **Protection Radius** có khớp với **Range** của Light không?
3. ✅ **PaperEnemy** script có tìm thấy **StreetLamp** không?

**Giải pháp:**
- Đảm bảo **Protection Radius = Range** (ví dụ: cả 2 = 8)
- Kiểm tra Console để xem có log "Ma đã vào vùng an toàn" không

---

## 📝 CODE TÍCH HỢP

### PaperEnemy đã được cập nhật:
- ✅ Tự động kiểm tra cả **LanternSystem** (đèn lồng player) VÀ **StreetLamp** (đèn tĩnh)
- ✅ Ma sẽ bị stun nếu vào vùng an toàn của BẤT KỲ đèn nào

### StressManager tích hợp:
- ✅ Player vào vùng an toàn của đèn tĩnh → Stress giảm
- ✅ Tự động gọi `stressMgr.AddStress(-0.1f)`

---

## ✅ CHECKLIST

Sau khi setup, đảm bảo:

- [ ] **PlayerInteraction** script gắn vào Main Camera
- [ ] **StreetLamp** script gắn vào 3 cây đèn
- [ ] **Point Light** được gán vào **Lamp Light** field
- [ ] **Collider** đủ lớn và **Is Trigger = false**
- [ ] **Protection Radius = Range** của Light (ví dụ: cả 2 = 8)
- [ ] **3 đèn** đã được setup
- [ ] Nhấn E → Đèn bật/tắt
- [ ] Ma không vào được vùng an toàn khi đèn bật

---

## 🎯 KẾT QUẢ MONG ĐỢI

- ✅ Player **nhìn vào đèn** → Hiển thị "Nhấn E để bật/tắt đèn"
- ✅ **Nhấn E** → Đèn bật/tắt
- ✅ **Ma không vào được** vùng 8m khi đèn bật
- ✅ **Player vào vùng 8m** → Stress giảm
- ✅ **3 đèn** hoạt động độc lập

**Chúc bạn setup thành công! 🔦✨**



