using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StoryLine
{
    public string characterName; // Tên nhân vật (để trống nếu là lời dẫn truyện)
    [TextArea(3, 10)]
    public string content;       // Nội dung thoại/mô tả
    public bool isNarration;     // Tích vào nếu đây là đoạn mô tả không khí

    [Header("Âm thanh (Tùy chọn)")]
    [Tooltip("Kéo file ghi âm giọng đọc hoặc tiếng động cho đoạn này vào đây")]
    public AudioClip voiceClip;  // <-- THÊM MỚI: File âm thanh cho từng câu
}

public class Scene1Manager : MonoBehaviour
{
    [Header("UI References")]
    public Text nameText;
    public Text contentText;
    public GameObject nameContainer;

    [Header("--- CẤU HÌNH ÂM THANH ---")]
    [Tooltip("Nguồn phát giọng đọc (Voice) - Sẽ tự động tạo nếu chưa gán")]
    public AudioSource voiceSource;

    [Tooltip("Nguồn phát tiếng gõ phím (SFX) - Sẽ tự động tạo nếu chưa gán")]
    public AudioSource sfxSource;

    [Tooltip("File âm thanh tiếng gõ lách cách (nếu muốn)")]
    public AudioClip typingSound;

    [Tooltip("Tần suất phát tiếng gõ (ví dụ: 2 ký tự kêu 1 lần cho đỡ ồn)")]
    [Range(1, 10)]
    public int typingFrequency = 10;

    [Header("Cài đặt Game")]
    public float typingSpeed = 0.05f;
    public string gameplaySceneName = "Chapter1";

    [Header("Nội dung kịch bản")]
    public List<StoryLine> storyLines = new List<StoryLine>();

    private int index = 0;
    private bool isTyping = false;

