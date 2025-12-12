using UnityEngine;

/// <summary>
/// Script cho Ma (Paper Enemy) - Phiên bản Truy Sát
/// AI đuổi theo Player như "tên lửa dẫn đường", lao thẳng về phía player và chụm lại vây quanh
/// Dừng lại khi bị nhìn thấy, và sợ đèn lồng của player
/// </summary>
public class PaperEnemy : MonoBehaviour
{
    [Header("Mục tiêu")]
    public Transform player; // Sẽ tự tìm Player
    public LanternSystem lanternSystem; // Hệ thống đèn của player

    [Header("Cài đặt Truy đuổi")]
    public float moveSpeed = 1.5f;     // Tốc độ chạy (giảm xuống để chúng vây quanh chậm hơn)
    public float stopDistance = 1.5f;  // Khoảng cách dừng lại (để chúng vây quanh chứ ko xuyên qua người)
    public float startDelay = 3.0f;    // Thời gian chờ lúc vào game (3 giây)
    
    [Header("Game Over Detection")]
    public float catchDistance = 1.0f; // Khoảng cách để bắt player (backup method nếu trigger không hoạt động)
    public bool useDistanceCheck = true; // Sử dụng kiểm tra khoảng cách như backup
    
    [Header("Trạng thái")]
    public bool isVisible = false; // Có đang bị nhìn thấy ko?
    public bool isStunned = false; // Có đang bị đèn chiếu ko?
    private bool canMove = false;  // Biến cờ: Được phép di chuyển chưa?

    [Header("Hiệu ứng Rung Lắc")]
    public float shakeAmount = 0.05f; // Độ rung khi bị đèn chiếu

    private Renderer rend;
    private Rigidbody rb;
    private Vector3 originalPosition; // Lưu vị trí gốc để rung lắc

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
        
