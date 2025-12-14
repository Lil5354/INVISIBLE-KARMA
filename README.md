# 🎮 INVISIBLE KARMA

Game kinh dị góc nhìn thứ nhất (First-Person Horror Game) với cơ chế đuổi bắt và quản lý ánh sáng.

## 📋 TỔNG QUAN

**INVISIBLE KARMA** là game kinh dị nơi người chơi phải:
- Tránh các hình nhân giấy (Paper Enemies) đuổi theo
- Quản lý đèn lồng dầu (Lantern) để tạo vùng an toàn
- Bật các đèn đường (Street Lamps) để cản kẻ địch
- Giữ 3 đèn đường và đèn lồng cá nhân sáng đến cuối màn để chiến thắng

---

## 🎯 TÍNH NĂNG CHÍNH

### 🧍 Player System
- **First Person Controller**: Di chuyển, quay đầu, head bobbing
- **Lantern System**: Đèn lồng dầu với hệ thống nhiên liệu
- **Player Interaction**: Tương tác với đèn đường (E để bật/tắt, R để sạc dầu)
- **Stress Manager**: Hệ thống căng thẳng ảnh hưởng đến gameplay

### 👻 Enemy System
- **Paper Enemy AI**: Đuổi theo player, dừng khi bị nhìn thấy
- **Light Stunning**: Bị choáng khi bị ánh sáng chiếu vào
- **Safe Zone Detection**: Không thể vào vùng an toàn của đèn lồng
- **Game Over Trigger**: Chạm vào player → Game Over

### 🏮 Street Lamp System
- **Toggle Light**: Bật/tắt đèn đường (phím E)
- **Oil Refill**: Sạc dầu cho đèn lồng player (phím R)
- **Fire Effect**: Hiệu ứng lửa khi đèn sáng
- **Safe Zone**: Tạo vùng an toàn cho player

### 🎮 Game Management
- **Main Menu**: Menu chính với nút Play, Exit, Options
- **Game Controller**: Quản lý Game Over, scene reload
- **Scene Management**: Chuyển cảnh giữa MainMenu và Chapter1

---

## 🚀 HƯỚNG DẪN SETUP

### 📍 BƯỚC 1: Setup Main Menu

#### 1.1. Tạo MenuManager GameObject

1. Mở scene **MainMenu**
2. Hierarchy → Chuột phải → **Create Empty**
3. Đặt tên: **`MenuManager`**
4. Inspector → **Add Component** → **Main Menu Controller**

#### 1.2. Cấu hình MainMenuController

1. Chọn **MenuManager** trong Hierarchy
2. Inspector → **Main Menu Controller**:
   - **Chapter1 Scene Index**: `1` (MainMenu = 0, Chapter1 = 1)
   - **Options Panel**: (Tùy chọn) Kéo Panel Options vào đây nếu có

#### 1.3. Gắn chức năng cho các nút

**Nút PLAY (Btn_Start):**
1. Chọn **Btn_Start** trong Hierarchy
2. Inspector → **Button** → **On Click ()**
3. Click dấu **`+`** → Kéo **MenuManager** vào ô **None (Object)**
4. Dropdown → **MainMenuController** → **PlayGame()**

**Nút EXIT (Btn_Exit):**
1. Chọn **Btn_Exit** trong Hierarchy
2. Làm tương tự → Chọn **QuitGame()**

**Nút OPTION (Btn_Option):**
1. Chọn **Btn_Option** trong Hierarchy
2. Làm tương tự → Chọn **OpenOptions()**

#### 1.4. Đăng ký Scenes trong Build Settings (BẮT BUỘC)

1. **File** → **Build Settings**
2. Kéo scene **MainMenu** vào danh sách (Index 0)
3. Kéo scene **Chapter1** vào danh sách (Index 1)
4. Đảm bảo cả 2 scene đều được tích ✅

**Thứ tự quan trọng:**
- Index 0: MainMenu (màn hình đầu tiên)
- Index 1: Chapter1 (màn chơi chính)

---

### 📍 BƯỚC 2: Setup Player

#### 2.1. Setup Player GameObject

1. Mở scene **Chapter1**
2. Chọn **Player** trong Hierarchy
3. Đảm bảo có các component:
   - **Character Controller**
   - **First Person Controller** (hoặc **Player Controller**)
   - **Lantern System**
   - **Player Interaction**

#### 2.2. Setup Lantern System

