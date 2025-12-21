using UnityEngine;
using System.Collections;
using TMPro; // Để hiện chữ
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Để chuyển cảnh sau khi ngất

/// <summary>
/// Script đạo diễn các sự kiện trong Rừng Trúc (Scene 3D)
/// Quản lý: Độc thoại, Jumpscare, và Cảnh truy đuổi
/// </summary>
public class ForestEventDirector : MonoBehaviour
{
    [Header("UI Hội thoại")]
    [Tooltip("Panel chứa subtitle (DialoguePanel)")]
    public GameObject dialoguePanel;
    
    [Tooltip("TextMeshPro hiển thị lời thoại")]
    public TextMeshProUGUI dialogueText;

    [Header("Diễn viên (Models)")]
    [Tooltip("Hình nhân áo vàng (ban đầu setActive = false)")]
    public GameObject yellowPaperDoll; // Hình nhân áo vàng (ban đầu setActive là false hoặc ẩn trong bụi)
    
    [Tooltip("Vị trí giữa đường nơi hình nhân sẽ nhảy ra")]
    public Transform jumpScarePosition; // Vị trí giữa đường nó sẽ nhảy ra
    
    [Tooltip("Group chứa hàng trăm hình nhân ở cuối đường (ban đầu ẩn)")]
    public GameObject hordeOfDolls; // Một cục (Group) chứa hàng trăm hình nhân ở cuối đường (ban đầu ẩn)

    [Header("Âm thanh (SFX)")]
    [Tooltip("AudioSource để phát âm thanh")]
    public AudioSource audioSource;
    
    [Tooltip("Tiếng quạt mở 'PHẠCH'")]
    public AudioClip sfxFanOpen; // Tiếng quạt "PHẠCH"
    
    [Tooltip("Tiếng giấy sột soạt")]
    public AudioClip sfxPaperRustle; // Tiếng giấy sột soạt
    
    [Tooltip("Tiếng hét của Linh")]
    public AudioClip sfxScream; // Tiếng hét Linh
    
    [Tooltip("Tiếng chân chạy rầm rập")]
    public AudioClip sfxChaseRun; // Tiếng chân chạy rầm rập

    [Header("Hiệu ứng màn hình")]
    [Tooltip("Image đen che toàn màn hình (để fade out)")]
    public Image blackScreen; // Tấm ảnh đen che toàn màn hình (để fade out)

    [Header("Cài đặt")]
    [Tooltip("Tên scene tiếp theo sau khi fade out (để trống nếu không chuyển)")]
    public string nextSceneName = ""; // Tên scene tiếp theo
    
    [Tooltip("Tự động bắt đầu Event 1 khi Start")]
    public bool autoStartEvent1 = true;

    // Biến kiểm tra để sự kiện chỉ chạy 1 lần
    private bool event1Triggered = false;
    private bool event2Triggered = false;
    private bool event3Triggered = false;
    
    private Coroutine currentSubtitleCoroutine; // Để có thể dừng subtitle cũ

    void Start()
    {
        Debug.Log("[ForestEventDirector] Script đã được khởi tạo");
        
        // Kiểm tra UI references
        if (dialoguePanel == null || dialogueText == null)
        {
            Debug.LogWarning("[ForestEventDirector] ⚠️ Thiếu UI References! Hãy kéo DialoguePanel và DialogueText vào Inspector.");
        }
        
        // Ẩn các diễn viên chưa cần thiết
        if (yellowPaperDoll != null)
        {
            yellowPaperDoll.SetActive(false);
            Debug.Log("[ForestEventDirector] Đã ẩn hình nhân áo vàng");
        }
        
        if (hordeOfDolls != null)
        {
            hordeOfDolls.SetActive(false);
            Debug.Log("[ForestEventDirector] Đã ẩn bầy hình nhân");
        }
        
        // Khởi tạo màn hình đen (trong suốt)
        if (blackScreen != null)
        {
            blackScreen.color = new Color(0, 0, 0, 0); // Trong suốt
            blackScreen.gameObject.SetActive(false);
        }
        
        // Ẩn dialogue panel lúc đầu
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        
        // Bắt đầu sự kiện 1 ngay khi vào game (hoặc bạn có thể dùng Trigger)
        if (autoStartEvent1)
        {
            StartCoroutine(Event1_Monologue());
        }
    }

