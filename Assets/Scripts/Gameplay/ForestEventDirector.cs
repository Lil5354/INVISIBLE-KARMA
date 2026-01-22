using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script đạo diễn các sự kiện trong Rừng Trúc (Scene 3D)
/// Quản lý: Độc thoại, Jumpscare, và Cảnh truy đuổi
/// Đã tích hợp hệ thống Âm thanh (Voice + SFX)
/// </summary>
public class ForestEventDirector : MonoBehaviour
{
    [Header("--- UI HỘI THOẠI ---")]
    [Tooltip("Panel chứa subtitle (DialoguePanel)")]
    public GameObject dialoguePanel;

    [Tooltip("Text hiển thị lời thoại")]
    public Text dialogueText;

    [Header("--- DIỄN VIÊN (MODELS) ---")]
    [Tooltip("Hình nhân áo vàng (ban đầu setActive = false)")]
    public GameObject yellowPaperDoll;

    [Tooltip("Vị trí giữa đường nơi hình nhân sẽ nhảy ra")]
    public Transform jumpScarePosition;

    [Tooltip("Group chứa hàng trăm hình nhân ở cuối đường")]
    public GameObject hordeOfDolls;

    [Header("--- CẤU HÌNH AUDIO SOURCE ---")]
    [Tooltip("Nguồn phát tiếng động (SFX) - Dùng AudioSource 1")]
    public AudioSource sfxSource;

    [Tooltip("Nguồn phát giọng nói (Voice) - Dùng AudioSource 2")]
    public AudioSource voiceSource;

    [Header("--- AUDIO CLIPS (SỰ KIỆN) ---")]
    [Tooltip("Tiếng quạt mở 'PHẠCH'")]
    public AudioClip sfxFanOpen;

    [Tooltip("Tiếng giấy sột soạt")]
    public AudioClip sfxPaperRustle;

    [Tooltip("Tiếng hét của Linh")]
    public AudioClip sfxScream;

    [Tooltip("Tiếng chân chạy rầm rập")]
    public AudioClip sfxChaseRun;

    [Header("--- AUDIO CLIPS (THOẠI) ---")]
    [Tooltip("Thoại 1: Từ nhỏ đã có một điều luật...")]
    public AudioClip voiceLine1;
    [Tooltip("Thoại 2: Chỉ là giấy bồi thôi...")]
    public AudioClip voiceLine2;
    [Tooltip("Thoại 3: Bình tĩnh...")]
    public AudioClip voiceLine3;
    [Tooltip("Thoại 4: Ai đó?!?")]
    public AudioClip voiceLine4;
    [Tooltip("Thoại 5: Chúng không đuổi theo mình...")]
    public AudioClip voiceLine5;
    [Tooltip("Tiếng la hét cuối cùng (Voice)")]
    public AudioClip voiceScreamEnd;

    [Header("--- HIỆU ỨNG MÀN HÌNH ---")]
    public Image blackScreen;

    [Header("--- CÀI ĐẶT ---")]
    public string nextSceneName = "";
    public bool autoStartEvent1 = true;

    // Biến trạng thái
    private bool event1Triggered = false;
    private bool event2Triggered = false;
    private bool event3Triggered = false;

    private Coroutine currentSubtitleCoroutine;

    void Start()
    {
        // 1. Tự động tìm và CẤU HÌNH AudioSource
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();
        if (voiceSource == null) voiceSource = sfxSource;

        // --- SỬA LỖI LẶP LẠI Ở ĐÂY ---
        if (sfxSource != null) sfxSource.loop = false;   // Ép tắt Loop cho SFX
        if (voiceSource != null) voiceSource.loop = false; // Ép tắt Loop cho Giọng nói

        // 2. Setup ban đầu
        SetupInitialState();

        // 3. Bắt đầu sự kiện 1
        if (autoStartEvent1)
        {
            StartCoroutine(Event1_Monologue());
        }
    }

