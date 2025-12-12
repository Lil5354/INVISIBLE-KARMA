# 👻 HƯỚNG DẪN SETUP HỆ THỐNG JUMP SCARE (MA XUẤT HIỆN)

## 📋 Tổng quan

Hệ thống này tạo ra hiệu ứng Jump Scare kinh điển:
- **Đi tới đèn** → **Quay đầu lại** → **Thấy ma** → **Ma đuổi khi không nhìn**

---

## ✅ BƯỚC 1: Chuẩn bị "Khuôn đúc" Ma (Prefab)

Để ma xuất hiện bất ngờ, bạn cần biến nó thành Prefab:

1. **Chọn con ma** (`hinhnhan` hoặc `hinhnhan1`, `hinhnhan2`, etc.) trong Hierarchy
   - Đảm bảo ma đã có:
     - ✅ **Collider** (Box Collider hoặc Mesh Collider)
     - ✅ **Rigidbody** (để vật lý hoạt động)
     - ✅ **Script `PaperEnemy`** đã được gắn

2. **Tạo thư mục Prefabs:**
   - Trong Project window, chuột phải vào `Assets`
   - Chọn **Create** → **Folder**
   - Đặt tên: **`Prefabs`**

3. **Tạo Prefab:**
   - Kéo con ma từ **Hierarchy** thả vào thư mục **`Prefabs`**
   - Tên con ma trong Hierarchy sẽ chuyển sang **màu xanh dương** (đã là Prefab)

4. **Xóa ma khỏi Scene:**
   - Chọn con ma trong Hierarchy
   - Nhấn **Delete** (hoặc chuột phải → **Delete**)
   - ✅ Yên tâm, nó đã được lưu trong Prefabs rồi!

---

## ✅ BƯỚC 2: Cấu hình PaperEnemy Script

1. **Mở Prefab ma:**
   - Trong Project, click vào Prefab ma trong thư mục `Prefabs`
   - Inspector sẽ hiện các component của Prefab

2. **Kiểm tra PaperEnemy Script:**
   - Tìm component **Paper Enemy (Script)**
   - **Player**: Để trống (script sẽ tự tìm)
   - **Chase Speed**: `3.5` (tốc độ đuổi theo)
   - **Stop Distance**: `1.0` (dừng lại khi quá gần)

3. **Đảm bảo có các component:**
   - ✅ **Renderer** (Mesh Renderer) - để kiểm tra visibility
   - ✅ **Rigidbody** - để vật lý hoạt động
   - ✅ **Collider** - để tương tác với đèn

---

## ✅ BƯỚC 3: Tạo Bẫy Xuất Hiện (HauntTrigger)

### 3.1. Tạo GameObject Trigger

1. **Tạo Cube:**
   - Menu: **GameObject** → **3D Object** → **Cube**
   - Đặt tên: **`BayMa_1`**

2. **Đặt vị trí:**
   - Kéo `BayMa_1` ra đặt trên đường đi
   - Cách đèn khoảng **10-15 mét** về phía trước
   - Đặt ở độ cao ngang với Player (Y = 1-2)

3. **Làm tàng hình:**
   - Chọn `BayMa_1`
   - Trong Inspector, tìm **Mesh Renderer**
   - **Bỏ tích** ô **Mesh Renderer** (hoặc disable component)
   - ✅ Cube giờ đã tàng hình!

4. **Cấu hình Collider:**
   - Tìm **Box Collider**
   - ✅ **Tích vào** ô **Is Trigger**
   - Điều chỉnh **Size** nếu cần:
     - X: `5` (rộng đường)
     - Y: `3` (cao)
     - Z: `2` (dày)

### 3.2. Gắn Script HauntTrigger

1. **Add Component:**
   - Chọn `BayMa_1`
   - Click **Add Component**
   - Tìm và chọn: **Haunt Trigger**

2. **Cấu hình Script:**
   - **Enemy Prefab**: Kéo Prefab ma từ thư mục `Prefabs` vào đây
   - **Spawn Distance Behind**: `5` hoặc `8` (mét)
     - ⚠️ Đừng để xa quá, kẻo người chơi quay lại không thấy
   - **Scare Sound**: (Tùy chọn) Kéo AudioSource vào nếu muốn có âm thanh

---