1. Chọn **Player** → Inspector → **Lantern System**:
   - **Max Oil**: `100` (dầu tối đa)
   - **Oil Consumption Rate**: `5` (tốc độ tiêu dầu)
   - **Lantern Light**: Kéo **Point Light** vào đây
   - **Safe Zone Radius**: `5` (bán kính vùng an toàn)

2. Đảm bảo có **Point Light** con của Player:
   - Tạo **Point Light** → Đặt làm con của Player
   - Range: `10-15`
   - Intensity: `1.5-2`

#### 2.3. Setup Player Interaction

1. Chọn **Main Camera** (con của Player)
2. Inspector → **Add Component** → **Player Interaction**
3. Cấu hình:
   - **Interact Range**: `5.0`
   - **Interact Key**: `E` (bật/tắt đèn đường)
   - **Refill Key**: `R` (sạc dầu)
   - **My Lantern**: Kéo **Player** vào đây

---

### 📍 BƯỚC 3: Setup Street Lamps

#### 3.1. Setup Street Lamp GameObject

1. Chọn đối tượng đèn đường (VD: **Latern1**)
2. Inspector → **Add Component** → **Street Lamp**
3. Cấu hình:
   - **Lamp Light**: Kéo **Point Light** của đèn vào đây
   - **Fire Particle**: Kéo **Particle System** (hiệu ứng lửa) vào đây
   - **Can Refill Oil**: ✅ (cho phép sạc dầu)
   - **Oil Refill Amount**: `50` (lượng dầu sạc mỗi lần)
   - **Refill Cooldown**: `5` (thời gian chờ giữa các lần sạc)

4. Đảm bảo có **Collider** (Box Collider hoặc Sphere Collider):
   - **Is Trigger**: ❌ (phải tắt để raycast trúng)

#### 3.2. Tự động Setup (Editor Tool)

1. Chọn đối tượng đèn đường
2. Inspector → **Add Component** → **Street Lamp Setup Helper**
3. Click nút **"Setup Street Lamp"** trong Inspector
4. Tool sẽ tự động:
   - Gắn script **Street Lamp**
   - Tìm và gán **Point Light**
   - Tìm và gán **Particle System**
   - Thêm **Collider** nếu chưa có

---

### 📍 BƯỚC 4: Setup Enemies

#### 4.1. Setup Paper Enemy

1. Chọn đối tượng enemy (VD: **hinhnhan1**)
2. Inspector → **Add Component** → **Paper Enemy**
3. Cấu hình:
   - **Player**: Kéo **Player** vào đây
   - **Lantern System**: Kéo **Player** (có LanternSystem) vào đây
   - **Move Speed**: `1.5`
   - **Start Delay**: `3.0` (chờ 3 giây trước khi bắt đầu đuổi)
   - **Catch Distance**: `1.0` (khoảng cách bắt player)

4. Đảm bảo có **Collider**:
   - **Is Trigger**: ✅ (để phát hiện va chạm với player)

#### 4.2. Setup Game Over System

1. Tạo **Empty GameObject** → Đặt tên: **GameManager**
2. Inspector → **Add Component** → **Game Controller**
3. Tạo UI Panel "You Lose":
   - **GameObject** → **UI** → **Canvas**
   - **Canvas** → Chuột phải → **UI** → **Panel** → Đặt tên: **LosePanel**
   - **LosePanel** → Chuột phải → **UI** → **Text** → Gõ: "YOU LOSE"
   - Tắt tích **Active** của **LosePanel** (ẩn lúc đầu)
4. **GameManager** → Inspector → **Game Controller**:
   - **Lose Panel**: Kéo **LosePanel** vào đây

5. Đảm bảo **Player** có **Tag**: `Player`
   - Chọn **Player** → Inspector → **Tag** → Chọn **Player**

---

## 🎮 ĐIỀU KHIỂN

### Phím điều khiển Player:
- **WASD**: Di chuyển
- **Mouse**: Quay đầu
- **Shift**: Chạy
- **F**: Bật/tắt đèn lồng
- **E**: Tương tác với đèn đường (bật/tắt)
- **R**: Sạc dầu từ đèn đường (khi đèn đã bật)

### Phím Debug (nếu có SceneTestManager):
- **F1**: Bật/tắt thông tin debug
- **R**: Reset player về vị trí ban đầu
- **T**: Bật/tắt chế độ test stress
- **Escape**: Bật/tắt con trỏ chuột

---

## ⚠️ TROUBLESHOOTING

### ❌ Lỗi: "Scene 'Chapter1' couldn't be loaded"

**Nguyên nhân:** Scene chưa được thêm vào Build Settings