        // Tự tìm Player nếu chưa gắn
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) 
            {
                player = p.transform;
                Debug.Log("PaperEnemy: Đã tìm thấy Player!");
            }
            else
            {
                Debug.LogError("PaperEnemy: Không tìm thấy Player! Đảm bảo Player có Tag = 'Player'");
            }
        }

        // Tự tìm LanternSystem nếu chưa gán
        if (lanternSystem == null && player != null)
        {
            lanternSystem = player.GetComponent<LanternSystem>();
            if (lanternSystem == null)
            {
                lanternSystem = player.GetComponentInChildren<LanternSystem>();
            }
        }

        // Nếu có Rigidbody, đặt nó thành kinematic để tránh va chạm vật lý
        // Điều này giúp các hình nhân không bị đẩy nhau lung tung
        if (rb != null)
        {
            rb.isKinematic = true; // Dùng transform.position thay vì physics
            rb.useGravity = false; // Ma giấy không cần trọng lực
        }
        
        // Bắt đầu đếm ngược 3 giây trước khi ma bắt đầu đuổi
        StartCoroutine(StartDelayRoutine());
    }
    
    /// <summary>
    /// Coroutine đếm ngược 3 giây trước khi ma bắt đầu đuổi
    /// </summary>
    System.Collections.IEnumerator StartDelayRoutine()
    {
        // Chờ startDelay giây
        yield return new WaitForSeconds(startDelay);
        
        // Cho phép di chuyển
        canMove = true;
        Debug.Log("PaperEnemy: Ma bắt đầu đi săn!");
    }

    void Update()
    {
        // Nếu chưa hết thời gian delay HOẶC game đã over -> Không làm gì cả
        if (!canMove)
        {
            return; // Đợi hết 3 giây
        }
        
        // Kiểm tra game over
        if (GameController.instance != null && GameController.instance.IsGameOver())
        {
            return; // Game đã over, không di chuyển nữa
        }
        
        if (player == null)
        {
            // Thử tìm lại Player
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) 
            {
                player = p.transform;
            }
            return;
        }

        // Tìm lại LanternSystem nếu chưa có
        if (lanternSystem == null)
        {
            lanternSystem = player.GetComponent<LanternSystem>();
            if (lanternSystem == null)
            {
                lanternSystem = player.GetComponentInChildren<LanternSystem>();
            }
        }

        CheckVisibility();
        CheckLanternProtection();

        // CHỈ DI CHUYỂN KHI: Không bị nhìn + Không bị đèn chiếu
        if (!isVisible && !isStunned)
        {
            ChasePlayer();
        }
        else if (isStunned)
        {
            // Ma bị đèn chiếu -> Rung lắc tại chỗ
            ShakeGhost();
        }
        
        // Kiểm tra khoảng cách với player (backup method nếu trigger không hoạt động)
        if (useDistanceCheck)
        {
            CheckPlayerCatch();
        }
    }
    
    /// <summary>
    /// Kiểm tra xem ma có chạm vào player không (backup method)
    /// </summary>
    void CheckPlayerCatch()
    {
        if (player == null) return;
        if (isStunned) return; // Nếu đang bị stun, không thể bắt player
        
        float distance = Vector3.Distance(transform.position, player.position);
        
        // Nếu quá gần player
        if (distance <= catchDistance)
        {
            Debug.Log($"PaperEnemy: Ma đã đến quá gần player! Khoảng cách: {distance:F2}m");
            TriggerGameOver();
        }
    }
    
    /// <summary>
    /// Gọi Game Over (dùng chung cho cả trigger và distance check)
    /// </summary>
    void TriggerGameOver()
    {
        if (GameController.instance != null)
        {
            GameController.instance.GameOver();
            Debug.Log("PaperEnemy: Ma đã bắt được Player! (Game Over triggered)");
        }
        else
        {
            Debug.LogError("PaperEnemy: Không tìm thấy GameController! Hãy tạo GameController trong scene.");
        }
    }

    /// <summary>
    /// Logic truy đuổi như "tên lửa dẫn đường" - lao thẳng về phía player
    /// </summary>
    void ChasePlayer()
    {
        // KIỂM TRA TRƯỚC: Nếu đang trong vùng an toàn, KHÔNG di chuyển
        if (isStunned)
        {
            return; // Dừng lại, không di chuyển
        }
        
        // 1. Xác định vị trí mục tiêu (Chỉ lấy X và Z, giữ nguyên độ cao Y của ma)
        // Điều này giúp ma không bị ngửa mặt lên trời hoặc cắm đầu xuống đất
        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);

        // 2. Xoay mặt về phía Player
        transform.LookAt(targetPosition);

        // 3. Tính khoảng cách
        float distance = Vector3.Distance(transform.position, player.position);

        // 4. Kiểm tra lại vùng an toàn TRƯỚC KHI di chuyển
        // Đảm bảo ma không di chuyển vào vùng an toàn
        if (lanternSystem != null && lanternSystem.IsInSafeZone(transform.position))
        {
            isStunned = true;
            return; // Dừng lại ngay lập tức
        }

        // 5. Nếu còn xa hơn khoảng cách dừng -> Lao tới
        if (distance > stopDistance)
        {
            // Tính vị trí mới trước khi di chuyển
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            
            // Kiểm tra xem vị trí mới có nằm trong vùng an toàn không
            if (lanternSystem != null && lanternSystem.IsInSafeZone(newPosition))
            {
                // Nếu vị trí mới nằm trong vùng an toàn, dừng lại ở biên giới
                isStunned = true;
                return;
            }
            
            // An toàn, di chuyển
            transform.position = newPosition;
            // Cập nhật vị trí gốc để rung lắc
            originalPosition = transform.position;
        }
    }

    /// <summary>
    /// Kiểm tra xem ma có đang bị player nhìn thấy không
    /// </summary>
    void CheckVisibility()
    {
        if (rend != null)
        {
            // Dùng Renderer.isVisible để kiểm tra nhanh
            isVisible = rend.isVisible;
        }
        else
        {
            isVisible = false;
        }
    }

    /// <summary>
    /// Kiểm tra xem ma có nằm trong vùng sáng bảo vệ của đèn lồng không
    /// </summary>
    void CheckLanternProtection()
    {
        bool wasStunned = isStunned;
        isStunned = false; // Reset, sẽ set lại nếu có vùng an toàn
        
        // 1. Kiểm tra đèn lồng của player
        if (lanternSystem != null)
        {
            if (lanternSystem.IsInSafeZone(transform.position))
            {
                isStunned = true;
            }
        }
        
        // 2. Kiểm tra đèn tĩnh trên đường (StreetLamp)
        StreetLamp[] streetLamps = FindObjectsOfType<StreetLamp>();
        foreach (var lamp in streetLamps)
        {
            if (lamp != null && lamp.IsInProtectionZone(transform.position))
            {
                isStunned = true;
                break; // Chỉ cần 1 đèn bảo vệ là đủ
            }
        }
        
        // Debug log khi vào/ra khỏi vùng an toàn
        if (isStunned && !wasStunned)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            Debug.Log($"PaperEnemy: Ma đã vào vùng an toàn! Khoảng cách: {distance:F2}m");
        }
        else if (!isStunned && wasStunned)
        {
            Debug.Log("PaperEnemy: Ma đã ra khỏi vùng an toàn!");
        }
    }

    /// <summary>
    /// Hiệu ứng rung lắc khi bị chiếu đèn (Ma đang đau đớn)
    /// </summary>
    void ShakeGhost()
    {
        // Đảm bảo ma không di chuyển vào sâu hơn trong vùng an toàn
        // Nếu ma đang trong vùng an toàn, đẩy nó ra ngoài một chút
        if (lanternSystem != null && player != null && lanternSystem.lanternLight != null)
        {
            Vector3 lightPos = lanternSystem.lanternLight.transform.position;
            Vector3 enemyPos = transform.position;
            Vector3 directionAway = (enemyPos - lightPos).normalized;
            
            // Tính khoảng cách hiện tại từ Light
            float currentDistance = Vector3.Distance(enemyPos, lightPos);
            float safeDist = lanternSystem.safeDistance;
            
            // Nếu quá gần, đẩy ra ngoài một chút
            if (currentDistance < safeDist * 0.8f) // Nếu vào sâu hơn 80% vùng an toàn
            {
                // Đẩy ra ngoài một chút
                Vector3 pushOutPosition = lightPos + directionAway * (safeDist * 0.9f);
                pushOutPosition.y = transform.position.y; // Giữ nguyên độ cao
                transform.position = pushOutPosition;
                originalPosition = transform.position;
            }
        }
        
        // Rung lắc nhẹ tại chỗ
        Vector3 shakeOffset = Random.insideUnitSphere * shakeAmount;
        transform.position = originalPosition + shakeOffset;
    }
    
    // --- TƯƠNG TÁC VỚI ĐÈN CŨ (Giữ lại để tương thích nếu có code khác đang dùng) ---
    /// <summary>
    /// Được gọi khi Ma đi vào vùng sáng của đèn (Hệ thống cũ - trigger)
    /// </summary>
    public void EnterLightZone()
    {
        isStunned = true;
        Debug.Log("Ma bị đèn làm choáng!");
    }

    /// <summary>
    /// Được gọi khi Ma đi ra khỏi vùng sáng (Hệ thống cũ - trigger)
    /// </summary>
    public void ExitLightZone()
    {
        isStunned = false;
    }
    
    // Giữ lại các method cũ để tương thích (nếu có code khác đang dùng)
    /// <summary>
    /// Lấy tốc độ hiện tại (để các script khác sử dụng)
    /// </summary>
    public float GetCurrentSpeed()
    {
        return isStunned ? 0f : moveSpeed;
    }
    
    /// <summary>
    /// Kiểm tra xem Ma có đang trong vùng sáng không
    /// </summary>
    public bool IsInLightZone()
    {
        return isStunned;
    }
    
    /// <summary>
    /// Xử lý khi ma chạm vào Player (dùng Trigger)
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PaperEnemy] OnTriggerEnter: {other.name}, Tag: {other.tag}");
        
        // Kiểm tra xem vật va chạm có phải là Player không
        if (other.CompareTag("Player"))
        {
            Debug.Log("[PaperEnemy] ✅ Player detected in trigger! Triggering Game Over...");
            TriggerGameOver();
        }
        else
        {
            Debug.LogWarning($"[PaperEnemy] OnTriggerEnter với vật thể không phải Player: {other.name}, Tag: '{other.tag}'");
        }
    }
    
    /// <summary>
    /// Xử lý khi ma chạm vào Player (dùng Collision - nếu không dùng Trigger)
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[PaperEnemy] OnCollisionEnter: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        
        // Kiểm tra xem vật va chạm có phải là Player không
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("[PaperEnemy] ✅ Player detected in collision! Triggering Game Over...");
            TriggerGameOver();
        }
        else
        {
            Debug.LogWarning($"[PaperEnemy] OnCollisionEnter với vật thể không phải Player: {collision.gameObject.name}, Tag: '{collision.gameObject.tag}'");
        }
    }
    
    /// <summary>
    /// Vẽ gizmo để debug trong Scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (lanternSystem != null && player != null)
        {
            // Vẽ line từ ma đến player
            Gizmos.color = isStunned ? Color.red : (canMove ? Color.green : Color.gray);
            Gizmos.DrawLine(transform.position, player.position);
            
            // Vẽ sphere nhỏ ở vị trí ma
            Gizmos.color = isStunned ? Color.yellow : (canMove ? Color.cyan : Color.gray);
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            
            // Nếu đang bị stun, vẽ text
            if (isStunned)
            {
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, "STUNNED!");
                #endif
            }
            // Nếu chưa được phép di chuyển
            else if (!canMove)
            {
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, "WAITING...");
                #endif
            }
        }
    }
}