    void SetupInitialState()
    {
        if (yellowPaperDoll != null) yellowPaperDoll.SetActive(false);
        if (hordeOfDolls != null) hordeOfDolls.SetActive(false);

        if (blackScreen != null)
        {
            blackScreen.color = new Color(0, 0, 0, 0);
            blackScreen.gameObject.SetActive(false);
        }

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// Hàm tiện ích: Phát SFX (dùng PlayOneShot để chồng âm thanh)
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// SỰ KIỆN 1: Độc thoại của Linh
    /// </summary>
    IEnumerator Event1_Monologue()
    {
        if (event1Triggered) yield break;
        event1Triggered = true;

        yield return new WaitForSeconds(2f);

        // Câu 1
        ShowSubtitle("Từ nhỏ đã có một điều luật bất li thân. Đi qua rừng trúc phải luôn bật sáng đèn...", 4f, voiceLine1);
        yield return new WaitForSeconds(7f);

        // Câu 2
        ShowSubtitle("(Thở dốc) Chỉ là giấy bồi thôi... Khung tre, hồ dán... Không có sự sống...", 4f, voiceLine2);
        yield return new WaitForSeconds(5.5f);

        // Câu 3
        ShowSubtitle("Bình tĩnh... Mày là kiến trúc sư mà...", 3f, voiceLine3);
        yield return new WaitForSeconds(3.5f);

        Debug.Log("[ForestEventDirector] ✅ Event 1 hoàn thành");
    }

    /// <summary>
    /// SỰ KIỆN 2: Hình nhân áo vàng Jumpscare
    /// </summary>
    public void TriggerEvent2_YellowDoll()
    {
        if (event2Triggered) return;
        StartCoroutine(PlayEvent2());
    }

    IEnumerator PlayEvent2()
    {
        event2Triggered = true;

        // 1. Tiếng sột soạt
        PlaySFX(sfxPaperRustle);
        Debug.Log("[ForestEventDirector] 🔊 Tiếng giấy sột soạt");

        yield return new WaitForSeconds(0.5f);

        // 2. Hình nhân xuất hiện & Quay mặt
        if (yellowPaperDoll != null && jumpScarePosition != null)
        {
            yellowPaperDoll.SetActive(true);
            yellowPaperDoll.transform.position = jumpScarePosition.position;

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Vector3 lookDirection = mainCamera.transform.position - yellowPaperDoll.transform.position;
                lookDirection.y = 0;
                yellowPaperDoll.transform.rotation = Quaternion.LookRotation(lookDirection);
            }
        }

        // 3. Linh hét lên "Ai đó?!?"
        ShowSubtitle("Ai đó?!?", 2f, voiceLine4);
        yield return new WaitForSeconds(1f);

        // 4. Tiếng quạt mở PHẠCH
        PlaySFX(sfxFanOpen);
        Debug.Log("[ForestEventDirector] 🔊 Tiếng quạt PHẠCH");

        yield return new WaitForSeconds(1.5f);

        // 5. Suy nghĩ nội tâm
        ShowSubtitle("Chúng không đuổi theo mình... Chúng đang lùa mình về phía con đường phía trước...", 4f, voiceLine5);
        yield return new WaitForSeconds(4.5f);

        Debug.Log("[ForestEventDirector] ✅ Event 2 hoàn thành");
    }

    /// <summary>
    /// SỰ KIỆN 3: Cảnh truy đuổi & Ngất
    /// </summary>
    public void TriggerEvent3_TheChase()
    {
        if (event3Triggered) return;
        StartCoroutine(PlayEvent3());
    }

    IEnumerator PlayEvent3()
    {
        event3Triggered = true;

        // 1. Hiện bầy hình nhân
        if (hordeOfDolls != null) hordeOfDolls.SetActive(true);

        // 2. Âm thanh hỗn loạn
        PlaySFX(sfxChaseRun); // Tiếng chân chạy
        PlaySFX(sfxScream);   // Tiếng hét (SFX môi trường)

        // 3. Linh hét (Voice)
        ShowSubtitle("ÁÁÁ!!!", 1f, voiceScreamEnd);
        yield return new WaitForSeconds(1.5f);

        // 4. Fade to Black
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            float fadeDuration = 2f;
            float timer = 0;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
                blackScreen.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
        }

        // 5. Chuyển Scene
        yield return new WaitForSeconds(1f);
        LoadNextScene();
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // Logic kiểm tra scene an toàn
            bool sceneExists = false;
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (System.IO.Path.GetFileNameWithoutExtension(scenePath) == nextSceneName)
                {
                    sceneExists = true;
                    break;
                }
            }

            if (sceneExists) SceneManager.LoadScene(nextSceneName);
            else Debug.LogError($"[ForestEventDirector] ❌ Không tìm thấy scene '{nextSceneName}'!");
        }
    }

    /// <summary>
    /// Hàm hiển thị Subtitle + Tự động phát Voice tương ứng
    /// </summary>
    /// <param name="text">Nội dung chữ</param>
    /// <param name="duration">Thời gian hiện</param>
    /// <param name="voiceClip">File ghi âm giọng nói (có thể null)</param>
    void ShowSubtitle(string text, float duration, AudioClip voiceClip = null)
    {
        if (currentSubtitleCoroutine != null) StopCoroutine(currentSubtitleCoroutine);
        currentSubtitleCoroutine = StartCoroutine(SubtitleRoutine(text, duration, voiceClip));
    }
    IEnumerator SubtitleRoutine(string text, float duration, AudioClip voiceClip)
    {
        // 1. Hiển thị UI
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (dialogueText != null) dialogueText.text = text;

        // 2. Phát giọng nói (Nếu có)
        if (voiceClip != null && voiceSource != null)
        {
            voiceSource.Stop(); // Ngắt câu cũ ngay lập tức
            voiceSource.loop = false; // --- QUAN TRỌNG: Đảm bảo không lặp ---
            voiceSource.clip = voiceClip;
            voiceSource.Play();
        }

        Debug.Log($"[ForestEventDirector] 💬: {text}");

        // 3. Chờ hết thời gian
        yield return new WaitForSeconds(duration);

        // 4. Tắt UI
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        currentSubtitleCoroutine = null;
    }

    [ContextMenu("Reset All Events")]
    public void ResetAllEvents()
    {
        event1Triggered = false;
        event2Triggered = false;
        event3Triggered = false;
    }
}