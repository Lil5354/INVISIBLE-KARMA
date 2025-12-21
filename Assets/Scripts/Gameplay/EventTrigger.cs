using UnityEngine;

/// <summary>
/// Script trigger đơn giản để kích hoạt events trong ForestEventDirector
/// Gắn vào các GameObject với Collider (Is Trigger = true)
/// </summary>
public class EventTrigger : MonoBehaviour
{
    [Header("Cài đặt")]
    [Tooltip("Kéo ForestEventDirector vào đây")]
    public ForestEventDirector director; // Tham chiếu đến ông đạo diễn
    
    [Tooltip("Tích vào nếu là sự kiện hình nhân áo vàng, bỏ tích nếu là sự kiện truy đuổi")]
    public bool isYellowDollEvent = true; // Nhớ tích vào ô này ở Inspector nếu là event áo vàng

    void OnTriggerEnter(Collider other)
    {
        // Dòng này để kiểm tra xem CÓ CÁI GÌ ĐÓ chạm vào bẫy không
        Debug.Log($"[EventTrigger] 🔔 Cái gì đó vừa chạm vào bẫy: {other.gameObject.name} (Tag: {other.tag})");

        if (other.CompareTag("Player"))
        {
            Debug.Log("[EventTrigger] ✅ >>> ĐÚNG LÀ PLAYER RỒI! GỌI ĐẠO DIỄN NGAY!");
            
            if (director != null)
            {
                if (isYellowDollEvent)
                {
                    Debug.Log("[EventTrigger] 🎬 Triggering Event 2: Yellow Doll Jumpscare");
                    director.TriggerEvent2_YellowDoll();
                }
                else
                {
                    Debug.Log("[EventTrigger] 🎬 Triggering Event 3: The Chase");
                    director.TriggerEvent3_TheChase();
                }
                
                // Xong việc thì tự hủy cái bẫy để không bị hù lại lần 2
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("[EventTrigger] ❌ LỖI: Chưa kéo GameDirector vào ô Director của cái bẫy!");
            }
        }
        else
        {
            Debug.Log($"[EventTrigger] ⚠️ Object '{other.name}' không phải Player (Tag: {other.tag}). Bỏ qua.");
        }
    }
}
