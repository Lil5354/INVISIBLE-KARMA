# HƯỚNG DẪN SETUP PLAYER - FIRST PERSON CONTROLLER

## 🎮 TÍNH NĂNG

### FirstPersonController.cs
- ✅ Di chuyển WASD (W/S tiến/lùi, A/D trái/phải)
- ✅ Quay đầu bằng chuột (Mouse Look)
- ✅ Chạy bằng Left Shift
- ✅ Head bobbing khi di chuyển
- ✅ Breathing/Idle animation khi đứng im (trạng thái căng thẳng)
- ✅ Camera shake và heartbeat khi stress

### FootstepAudioController.cs
- ✅ Tự động phát âm thanh bước chân khi di chuyển
- ✅ Âm thanh khác nhau khi đi bộ và chạy
- ✅ Random pitch và volume để tự nhiên hơn

### StressManager.cs
- ✅ Quản lý mức độ căng thẳng tự động
- ✅ Tăng stress khi có sự kiện đáng sợ
- ✅ Tự động giảm stress theo thời gian

### SceneTestManager.cs
- ✅ Debug info hiển thị trên màn hình
- ✅ Test controls (Reset, Stress, etc.)
- ✅ Toggle cursor lock

---

## 🚀 CÁCH SETUP NHANH (3 BƯỚC)

### BƯỚC 1: Tự động Setup Player

1. Mở scene của bạn trong Unity
2. Tạo Empty GameObject, đặt tên "PlayerSetupHelper"
3. Add Component: `PlayerSetupHelper`
4. Trong Inspector, click chuột phải vào component → **"Setup Player"**
   - HOẶC tick **"Auto Setup On Start"** và chạy scene

→ Script sẽ tự động tạo Player với tất cả components cần thiết!

### BƯỚC 2: Thêm âm thanh bước chân (Tùy chọn)

1. Import audio files vào `Assets/Audio/Footsteps/`
2. Select Player GameObject
3. Trong `FootstepAudioController` component:
   - Kéo audio clips vào **"Footstep Sounds"** array
   - (Tùy chọn) Thêm audio clips vào **"Run Footstep Sounds"** cho âm thanh chạy

### BƯỚC 3: Thêm Scene Test Manager (Để test)

1. Tạo Empty GameObject, đặt tên "SceneTestManager"
2. Add Component: `SceneTestManager`
3. Chạy scene và sử dụng các phím tắt để test

---

## 🎯 SETUP THỦ CÔNG (Nếu muốn tự control)

### 1. Tạo Player GameObject

```
1. Tạo Empty GameObject → Đặt tên "Player"
2. Add Component: CharacterController
   - Height: 1.8
   - Radius: 0.3
   - Center: (0, 0.9, 0)
3. Add Component: FirstPersonController
4. Add Component: StressManager
5. Add Component: FootstepAudioController
6. Add Component: AudioSource
```

### 2. Setup Camera

```
1. Tạo Empty GameObject → Đặt tên "CameraHolder"
   - Đặt làm child của Player
   - Local Position: (0, 1.6, 0)
2. Đặt Main Camera làm child của CameraHolder
   - Local Position: (0, 0, 0)
   - Field of View: 75
```

### 3. Cấu hình FirstPersonController

Trong Inspector, bạn có thể tùy chỉnh:

**Movement Settings:**
- Walk Speed: 3 m/s
- Run Speed: 5 m/s
- Gravity: -9.81

**Mouse Look Settings:**
- Mouse Sensitivity: 2
- Vertical Look Limit: 80 độ

**Head Bobbing:**
- Bobbing Amount: 0.05
- Bobbing Speed: 10

**Breathing/Idle:**
- Breathing Intensity: 0.02
- Breathing Speed: 1.5
- Idle Sway Amount: 0.01

**Stress Effects:**
- Max Stress Shake: 0.05
- Heartbeat Intensity: 0.03

---

## 🎮 ĐIỀU KHIỂN

### Di chuyển
- **W** - Tiến
- **S** - Lùi
- **A** - Trái
- **D** - Phải
- **Left Shift** - Chạy
- **Mouse** - Quay đầu

