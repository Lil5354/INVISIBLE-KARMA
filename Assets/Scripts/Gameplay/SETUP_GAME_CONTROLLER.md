# 🎮 HƯỚNG DẪN SETUP GAME CONTROLLER & GAME OVER

## 📋 TỔNG QUAN

Hệ thống Game Controller quản lý:
- ✅ **Delay 3 giây** trước khi ma bắt đầu đuổi
- ✅ **Game Over** khi ma chạm vào player
- ✅ **UI "YOU LOSE"** hiển thị khi thua
- ✅ **Tự động reload scene** sau 2 giây

---

## 🚀 BƯỚC 1: Setup GameController

### 1.1. Tạo GameManager GameObject

1. Trong Hierarchy, **chuột phải** → **Create Empty**
2. Đặt tên: **`GameManager`**
3. **Add Component** → **Game Controller**

### 1.2. Cấu hình GameController

Trong Inspector, cấu hình:

**Cài đặt UI:**
- **Lose Panel:** (Sẽ gán sau khi tạo UI)
- **Lose Text:** (Tùy chọn) Text component
- **Lose Display Time:** `2.0` (Thời gian hiển thị "YOU LOSE" trước khi reload)

**Cài đặt Game:**
- **Auto Find Player Start:** ✅ (Tự động lấy vị trí player khi Start)
- **Player Start Position:** (Tự động set)

**References:**
- **Player:** (Tự động tìm nếu để trống)

---

## 🚀 BƯỚC 2: Tạo UI "YOU LOSE"

### 2.1. Tạo Canvas

1. Trong Hierarchy, **chuột phải** → **UI** → **Canvas**
2. Canvas sẽ tự động tạo với:
   - **Canvas** (component chính)
   - **EventSystem** (để xử lý input)
   - **GraphicRaycaster** (để raycast UI)

### 2.2. Tạo Panel "LosePanel"

1. **Chuột phải** vào **Canvas** → **UI** → **Panel**
2. Đặt tên: **`LosePanel`**
3. Trong Inspector, cấu hình:

**Rect Transform:**
- **Anchor Presets:** Nhấn **Alt + Shift** và chọn **Stretch-Stretch** (full screen)
- **Left, Right, Top, Bottom:** `0` (full screen)

**Image Component:**
- **Color:** Đen với Alpha `200` (mờ mờ, rùng rợn)
- Hoặc đỏ với Alpha `180`

### 2.3. Tạo Text "YOU LOSE"

1. **Chuột phải** vào **LosePanel** → **UI** → **Text - Legacy** (hoặc **TextMeshPro**)
2. Đặt tên: **`LoseText`**
3. Trong Inspector, cấu hình:

**Rect Transform:**
- **Anchor Presets:** **Middle-Center**
- **Pos X, Pos Y:** `0, 0` (giữa màn hình)
- **Width:** `600`
- **Height:** `100`

**Text Component:**
- **Text:** `BẠN ĐÃ CHẾT` hoặc `YOU LOSE`
- **Font Size:** `72` hoặc `80` (to, dễ nhìn)
- **Alignment:** **Center** (căn giữa)
- **Color:** **Đỏ** hoặc **Trắng** (nổi bật)
- **Font Style:** **Bold** (đậm)

**Tùy chọn thêm:**
- Add Component → **Outline** hoặc **Shadow**
- **Effect Color:** Đen
- **Effect Distance:** `(3, -3)`

### 2.4. Tắt LosePanel lúc đầu

1. **Chọn LosePanel** trong Hierarchy
2. **Bỏ tích** ô **Active** (góc trên bên trái Inspector)
3. → Panel sẽ ẩn lúc đầu game, chỉ hiện khi Game Over

### 2.5. Gán vào GameController

1. **Chọn GameManager** trong Hierarchy
2. Trong **Game Controller** component:
   - Kéo **LosePanel** từ Hierarchy vào ô **Lose Panel**
   - (Tùy chọn) Kéo **LoseText** vào ô **Lose Text**

---

## 🚀 BƯỚC 3: Setup Player Tag & Collider

### 3.1. Đặt Tag cho Player

1. **Chọn Player** trong Hierarchy
2. Ở phần **Tag** (góc trên bên phải Inspector):
   - Đổi từ `Untagged` thành **`Player`**
   - ⚠️ **Rất quan trọng!** Nếu không, ma sẽ không biết đâu là player

### 3.2. Kiểm tra Collider của Player

1. **Chọn Player** trong Hierarchy
2. Kiểm tra có **CharacterController** hoặc **Collider**:
   - **CharacterController** → Đã có sẵn (OK)
   - Hoặc **Capsule Collider** / **Box Collider**

---

## 🚀 BƯỚC 4: Setup Enemy Collider

### 4.1. Thêm Collider cho Enemy

1. **Chọn Enemy** (PaperEnemy) trong Hierarchy
2. Kiểm tra có **Collider** chưa:
   - Nếu chưa có → **Add Component** → **Box Collider** hoặc **Capsule Collider**
   - Chỉnh kích thước Collider to ra một chút (để dễ chạm player)

### 4.2. Cấu hình Collider

**Quan trọng:** Có 2 cách setup:

**Cách 1: Dùng Trigger (Khuyên dùng)**
- **Is Trigger:** ✅ **TÍCH** (để ma đi xuyên qua player một chút rồi mới kích hoạt)
- → Sử dụng `OnTriggerEnter()` trong code

**Cách 2: Dùng Collision**
- **Is Trigger:** ❌ **BỎ TÍCH** (ma sẽ đẩy player)
- → Sử dụng `OnCollisionEnter()` trong code