    void Start()
    {
        // 1. Tự động tạo AudioSource nếu bạn quên kéo vào
        if (voiceSource == null)
        {
            voiceSource = gameObject.AddComponent<AudioSource>();
            voiceSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }

        // Đảm bảo không lặp lại giọng đọc
        voiceSource.loop = false;
        sfxSource.loop = false;

        // 2. Load dữ liệu mẫu nếu list rỗng
        if (storyLines.Count == 0)
        {
            LoadStoryData();
        }

        // 3. Kiểm tra UI
        if (nameText == null || contentText == null)
        {
            Debug.LogError("[Scene1Manager] ⚠️ Thiếu UI References!");
            return;
        }

        // 4. Bắt đầu dòng đầu tiên
        if (storyLines.Count > 0)
        {
            StartCoroutine(PlayLine(storyLines[index]));
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        // Bấm chuột trái hoặc Space để qua câu
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // SKIP: Hiện hết chữ ngay lập tức
                StopAllCoroutines();
                contentText.text = storyLines[index].content;
                isTyping = false;
                Debug.Log("[Scene1Manager] Đã skip hiệu ứng gõ chữ");
            }
            else
            {
                // NEXT: Chuyển câu tiếp theo
                NextLine();
            }
        }
    }

    void NextLine()
    {
        // QUAN TRỌNG: Ngắt giọng đọc cũ khi chuyển sang câu mới
        if (voiceSource.isPlaying) voiceSource.Stop();

        index++;
        if (index < storyLines.Count)
        {
            StartCoroutine(PlayLine(storyLines[index]));
        }
        else
        {
            Debug.Log("[Scene1Manager] ✅ HẾT SCENE 1 -> Chuyển Scene");
            LoadGameplayScene();
        }
    }

    void LoadGameplayScene()
    {
        if (string.IsNullOrEmpty(gameplaySceneName)) return;

        // Kiểm tra an toàn trước khi load
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (System.IO.Path.GetFileNameWithoutExtension(scenePath) == gameplaySceneName)
            {
                sceneExists = true;
                break;
            }
        }

        if (sceneExists) SceneManager.LoadScene(gameplaySceneName);
        else Debug.LogError($"[Scene1Manager] ❌ Không tìm thấy scene '{gameplaySceneName}'!");
    }

    IEnumerator PlayLine(StoryLine line)
    {
        isTyping = true;
        contentText.text = "";

        // --- XỬ LÝ UI ---
        if (line.isNarration || string.IsNullOrEmpty(line.characterName))
        {
            nameText.text = "";
            if (nameContainer != null) nameContainer.SetActive(false);
            contentText.fontStyle = FontStyle.Italic;
        }
        else
        {
            nameText.text = line.characterName;
            if (nameContainer != null) nameContainer.SetActive(true);
            contentText.fontStyle = FontStyle.Normal;
        }

        // --- XỬ LÝ ÂM THANH (VOICE) ---
        // Nếu dòng này có file âm thanh thì phát
        if (line.voiceClip != null && voiceSource != null)
        {
            voiceSource.clip = line.voiceClip;
            voiceSource.Play();
        }

        // --- HIỆU ỨNG GÕ CHỮ & TIẾNG LÁCH CÁCH ---
        int charCount = 0;
        foreach (char letter in line.content.ToCharArray())
        {
            contentText.text += letter;
            charCount++;

            // Phát tiếng gõ phím (nếu có setup)
            if (sfxSource != null && typingSound != null && charCount % typingFrequency == 0)
            {
                // Random nhẹ cao độ để nghe tự nhiên hơn
                sfxSource.pitch = Random.Range(0.95f, 1.05f);
                sfxSource.PlayOneShot(typingSound);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    // Hàm này chỉ chạy 1 lần đầu để tạo khung dữ liệu
    // Sau khi chạy xong, bạn nên chỉnh sửa list StoryLines trong Inspector để gán Audio Clip
    // Hàm này nạp cứng nội dung bạn gửi (Code-driven)
    void LoadStoryData()
    {
        storyLines.Clear();

        storyLines.Add(new StoryLine
        {
            isNarration = true,
            content = "Gió đêm rít từng cơn lạnh buốt. Những đồng tiền vàng mã bị cuốn bay tứ tung, xoay vòng trong ánh đèn lồng đỏ quạch như một đám tang không người."
        });

        storyLines.Add(new StoryLine
        {
            isNarration = true,
            content = "Linh bước xuống xe. Cô tuyệt vọng giơ cao chiếc điện thoại, nhưng dòng chữ 'No Signal' nhấp nháy .."
        });

        storyLines.Add(new StoryLine
        {
            characterName = "LINH",
            content = "Cha ơi... Cuối cùng con cũng về lại nơi này, sao mọi thứ lại lạnh tanh thế này?.... Mọi người đi đâu hết rồi?"
        });

        storyLines.Add(new StoryLine
        {
            isNarration = true,
            content = "Linh đi sâu vào trong thôn. Không một bóng người sống. Cửa các ngôi nhà gỗ đều đóng im ỉm, then cài chặt như sợ hãi một thứ gì đó từ bên ngoài"
        });

        storyLines.Add(new StoryLine
        {
            isNarration = true,
            content = "...Chỉ có những hàng mã được bày la liệt. Ngựa giấy, xe tang, và những hình nhân thế mạng đứng rũ rượi... đôi mắt chấm mực đen vô hồn như đang dõi theo từng bước chân của cô."
        });

        storyLines.Add(new StoryLine
        {
            isNarration = true,
            content = "Sự im lặng bao trùm đến mức Linh có thể nghe thấy tiếng tim mình đập thình thịch."
        });
        storyLines.Add(new StoryLine
        {
            isNarration = true,
            content = "Phía trước là Rừng Trúc. Ánh trăng rằm soi rõ con đường độc đạo... Cô bắt buộc phải đi qua nó."
        });

        Debug.Log($"[Scene1Manager] ✅ Đã load {storyLines.Count} dòng kịch bản từ code");
    }

    public void ResetStory()
    {
        index = 0;
        StopAllCoroutines();
        if (voiceSource.isPlaying) voiceSource.Stop(); // Reset cả âm thanh
        isTyping = false;
        StartCoroutine(PlayLine(storyLines[index]));
    }

    public void SkipToGameplay()
    {
        StopAllCoroutines();
        if (voiceSource.isPlaying) voiceSource.Stop();
        LoadGameplayScene();
    }
}