    /// <summary>
    /// SỰ KIỆN 1: Độc thoại của Linh (tự động chạy khi Start)
    /// </summary>
    IEnumerator Event1_Monologue()
    {
        if (event1Triggered)
        {
            Debug.LogWarning("[ForestEventDirector] Event 1 đã được trigger rồi!");
            yield break;
        }
        
        event1Triggered = true;
        Debug.Log("[ForestEventDirector] 🎬 Bắt đầu Event 1: Độc thoại");
        
        yield return new WaitForSeconds(2f); // Đợi 2s sau khi vào game
        ShowSubtitle("Từ nhỏ đã có một điều luật bất li thân. Đi qua rừng trúc phải luôn bật sáng đèn...", 4f);

        yield return new WaitForSeconds(4.5f);
        ShowSubtitle("(Thở dốc) Chỉ là giấy bồi thôi... Khung tre, hồ dán... Không có sự sống...", 4f);
        yield return new WaitForSeconds(4.5f);
        
        ShowSubtitle("Bình tĩnh... Mày là kiến trúc sư mà...", 3f);
        yield return new WaitForSeconds(3.5f);
        
        Debug.Log("[ForestEventDirector] ✅ Event 1 hoàn thành");
    }

    /// <summary>
    /// SỰ KIỆN 2: Hình nhân áo vàng Jumpscare
    /// Hàm này sẽ được gọi từ EventTrigger khi Player đi đến giữa đường
    /// </summary>
    public void TriggerEvent2_YellowDoll()
    {
        if (event2Triggered)
        {
            Debug.LogWarning("[ForestEventDirector] Event 2 đã được trigger rồi!");
            return;
        }
        
        Debug.Log("[ForestEventDirector] 🎬 Bắt đầu Event 2: Jumpscare hình nhân áo vàng");
        StartCoroutine(PlayEvent2());
    }

    IEnumerator PlayEvent2()
    {
        event2Triggered = true;
        
        // 1. Tiếng sột soạt dữ dội
        if (audioSource != null && sfxPaperRustle != null)
        {
            audioSource.PlayOneShot(sfxPaperRustle);
            Debug.Log("[ForestEventDirector] 🔊 Phát tiếng giấy sột soạt");
        }
        
        yield return new WaitForSeconds(0.5f);

        // 2. Hình nhân áo vàng xuất hiện giữa đường
        if (yellowPaperDoll != null && jumpScarePosition != null)
        {
            yellowPaperDoll.SetActive(true);
            yellowPaperDoll.transform.position = jumpScarePosition.position;
            
            // Quay mặt vào người chơi (Camera)
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 lookDirection = mainCamera.transform.position - yellowPaperDoll.transform.position;
                lookDirection.y = 0; // Chỉ quay ngang
                if (lookDirection != Vector3.zero)
                {
                    yellowPaperDoll.transform.rotation = Quaternion.LookRotation(lookDirection);
                }
            }
            
            Debug.Log("[ForestEventDirector] 👻 Hình nhân áo vàng đã xuất hiện!");
        }
        else
        {
            Debug.LogWarning("[ForestEventDirector] ⚠️ Thiếu yellowPaperDoll hoặc jumpScarePosition!");
        }

        // 3. Linh hét lên
        ShowSubtitle("Ai đó?!?", 2f);
        yield return new WaitForSeconds(1f);

        // 4. Tiếng quạt mở PHẠCH
        if (audioSource != null && sfxFanOpen != null)
        {
            audioSource.PlayOneShot(sfxFanOpen);
            Debug.Log("[ForestEventDirector] 🔊 Phát tiếng quạt PHẠCH");
        }
        
        // 5. Suy nghĩ nội tâm
        yield return new WaitForSeconds(1.5f);
        ShowSubtitle("Chúng không đuổi theo mình... Chúng đang lùa mình về phía con đường phía trước...", 4f);
        yield return new WaitForSeconds(4.5f);
        