## ✅ BƯỚC 4: Kiểm tra Player Tag

1. **Chọn Player** trong Hierarchy
2. **Kiểm tra Tag:**
   - Ở góc trên Inspector, tìm **Tag**
   - Đảm bảo Tag = **`Player`**
   - Nếu chưa đúng:
     - Click vào Tag → Chọn **Player**
     - Nếu không có tag Player:
       - Click **Add Tag...**
       - Dấu **+** → Đặt tên: **`Player`**
       - Quay lại chọn Player → Tag → Chọn **Player**

---

## ✅ BƯỚC 5: Test Gameplay

1. **Bấm Play**
2. **Đi bộ** trên đường rừng (lúc này chưa có ma)
3. **Đi qua** cái Cube tàng hình (`BayMa_1`)
   - ✅ Code sẽ tự động tạo ma sau lưng bạn (cách 5-8 mét)
4. **Tiếp tục đi** về phía đèn (chưa biết gì)
5. **Quay đầu lại** → 👻 **Con ma đang đứng đó!**
6. **Nhìn vào ma** → Ma đứng im
7. **Quay đi chạy** → Ma bắt đầu đuổi theo
8. **Chạy đến đèn** → Bấm **E** bật đèn
9. **Vùng sáng hiện ra** → Ma dừng lại, không dám vào

---

## 🎨 Tùy chỉnh nâng cao

### Tạo nhiều bẫy:

1. **Duplicate bẫy:**
   - Chọn `BayMa_1`
   - Nhấn **Ctrl + D** (Windows) hoặc **Cmd + D** (Mac)
   - Đặt tên: `BayMa_2`, `BayMa_3`, etc.

2. **Đặt vị trí:**
   - Rải rác dọc đường đi
   - Mỗi bẫy cách nhau **20-30 mét**

3. **Gắn Prefab:**
   - Mỗi bẫy kéo Prefab ma vào (có thể dùng cùng Prefab)

### Tùy chỉnh AI Ma:

Trong **PaperEnemy** script:
- **Chase Speed**: Tăng để ma đuổi nhanh hơn (nguy hiểm hơn)
- **Stop Distance**: Giảm để ma đến gần hơn trước khi dừng

### Thêm âm thanh:

1. **Tạo AudioSource:**
   - Chọn `BayMa_1`
   - **Add Component** → **Audio Source**
   - Kéo file âm thanh vào **Audio Clip**

2. **Gắn vào script:**
   - Kéo AudioSource vào ô **Scare Sound** trong HauntTrigger

---

## 🐛 Troubleshooting

**Ma không xuất hiện:**
- ✅ Kiểm tra Prefab đã được gán vào `Enemy Prefab` chưa
- ✅ Kiểm tra Player có Tag = "Player" chưa
- ✅ Kiểm tra Box Collider có **Is Trigger** = true chưa
- ✅ Kiểm tra `hasTriggered` = false (nếu đã trigger rồi thì không trigger nữa)

**Ma không đuổi theo:**
- ✅ Kiểm tra PaperEnemy script có được gắn vào Prefab chưa
- ✅ Kiểm tra Prefab có Renderer (Mesh Renderer) chưa
- ✅ Kiểm tra Prefab có Rigidbody chưa

**Ma không dừng khi bị nhìn:**
- ✅ Kiểm tra Prefab có Renderer component chưa
- ✅ Renderer phải có mesh được render (không bị disable)

**Ma không sợ đèn:**
- ✅ Kiểm tra đèn có Sphere Collider với **Is Trigger** = true chưa
- ✅ Kiểm tra đèn có script `Lantern` chưa
- ✅ Kiểm tra đèn đã được bật (isLit = true) chưa

---

## 🎮 Gameplay Loop Hoàn Chỉnh

1. **Đi bộ** → Chưa có ma
2. **Đi qua trigger** → Ma xuất hiện sau lưng
3. **Quay đầu lại** → Thấy ma (Jump Scare!)
4. **Nhìn vào ma** → Ma đứng im
5. **Quay đi chạy** → Ma đuổi theo
6. **Chạy đến đèn** → Bấm E
7. **Đèn sáng** → Ma dừng lại, an toàn!

---

**Xong rồi!** Bây giờ bạn đã có hệ thống Jump Scare hoàn chỉnh! 👻✨








