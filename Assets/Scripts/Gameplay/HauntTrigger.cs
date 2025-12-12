using UnityEngine;

/// <summary>
/// Script tạo ra ma sau lưng người chơi khi đi qua trigger
/// Sử dụng cho Jump Scare effect
/// </summary>
public class HauntTrigger : MonoBehaviour
{
    [Header("Cài đặt Ma")]
    public GameObject enemyPrefab; // Kéo Prefab con ma vào đây
    public float spawnDistanceBehind = 10f; // Ma xuất hiện cách lưng bao xa?

    [Header("Cài đặt Âm thanh (Tùy chọn)")]
    public AudioSource scareSound; // Tiếng động khi ma xuất hiện

    [Header("Debug")]
    public bool showDebugInfo = true;

    private bool hasTriggered = false; // Chỉ kích hoạt 1 lần

    void Start()
    {
        // Kiểm tra setup
        if (enemyPrefab == null && showDebugInfo)
        {
            Debug.LogWarning($"HauntTrigger ({gameObject.name}): Enemy Prefab chưa được gán!");
        }
        else if (enemyPrefab != null && showDebugInfo)
        {
            Debug.Log($"HauntTrigger ({gameObject.name}): Enemy Prefab đã được gán: {enemyPrefab.name}");
        }

        // Kiểm tra Collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"HauntTrigger ({gameObject.name}): Không có Collider!");
        }
        else
        {
            if (!col.isTrigger)
            {
                Debug.LogError($"HauntTrigger ({gameObject.name}): Collider chưa được đặt Is Trigger = true!");
            }
            else if (showDebugInfo)
            {
                Debug.Log($"HauntTrigger ({gameObject.name}): Collider OK - Is Trigger = true");
            }
        }

        // Kiểm tra vị trí
        if (showDebugInfo)
        {
            Debug.Log($"HauntTrigger ({gameObject.name}): Vị trí = {transform.position}");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // LUÔN log để debug (không cần check showDebugInfo)
        Debug.Log($"[HauntTrigger] OnTriggerEnter được gọi! Vật thể: {other.name}, Tag: {other.tag}, GameObject: {other.gameObject.name}");

        if (showDebugInfo)
        {
            Debug.Log($"HauntTrigger ({gameObject.name}): Có vật thể đi vào! Tag: {other.tag}, Name: {other.name}");
        }

        // Kiểm tra xem người đi vào có phải Player không
        bool isPlayer = other.CompareTag("Player");
        
        Debug.Log($"[HauntTrigger] Kiểm tra Player: isPlayer = {isPlayer}, hasTriggered = {hasTriggered}");

        if (isPlayer && !hasTriggered)
        {
            Debug.Log($"[HauntTrigger] ✅ Player đã đi vào trigger! Bắt đầu spawn ma...");
            
            if (showDebugInfo)
            {
                Debug.Log("HauntTrigger: Player đã đi vào trigger!");
            }

            SpawnGhostBehindPlayer(other.transform);
            hasTriggered = true; // Khóa lại không cho spawn nữa
            
            // Tự hủy cái bẫy này sau 1 giây để dọn rác
            Destroy(gameObject, 1f); 
        }
        else if (hasTriggered)
        {
            Debug.Log("[HauntTrigger] Đã trigger rồi, bỏ qua.");
        }
        else if (!isPlayer)
        {
            Debug.LogWarning($"[HauntTrigger] Vật thể đi vào không phải Player! Tag: '{other.tag}' (Cần Tag = 'Player')");
        }
    }

    // Thêm method này để test - sẽ log mỗi frame khi có vật thể trong trigger
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered && showDebugInfo)
        {
            Debug.Log($"[HauntTrigger] Player đang ở trong trigger! Vị trí Player: {other.transform.position}");
        }
    }

    void SpawnGhostBehindPlayer(Transform playerTransform)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("HauntTrigger: Enemy Prefab chưa được gán!");
            return;
        }

        // Tính vị trí sau lưng: Vị trí Player - (Hướng mặt * Khoảng cách)
        Vector3 spawnPos = playerTransform.position - (playerTransform.forward * spawnDistanceBehind);
        
        // Giữ độ cao của ma bằng độ cao của Player (để ko bị chìm xuống đất)
        spawnPos.y = playerTransform.position.y;

        // Tạo ra con ma
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        if (scareSound != null) 
        {
            scareSound.Play();
        }

        Debug.Log($"Đã có ma bám theo sau lưng! Vị trí spawn: {spawnPos}");
    }
}

