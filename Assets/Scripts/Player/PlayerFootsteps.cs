using UnityEngine;

/// <summary>
/// Script đơn giản để phát tiếng bước chân khi Player đi và chạy
/// Gắn vào Player object
/// </summary>
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Cài đặt")]
    [Tooltip("Kéo AudioSource vào đây")]
    public AudioSource footstepSource;
    
    [Tooltip("Kéo CharacterController vào đây (hoặc để trống, script sẽ tự tìm)")]
    public CharacterController characterController;

    [Header("Âm thanh")]
    [Tooltip("Kéo các file tiếng đi bộ vào đây (nên có 3-5 variations)")]
    public AudioClip[] walkSounds;
    
    [Tooltip("Kéo các file tiếng chạy rầm rập vào đây (nên có 3-5 variations)")]
    public AudioClip[] runSounds;

    [Header("Tốc độ phát (Giây)")]
    [Tooltip("Đi bộ: bao nhiêu giây kêu 1 lần")]
    public float walkInterval = 0.5f; // Đi bộ: 0.5s kêu 1 lần
    
    [Tooltip("Chạy: bao nhiêu giây kêu 1 lần (nhanh hơn đi bộ)")]
    public float runInterval = 0.3f;  // Chạy: 0.3s kêu 1 lần

    private float stepTimer;

    void Start()
    {
        // Tự động tìm CharacterController nếu chưa gán
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                Debug.LogError("[PlayerFootsteps] ⚠️ Không tìm thấy CharacterController! Script này cần CharacterController để hoạt động.");
                enabled = false;
                return;
            }
        }
        
        // Tự động tạo AudioSource nếu chưa gán
        if (footstepSource == null)
        {
            footstepSource = gameObject.AddComponent<AudioSource>();
            footstepSource.playOnAwake = false;
            footstepSource.loop = false;
            footstepSource.spatialBlend = 0f; // 2D sound
            Debug.Log("[PlayerFootsteps] ✅ Đã tự động tạo AudioSource");
        }
        
        // Kiểm tra âm thanh
        if (walkSounds == null || walkSounds.Length == 0)
        {
            Debug.LogWarning("[PlayerFootsteps] ⚠️ Chưa có âm thanh đi bộ! Hãy kéo các file sound vào mảng 'Walk Sounds'.");
        }
        
        if (runSounds == null || runSounds.Length == 0)
        {
            Debug.LogWarning("[PlayerFootsteps] ⚠️ Chưa có âm thanh chạy! Sẽ không có tiếng khi chạy.");
        }
    }

    void Update()
    {
        // 1. Kiểm tra xem có đang đứng trên đất không?
        if (characterController != null && characterController.isGrounded)
        {
            // 2. Kiểm tra xem người chơi CÓ BẤM NÚT di chuyển không (WASD)
            // (Cách này sửa lỗi nhân vật trượt nhẹ mà vẫn kêu)
            float inputX = Input.GetAxis("Horizontal"); // A, D
            float inputZ = Input.GetAxis("Vertical");   // W, S

            // Nếu có bấm phím (giá trị khác 0)
            if (Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputZ) > 0.1f)
            {
                HandleFootsteps();
            }
            else
            {
                // Đứng im -> Reset timer để bước đầu tiên khi đi lại sẽ kêu ngay lập tức
                stepTimer = 0;
            }
        }
    }

    void HandleFootsteps()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift); // Giữ Shift là chạy

        // Chọn tốc độ dựa trên việc có chạy hay không
        float currentInterval = isRunning ? runInterval : walkInterval;

        stepTimer -= Time.deltaTime; // Đếm ngược thời gian

        if (stepTimer <= 0)
        {
            PlaySound(isRunning);
            stepTimer = currentInterval; // Reset đồng hồ
        }
    }

    void PlaySound(bool isRunning)
    {
        if (footstepSource == null)
        {
            return;
        }
        
        AudioClip clipToPlay = null;

        // Tách biệt hoàn toàn: Chạy ra Chạy, Đi ra Đi
        if (isRunning)
        {
            if (runSounds != null && runSounds.Length > 0)
                clipToPlay = runSounds[Random.Range(0, runSounds.Length)];
        }
        else
        {
            if (walkSounds != null && walkSounds.Length > 0)
                clipToPlay = walkSounds[Random.Range(0, walkSounds.Length)];
        }

        // Chỉ phát nếu có file âm thanh
        if (clipToPlay != null)
        {
            // Ngẫu nhiên độ cao chút xíu cho đỡ nhàm chán
            footstepSource.pitch = Random.Range(0.9f, 1.1f);
            // Dùng PlayOneShot để phát dứt khoát từng bước
            footstepSource.PlayOneShot(clipToPlay);
        }
    }
}