**Giải pháp:**
1. **File** → **Build Settings**
2. Kéo scene **Chapter1** vào danh sách "Scenes In Build"
3. Đảm bảo scene có index >= 0

### ❌ Lỗi: Nhấn E không bật đèn đường

**Nguyên nhân:**
- Raycast không trúng collider
- Script StreetLamp chưa được gắn
- Collider bị set Is Trigger = true

**Giải pháp:**
1. Kiểm tra **Collider** của đèn đường → **Is Trigger** phải = ❌
2. Đảm bảo script **Street Lamp** đã được gắn
3. Kiểm tra **Interact Range** trong **Player Interaction** (tăng lên 5-6 nếu cần)

### ❌ Lỗi: Sạc dầu không hoạt động

**Nguyên nhân:**
- Đèn đường chưa bật
- Cooldown chưa hết
- Dầu đã đầy

**Giải pháp:**
1. Đảm bảo đèn đường đã bật (nhấn E trước)
2. Kiểm tra Console để xem log debug
3. Đảm bảo **Can Refill Oil** = ✅ trong **Street Lamp**

### ❌ Lỗi: Ma vào được vùng an toàn

**Nguyên nhân:**
- Đèn lồng chưa bật
- Safe zone radius quá nhỏ
- LanternSystem có nhiều instance (duplicate)

**Giải pháp:**
1. Nhấn **F** để bật đèn lồng
2. Kiểm tra **Lantern System** → **Safe Zone Radius** (nên >= 5)
3. Xóa các **LanternSystem** duplicate (chỉ giữ 1 trên Player)

### ❌ Lỗi: Game Over không hiện "YOU LOSE"

**Nguyên nhân:**
- Player chưa có Tag "Player"
- LosePanel chưa được gán vào GameController
- OnTriggerEnter không hoạt động

**Giải pháp:**
1. Chọn **Player** → Inspector → **Tag** → Chọn **Player**
2. **GameManager** → **Game Controller** → Kéo **LosePanel** vào
3. Đảm bảo enemy có **Collider** với **Is Trigger** = ✅

---

## 📁 CẤU TRÚC THƯ MỤC

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── FirstPersonController.cs    # Điều khiển player
│   │   ├── LanternSystem.cs            # Hệ thống đèn lồng
│   │   ├── PlayerInteraction.cs        # Tương tác với đèn đường
│   │   └── StressManager.cs             # Quản lý căng thẳng
│   ├── Gameplay/
│   │   ├── MainMenuController.cs       # Điều khiển menu chính
│   │   ├── GameController.cs           # Quản lý game state
│   │   ├── PaperEnemy.cs                # AI enemy
│   │   └── StreetLamp.cs                # Đèn đường
│   └── Editor/
│       └── StreetLampSetupHelper.cs     # Tool tự động setup đèn
├── Scenes/
│   ├── MainMenu.unity                   # Scene menu chính
│   └── Chapter1.unity                  # Scene màn chơi chính
└── ...
```

---

## 🔧 YÊU CẦU HỆ THỐNG

- **Unity Version**: 2022.3.62f3 hoặc mới hơn
- **Platform**: Windows, Mac, Linux
- **Render Pipeline**: Built-in Render Pipeline

---

## 📝 LƯU Ý QUAN TRỌNG

1. **Build Settings**: Phải thêm cả MainMenu và Chapter1 vào Build Settings
2. **Scene Index**: MainMenu = 0, Chapter1 = 1 (quan trọng cho MainMenuController)
3. **Player Tag**: Player phải có Tag "Player" để enemy phát hiện
4. **Collider Settings**: 
   - Đèn đường: Is Trigger = ❌ (để raycast trúng)
   - Enemy: Is Trigger = ✅ (để phát hiện va chạm)
5. **LanternSystem**: Chỉ nên có 1 instance trên Player, xóa các duplicate

---

## 🎯 MỤC TIÊU GAMEPLAY

Người chơi phải:
1. ✅ Giữ 3 đèn đường sáng
2. ✅ Giữ đèn lồng cá nhân sáng
3. ✅ Tránh bị ma chạm vào
4. ✅ Đến cuối màn → **CHIẾN THẮNG**

---

## 📞 HỖ TRỢ

Nếu gặp vấn đề, kiểm tra:
1. Console logs (Window → General → Console)
2. Debug messages trong code
3. Inspector settings của các GameObject
4. Build Settings (File → Build Settings)

---

**Chúc bạn chơi game vui vẻ! 🎮👻**