**Khuyên dùng Cách 1** (Trigger) vì:
- Ma có thể đi xuyên qua player một chút → Cảm giác rùng rợn hơn
- Không bị đẩy lùi → Gameplay mượt hơn

---

## 🚀 BƯỚC 5: Cấu hình PaperEnemy

### 5.1. Kiểm tra Start Delay

1. **Chọn Enemy** trong Hierarchy
2. Trong **Paper Enemy** component:
   - **Start Delay:** `3.0` (3 giây delay - đã set mặc định)
   - Có thể thay đổi nếu muốn

### 5.2. Test Delay

1. **Play Scene**
2. **Quan sát Console:**
   - Sau 3 giây → Log: "PaperEnemy: Ma bắt đầu đi săn!"
3. **Ma sẽ đứng yên** trong 3 giây đầu

---

## 🎮 CÁCH HOẠT ĐỘNG

### Timeline:

1. **0 giây:** Game bắt đầu → Ma đứng yên
2. **3 giây:** Ma bắt đầu đuổi → Log "Ma bắt đầu đi săn!"
3. **Ma chạm player:** 
   - Game dừng lại (`Time.timeScale = 0`)
   - Hiện bảng "YOU LOSE"
   - Đợi 2 giây
4. **Reload scene:** Tự động load lại từ đầu

---

## 🔍 DEBUG

### Console Logs:

Khi game bắt đầu:
```
PaperEnemy: Đã tìm thấy Player!
```

Sau 3 giây:
```
PaperEnemy: Ma bắt đầu đi săn!
```

Khi ma chạm player:
```
PaperEnemy: Ma đã bắt được Player!
Game Over! Player bị bắt!
```

### Kiểm tra trong Inspector:

1. **Chọn Enemy:**
   - **Can Move** (private) → Sẽ là `false` trong 3 giây đầu
   - Sau 3 giây → `true`

2. **Chọn GameManager:**
   - **Game Over** (private) → `false` lúc đầu
   - Khi thua → `true`

---

## 🐛 TROUBLESHOOTING

### Vấn đề 1: Ma không đợi 3 giây

**Kiểm tra:**
1. ✅ **Start Delay** có = 3.0 không?
2. ✅ Console có log "Ma bắt đầu đi săn!" không?

**Giải pháp:**
- Kiểm tra Console để xem có lỗi không
- Đảm bảo Coroutine không bị dừng

### Vấn đề 2: Ma chạm player nhưng không Game Over

**Kiểm tra:**
1. ✅ Player có **Tag = "Player"** không?
2. ✅ Enemy có **Collider** không?
3. ✅ **Is Trigger** có đúng không? (Trigger = true nếu dùng OnTriggerEnter)
4. ✅ **GameController** có trong scene không?

**Giải pháp:**
- Kiểm tra Tag của Player (phải là "Player")
- Kiểm tra Collider của Enemy
- Kiểm tra Console để xem có log "Ma đã bắt được Player!" không

### Vấn đề 3: UI "YOU LOSE" không hiện

**Kiểm tra:**
1. ✅ **LosePanel** có được gán vào GameController không?
2. ✅ **LosePanel** có bị tắt (Active = false) lúc đầu không?
3. ✅ Console có log "Game Over!" không?

**Giải pháp:**
- Kiểm tra LosePanel có được gán vào GameController
- Đảm bảo LosePanel bị tắt lúc đầu (Active = false)
- Kiểm tra Console để xem có lỗi không

### Vấn đề 4: Game không reload

**Kiểm tra:**
1. ✅ **Lose Display Time** có > 0 không?
2. ✅ Console có log gì không?

**Giải pháp:**
- Kiểm tra Lose Display Time = 2.0
- Đảm bảo không có lỗi trong Console

---

## 📝 CODE TÍCH HỢP

### PaperEnemy đã được cập nhật:
- ✅ **StartDelayRoutine()** - Coroutine đếm ngược 3 giây
- ✅ **canMove** - Biến cờ kiểm tra có được phép di chuyển không
- ✅ **OnTriggerEnter()** - Xử lý khi chạm player (dùng Trigger)
- ✅ **OnCollisionEnter()** - Xử lý khi chạm player (dùng Collision)
- ✅ Kiểm tra `GameController.instance.IsGameOver()` để dừng di chuyển

### GameController:
- ✅ **Singleton pattern** - Gọi từ bất cứ đâu bằng `GameController.instance`
- ✅ **GameOver()** - Hiện UI, dừng thời gian, reload scene
- ✅ **ResetPlayer()** - Reset player về vị trí ban đầu (không reload scene)

---

## ✅ CHECKLIST

Sau khi setup, đảm bảo:

- [ ] **GameManager** GameObject đã được tạo
- [ ] **Game Controller** script đã được gắn
- [ ] **LosePanel** đã được tạo và gán vào GameController
- [ ] **LosePanel** bị tắt lúc đầu (Active = false)
- [ ] **Player** có Tag = "Player"
- [ ] **Enemy** có Collider với Is Trigger = true
- [ ] **Start Delay** = 3.0 trong PaperEnemy
- [ ] Ma đợi 3 giây trước khi đuổi
- [ ] Ma chạm player → Game Over → Reload scene

---

## 🎯 KẾT QUẢ MONG ĐỢI

- ✅ **0-3 giây:** Ma đứng yên, player có thời gian chuẩn bị
- ✅ **Sau 3 giây:** Ma bắt đầu đuổi
- ✅ **Ma chạm player:** Game dừng, hiện "YOU LOSE"
- ✅ **Sau 2 giây:** Tự động reload scene

**Chúc bạn setup thành công! 🎮✨**


