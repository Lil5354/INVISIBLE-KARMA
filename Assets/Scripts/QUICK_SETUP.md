# 🚀 HƯỚNG DẪN SETUP NHANH

## CÁCH 1: Setup từ Context Menu (DỄ NHẤT) ⭐⭐⭐

### Bước 1: Tạo Empty GameObject
- Trong Hierarchy, click chuột phải → **Create Empty**
- Đặt tên: `QuickSetup` (hoặc tên bất kỳ)

### Bước 2: Add Component
- Chọn GameObject vừa tạo
- Trong Inspector, click **Add Component**
- Tìm và chọn: **Quick Setup Button**

### Bước 3: Click chuột phải vào component
- Click chuột phải vào component **Quick Setup Button** trong Inspector
- Chọn **"Setup Player & Test Manager"** từ context menu

→ Script sẽ tự động:
- ✅ Tạo PlayerSetupHelper
- ✅ Setup Player với tất cả components
- ✅ Tạo SceneTestManager
- ✅ Sẵn sàng để test!

### Bước 4: Play Scene và Test

---

## CÁCH 2: Tự động Setup từ Menu Unity

### Bước 1: Mở Unity Editor và Scene của bạn

### Bước 2: Chọn menu **Tools > Auto Setup Player & Test Manager**

**Lưu ý:** Nếu không thấy menu Tools, hãy:
1. Đợi Unity compile scripts (xem Console để kiểm tra lỗi)
2. Hoặc dùng **CÁCH 1** ở trên (dễ hơn)

→ Script sẽ tự động:
- ✅ Tạo PlayerSetupHelper
- ✅ Setup Player với tất cả components
- ✅ Tạo SceneTestManager
- ✅ Sẵn sàng để test!

### Bước 3: Play Scene và Test

**Điều khiển:**
- **WASD** - Di chuyển
- **Mouse** - Quay đầu
- **Left Shift** - Chạy
- **F1** - Toggle debug info
- **Escape** - Toggle cursor

**Test Controls (khi có SceneTestManager):**
- **R** - Reset Player
- **T** - Toggle Stress Test Mode
- **Y** - Tăng stress
- **U** - Giảm stress

---

## CÁCH 2: Tự động Setup khi Play Scene

### Bước 1: Chọn menu **Tools > Add Auto Setup On Scene Load**

→ Script sẽ tự động tạo GameObject "AutoSetupOnSceneLoad"

### Bước 2: Play Scene

→ Player và SceneTestManager sẽ được tạo tự động khi scene bắt đầu!

---

## CÁCH 3: Setup từng phần riêng lẻ

### Setup chỉ Player:
**Tools > Setup Player Only**

### Setup chỉ Test Manager:
**Tools > Setup Test Manager Only**

---

## 📝 LƯU Ý

1. **Cursor Lock**: Khi Play, cursor sẽ tự động lock. Nhấn **Escape** để unlock.

2. **Debug Info**: Nhấn **F1** để bật/tắt thông tin debug ở góc trái màn hình.

3. **Console**: Kiểm tra Console (Window > General > Console) để xem log khi setup.

4. **Scene Changes**: Sau khi setup, nhớ **Save Scene** (Ctrl+S) để lưu thay đổi.

---

## ✅ CHECKLIST

Sau khi setup, bạn sẽ có:
- [x] GameObject "Player" với CharacterController
- [x] FirstPersonController component
- [x] CameraHolder với Main Camera
- [x] StressManager component
- [x] FootstepAudioController component
- [x] AudioSource component
- [x] SceneTestManager (nếu dùng Auto Setup)

---

## 🎮 SẴN SÀNG TEST!

Bây giờ bạn có thể Play scene và test ngay!

**Chúc bạn test vui vẻ! 🎮**