        Debug.Log("[ForestEventDirector] ✅ Event 2 hoàn thành");
    }

    /// <summary>
    /// SỰ KIỆN 3: Cảnh truy đuổi & Ngất (Kết thúc Gameplay)
    /// Gọi hàm này khi người chơi đi đến gần ngôi nhà cuối đường
    /// </summary>
    public void TriggerEvent3_TheChase()
    {
        if (event3Triggered)
        {
            Debug.LogWarning("[ForestEventDirector] Event 3 đã được trigger rồi!");
            return;
        }
        
        Debug.Log("[ForestEventDirector] 🎬 Bắt đầu Event 3: Cảnh truy đuổi");
        StartCoroutine(PlayEvent3());
    }

    IEnumerator PlayEvent3()
    {
        event3Triggered = true;
        
        // 1. Hiện bầy hình nhân phía sau
        if (hordeOfDolls != null)
        {
            hordeOfDolls.SetActive(true);
            Debug.Log("[ForestEventDirector] 👻👻👻 Bầy hình nhân đã xuất hiện!");
        }
        
        // 2. Tiếng chạy rầm rập + Tiếng hét
        if (audioSource != null)
        {
            if (sfxChaseRun != null)
            {
                audioSource.PlayOneShot(sfxChaseRun);
                Debug.Log("[ForestEventDirector] 🔊 Phát tiếng chạy rầm rập");
            }
            
            if (sfxScream != null)
            {
                audioSource.PlayOneShot(sfxScream);
                Debug.Log("[ForestEventDirector] 🔊 Phát tiếng hét");
            }
        }
        
        ShowSubtitle("ÁÁÁ!!!", 1f);
        yield return new WaitForSeconds(1.5f);

        // 3. Màn hình tối dần (Fade to Black)
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            float fadeDuration = 2f;
            float timer = 0;
            
            Debug.Log("[ForestEventDirector] 🌑 Bắt đầu fade to black...");
            
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                blackScreen.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            
            Debug.Log("[ForestEventDirector] ✅ Fade to black hoàn thành");
        }

        // 4. Chuyển sang màn Cutscene tiếp theo hoặc Game Over
        yield return new WaitForSeconds(1f);
        
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"[ForestEventDirector] 🚀 Chuyển sang scene: {nextSceneName}");
            
            // Kiểm tra scene có tồn tại không
            bool sceneExists = false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (sceneName == nextSceneName)
                {
                    sceneExists = true;
                    break;
                }
            }
            
            if (sceneExists)
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogError($"[ForestEventDirector] ❌ Không tìm thấy scene '{nextSceneName}' trong Build Settings!");
            }
        }
        else
        {
            Debug.Log("[ForestEventDirector] ℹ️ Không có scene tiếp theo được cấu hình");
        }
    }

    /// <summary>
    /// Hàm phụ trợ để hiện chữ subtitle
    /// </summary>
    void ShowSubtitle(string text, float duration)
    {
        // Dừng subtitle cũ nếu có
        if (currentSubtitleCoroutine != null)
        {
            StopCoroutine(currentSubtitleCoroutine);
        }
        
        currentSubtitleCoroutine = StartCoroutine(SubtitleRoutine(text, duration));
    }

    IEnumerator SubtitleRoutine(string text, float duration)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }
        
        if (dialogueText != null)
        {
            dialogueText.text = text;
            Debug.Log($"[ForestEventDirector] 💬 Subtitle: {text}");
        }
        
        yield return new WaitForSeconds(duration);
        
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
        
        currentSubtitleCoroutine = null;
    }
    
    /// <summary>
    /// Reset tất cả events (có thể gọi từ Inspector hoặc script khác)
    /// </summary>
    [ContextMenu("Reset All Events")]
    public void ResetAllEvents()
    {
        event1Triggered = false;
        event2Triggered = false;
        event3Triggered = false;
        Debug.Log("[ForestEventDirector] Đã reset tất cả events");
    }
}