### Test Controls (Khi có SceneTestManager)
- **R** - Reset Player về vị trí ban đầu
- **T** - Toggle Stress Test Mode (tự động tăng stress)
- **Y** - Tăng stress
- **U** - Giảm stress
- **Escape** - Toggle cursor lock/unlock
- **F1** - Toggle debug info

---

## 📝 SỬ DỤNG TRONG CODE

### Tăng stress từ script khác:

```csharp
// Cách 1: Dùng FirstPersonController
FirstPersonController player = FindObjectOfType<FirstPersonController>();
player.AddStress(0.3f); // Tăng 30% stress

// Cách 2: Dùng StressManager
StressManager stressMgr = FindObjectOfType<StressManager>();
stressMgr.TriggerJumpscare(); // Trigger jumpscare
stressMgr.OnPaperFigureMoved(); // Khi hình nhân di chuyển
stressMgr.StartChase(); // Khi bắt đầu chase
```

### Kiểm tra trạng thái player:

```csharp
FirstPersonController player = FindObjectOfType<FirstPersonController>();

if (player.IsMoving())
{
    Debug.Log("Player đang di chuyển");
}

float speed = player.GetCurrentSpeed();
float stress = player.GetStressLevel();
```

---

## 🔧 TROUBLESHOOTING

### Vấn đề: Camera không quay được
- ✅ Kiểm tra CameraHolder đã được tạo chưa
- ✅ Kiểm tra Main Camera là child của CameraHolder
- ✅ Kiểm tra Cursor.lockState = Locked (nhấn Escape để toggle)

### Vấn đề: Không di chuyển được
- ✅ Kiểm tra CharacterController đã được add chưa
- ✅ Kiểm tra Player có đang ở trên mặt đất không (Ground Check)
- ✅ Kiểm tra Input Manager settings (WASD keys)

### Vấn đề: Không có âm thanh bước chân
- ✅ Kiểm tra FootstepAudioController đã được add chưa
- ✅ Kiểm tra AudioSource component có trên Player không
- ✅ Thêm audio clips vào Footstep Sounds array

### Vấn đề: Head bobbing quá mạnh/yếu
- ✅ Điều chỉnh "Bobbing Amount" trong FirstPersonController
- ✅ Điều chỉnh "Bobbing Speed" để thay đổi tốc độ

---

## 📦 CẤU TRÚC THƯ MỤC

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── FirstPersonController.cs
│   │   ├── FootstepAudioController.cs
│   │   ├── StressManager.cs
│   │   └── PlayerSetupHelper.cs
│   └── Gameplay/
│       └── SceneTestManager.cs
├── Audio/
│   └── Footsteps/
│       ├── footstep_01.ogg
│       ├── footstep_02.ogg
│       └── ...
└── Scenes/
    └── TestScene.unity
```

---

## ✅ CHECKLIST TRƯỚC KHI TEST

- [ ] Player GameObject đã được tạo
- [ ] CharacterController đã được add và cấu hình
- [ ] FirstPersonController đã được add
- [ ] Camera đã được setup đúng (child của CameraHolder)
- [ ] StressManager đã được add (tùy chọn)
- [ ] FootstepAudioController đã được add và có audio clips (tùy chọn)
- [ ] SceneTestManager đã được add để test (tùy chọn)
- [ ] Cursor đã được lock (nhấn Escape để toggle)

---

## 🎨 TIPS

1. **Head Bobbing**: Điều chỉnh "Bobbing Amount" từ 0.03-0.08 để có cảm giác tự nhiên
2. **Stress Effects**: Tăng "Max Stress Shake" lên 0.1-0.15 để có hiệu ứng mạnh hơn
3. **Mouse Sensitivity**: Điều chỉnh từ 1-3 tùy sở thích
4. **Footstep Audio**: Sử dụng 3-5 audio clips khác nhau để tự nhiên hơn

---

## 📞 HỖ TRỢ

Nếu gặp vấn đề, kiểm tra:
1. Console logs trong Unity
2. Debug info từ SceneTestManager (nhấn F1)
3. Các components đã được add đầy đủ chưa

**Chúc bạn test vui vẻ! 🎮**










