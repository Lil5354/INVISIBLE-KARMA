using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Bắt buộc dùng TextMeshPro
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class StoryLine
{
    public string characterName; // Tên nhân vật (để trống nếu là lời dẫn truyện)
    [TextArea(3, 10)]
    public string content;       // Nội dung thoại/mô tả
    public bool isNarration;     // Tích vào nếu đây là đoạn mô tả không khí
}

public class Scene1Manager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("TextMeshPro hiển thị tên nhân vật")]
    public TextMeshProUGUI nameText;
    
    [Tooltip("TextMeshPro hiển thị nội dung thoại/mô tả")]
    public TextMeshProUGUI contentText;
    
    [Tooltip("Cái khung chứa tên (để tắt đi khi dẫn truyện) - Tùy chọn")]
    public GameObject nameContainer; // Cái khung chứa tên (để tắt đi khi dẫn truyện)

    [Header("Cài đặt")]
    [Tooltip("Tốc độ gõ chữ (giây/ ký tự) - Số càng nhỏ càng nhanh")]
    public float typingSpeed = 0.05f; // Tốc độ chữ chạy
    
    [Tooltip("Tên scene gameplay để chuyển khi hết scene 1")]
    public string gameplaySceneName = "Chapter1";

    [Header("Nội dung kịch bản (Nhập ở đây hoặc Inspector)")]
    public List<StoryLine> storyLines = new List<StoryLine>();

    private int index = 0;
    private bool isTyping = false;

    void Start()
    {
        // TỰ ĐỘNG NẠP DỮ LIỆU CỐT TRUYỆN CỦA BẠN VÀO ĐÂY
        // (Hoặc bạn có thể xóa đoạn này và nhập tay ngoài Inspector cho dễ sửa)
        
        // Chỉ load data nếu list rỗng (cho phép nhập từ Inspector)
        if (storyLines.Count == 0)
        {
            LoadStoryData();
        }
        
        // Kiểm tra UI references
        if (nameText == null || contentText == null)
        {
            Debug.LogError("[Scene1Manager] ⚠️ Thiếu UI References! Hãy kéo TextMeshPro components vào Inspector.");
            return;
        }
        
        // Bắt đầu phát dòng đầu tiên
        if (storyLines.Count > 0)
        {
            StartCoroutine(PlayLine(storyLines[index]));
        }
        else
        {
            Debug.LogWarning("[Scene1Manager] ⚠️ Không có nội dung kịch bản!");
        }
        
        // Mở khóa cursor
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
                // Nếu đang gõ mà bấm -> Hiện hết luôn (Skip)
                StopAllCoroutines();
                contentText.text = storyLines[index].content;
                isTyping = false;
                Debug.Log("[Scene1Manager] Đã skip hiệu ứng gõ chữ");
            }
            else
            {
                // Nếu gõ xong rồi -> Chuyển câu tiếp theo
                NextLine();
            }
        }
    }

    void NextLine()
    {
        index++;
        if (index < storyLines.Count)
        {
            StartCoroutine(PlayLine(storyLines[index]));
        }
        else
        {
            Debug.Log("═══════════════════════════════════════");
            Debug.Log($"[Scene1Manager] ✅ HẾT SCENE 1 -> Chuyển sang Scene Gameplay: {gameplaySceneName}");
            Debug.Log("═══════════════════════════════════════");
            
            // Chuyển sang scene gameplay
            LoadGameplayScene();
        }
    }
    
    /// <summary>
    /// Chuyển sang scene gameplay
    /// </summary>
    void LoadGameplayScene()
    {
        if (string.IsNullOrEmpty(gameplaySceneName))
        {
            Debug.LogError("[Scene1Manager] ❌ Chưa cấu hình tên scene gameplay!");
            return;
        }
        
        // Kiểm tra scene có tồn tại không
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == gameplaySceneName)
            {
                sceneExists = true;
                Debug.Log($"[Scene1Manager] ✅ Tìm thấy scene '{gameplaySceneName}' trong Build Settings (Index: {i})");
                break;
            }
        }
        
        if (!sceneExists)
        {
            Debug.LogError($"[Scene1Manager] ❌ KHÔNG TÌM THẤY SCENE '{gameplaySceneName}' TRONG BUILD SETTINGS!");
            Debug.LogError($"[Scene1Manager] ❌ Vui lòng thêm scene '{gameplaySceneName}' vào File -> Build Settings -> Add Open Scenes");
            return;
        }
        
        SceneManager.LoadScene(gameplaySceneName);
        Debug.Log($"[Scene1Manager] ✅ Đã chuyển sang scene: {gameplaySceneName}");
    }

    IEnumerator PlayLine(StoryLine line)
    {
        isTyping = true;
        contentText.text = ""; // Xóa trắng

        // Xử lý hiển thị Tên
        if (line.isNarration || string.IsNullOrEmpty(line.characterName))
        {
            nameText.text = "";
            
            // Nếu có khung tên riêng thì tắt nó đi
            if (nameContainer != null)
            {
                nameContainer.SetActive(false);
            }
            
            // Chữ nghiêng cho lời dẫn
            contentText.fontStyle = FontStyles.Italic;
            Debug.Log($"[Scene1Manager] 📖 Dẫn truyện: {line.content.Substring(0, Mathf.Min(30, line.content.Length))}...");
        }
        else
        {
            nameText.text = line.characterName;
            
            // Bật khung tên nếu có
            if (nameContainer != null)
            {
                nameContainer.SetActive(true);
            }
            
            // Chữ bình thường cho lời thoại
            contentText.fontStyle = FontStyles.Normal;
            Debug.Log($"[Scene1Manager] 💬 {line.characterName}: {line.content.Substring(0, Mathf.Min(30, line.content.Length))}...");
        }

        // Hiệu ứng gõ máy chữ
        foreach (char letter in line.content.ToCharArray())
        {
            contentText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        Debug.Log("[Scene1Manager] ✅ Đã hiển thị xong dòng này. Nhấn Space/Click để tiếp tục...");
    }

    // Hàm này nạp cứng nội dung bạn gửi (Code-driven)
    void LoadStoryData()
    {
        storyLines.Clear();
        
        storyLines.Add(new StoryLine { 
            isNarration = true, 
            content = "Gió thổi mạnh. Tiền vàng mã bay tứ tung trong ánh đèn lồng đỏ quạch." 
        });

        storyLines.Add(new StoryLine { 
            isNarration = true, 
            content = "Linh bước xuống xe. Cô cầm điện thoại lên cao cố tìm sóng, nhưng màn hình chỉ báo 'No Signal'." 
        });

        storyLines.Add(new StoryLine { 
            characterName = "LINH", 
            content = "Bố ơi... Bố gọi con về gấp, sao mọi thứ lại lạnh tanh thế này? Mọi người đi đâu hết rồi?" 
        });

        storyLines.Add(new StoryLine { 
            isNarration = true, 
            content = "Linh đi sâu vào làng. Không một bóng người. Cửa các nhà đều đóng kín..." 
        });

        storyLines.Add(new StoryLine { 
            isNarration = true, 
            content = "...bên ngoài bày la liệt những hàng mã: Ngựa giấy, xe hơi giấy, và những hình nhân người hầu đứng rũ rượi." 
        });

        storyLines.Add(new StoryLine { 
            isNarration = true, 
            content = "Sự im lặng bao trùm đến mức Linh có thể nghe thấy tiếng tim mình đập thình thịch." 
        });
        
        Debug.Log($"[Scene1Manager] ✅ Đã load {storyLines.Count} dòng kịch bản từ code");
    }
    
    /// <summary>
    /// Reset về dòng đầu tiên (có thể gọi từ button UI)
    /// </summary>
    public void ResetStory()
    {
        index = 0;
        StopAllCoroutines();
        isTyping = false;
        StartCoroutine(PlayLine(storyLines[index]));
        Debug.Log("[Scene1Manager] Đã reset về dòng đầu tiên");
    }
    
    /// <summary>
    /// Skip toàn bộ và chuyển scene ngay (có thể gọi từ button UI)
    /// </summary>
    public void SkipToGameplay()
    {
        StopAllCoroutines();
        isTyping = false;
        LoadGameplayScene();
        Debug.Log("[Scene1Manager] Đã skip toàn bộ scene 1");
    }
}


