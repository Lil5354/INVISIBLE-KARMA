using UnityEngine;

/// <summary>
/// Quản lý âm thanh bước chân cho nhân vật
/// Tự động phát âm thanh khi di chuyển với các loại surface khác nhau
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class FootstepAudioController : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] footstepSounds; // Mảng các âm thanh bước chân
    [SerializeField] private AudioClip[] runFootstepSounds; // Âm thanh khi chạy (nếu khác)
    [SerializeField] private float footstepInterval = 0.5f; // Khoảng thời gian giữa các bước
    [SerializeField] private float runFootstepInterval = 0.3f; // Khoảng thời gian khi chạy
    [SerializeField] private float volumeMin = 0.7f;
    [SerializeField] private float volumeMax = 1f;
    [SerializeField] private float pitchMin = 0.9f;
    [SerializeField] private float pitchMax = 1.1f;
    
    [Header("Surface Detection")]
    [SerializeField] private LayerMask groundLayer = 1;
    [SerializeField] private float raycastDistance = 1.5f;
    
    [Header("References")]
    [SerializeField] private FirstPersonController playerController;
    
    private AudioSource audioSource;
    private float footstepTimer = 0f;
    private bool isMoving = false;
    private bool isRunning = false;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Tự động tìm FirstPersonController nếu chưa gán
        if (playerController == null)
        {
            playerController = GetComponent<FirstPersonController>();
            if (playerController == null)
            {
                playerController = FindObjectOfType<FirstPersonController>();
            }
        }
        
        // Setup AudioSource
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D sound
            audioSource.loop = false;
        }
    }
    
    void Update()
    {
        if (playerController == null || audioSource == null)
            return;
        
        // Kiểm tra trạng thái di chuyển
        isMoving = playerController.IsMoving();
        isRunning = playerController.GetCurrentSpeed() > 3.5f; // Nhanh hơn walk speed
        
        if (isMoving)
        {
            // Tăng timer
            float interval = isRunning ? runFootstepInterval : footstepInterval;
            footstepTimer += Time.deltaTime;
            
            // Phát âm thanh khi đến interval
            if (footstepTimer >= interval)
            {
                PlayFootstep();
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }
    
    /// <summary>
    /// Phát âm thanh bước chân
    /// </summary>
    void PlayFootstep()
    {
        if (audioSource == null || footstepSounds == null || footstepSounds.Length == 0)
            return;
        
        // Chọn âm thanh phù hợp
        AudioClip[] clipsToUse = isRunning && runFootstepSounds != null && runFootstepSounds.Length > 0
            ? runFootstepSounds
            : footstepSounds;
        
        if (clipsToUse.Length == 0)
            return;
        
        // Random clip
        AudioClip clipToPlay = clipsToUse[Random.Range(0, clipsToUse.Length)];
        
        // Random volume và pitch để tự nhiên hơn
        audioSource.volume = Random.Range(volumeMin, volumeMax);
        audioSource.pitch = Random.Range(pitchMin, pitchMax);
        
        // Phát âm thanh
        audioSource.PlayOneShot(clipToPlay);
    }
    
    /// <summary>
    /// Thêm âm thanh bước chân vào mảng
    /// </summary>
    public void AddFootstepSound(AudioClip clip)
    {
        if (clip == null) return;
        
        System.Collections.Generic.List<AudioClip> list = new System.Collections.Generic.List<AudioClip>();
        if (footstepSounds != null)
        {
            list.AddRange(footstepSounds);
        }
        list.Add(clip);
        footstepSounds = list.ToArray();
    }
    
    /// <summary>
    /// Thêm âm thanh chạy vào mảng
    /// </summary>
    public void AddRunFootstepSound(AudioClip clip)
    {
        if (clip == null) return;
        
        System.Collections.Generic.List<AudioClip> list = new System.Collections.Generic.List<AudioClip>();
        if (runFootstepSounds != null)
        {
            list.AddRange(runFootstepSounds);
        }
        list.Add(clip);
        runFootstepSounds = list.ToArray();
    }
    
    /// <summary>
    /// Phát âm thanh bước chân một lần (dùng cho scripted events)
    /// </summary>
    public void PlayFootstepOnce()
    {
        PlayFootstep();
    }
}










