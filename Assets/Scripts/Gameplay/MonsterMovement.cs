using UnityEngine;
using UnityEngine.AI; // Bắt buộc có dòng này để dùng AI Navigation

/// <summary>
/// Script điều khiển con quái bò liên tục về phía mục tiêu
/// Sử dụng NavMesh Agent để di chuyển theo NavMesh (mặt đất màu xanh)
/// </summary>
public class MonsterMovement : MonoBehaviour
{
    [Header("Cài đặt")]
    [Tooltip("Mục tiêu để bò tới (Kéo Player vào đây)")]
    public Transform target;       // Kéo Player vào đây
    
    [Header("Tùy chọn")]
    [Tooltip("Tự động tìm Player nếu chưa gán target")]
    public bool autoFindPlayer = true;
    
    private NavMeshAgent agent;    // Biến điều khiển AI

    void Start()
    {
        // Tự động tìm NavMeshAgent trên người con quái
        agent = GetComponent<NavMeshAgent>();
        
        if (agent == null)
        {
            Debug.LogError("[MonsterMovement] ⚠️ Không tìm thấy NavMeshAgent component! Hãy Add Component -> NavMesh Agent vào con quái.");
            return;
        }
        
        // Tự động tìm Player nếu chưa gán target
        if (target == null && autoFindPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log($"[MonsterMovement] ✅ Đã tự động tìm thấy Player: {player.name}");
            }
            else
            {
                Debug.LogWarning("[MonsterMovement] ⚠️ Không tìm thấy Player! Hãy kéo Player vào field 'Target' hoặc đảm bảo Player có Tag 'Player'.");
            }
        }
        
        // Kiểm tra NavMesh có được bake chưa
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("[MonsterMovement] ⚠️ NavMeshAgent không đứng trên NavMesh! Hãy đảm bảo:");
            Debug.LogError("[MonsterMovement] 1. Đã tạo NavMeshSurface component trên mặt đất");
            Debug.LogError("[MonsterMovement] 2. Đã bấm nút 'Bake' trong NavMeshSurface component");
            Debug.LogError("[MonsterMovement] 3. Con quái phải đứng trên vùng màu xanh (NavMesh)");
        }
        
        // Kiểm tra Animator và tắt Root Motion nếu có
        Animator animator = GetComponent<Animator>();
        if (animator != null && animator.applyRootMotion)
        {
            Debug.LogWarning("[MonsterMovement] ⚠️ Animator đang bật 'Apply Root Motion'! Nên tắt để NavMeshAgent điều khiển di chuyển.");
        }
    }

    void Update()
    {
        if (target != null && agent != null && agent.isOnNavMesh)
        {
            // AI sẽ tự tính toán đường đi trên mặt đất màu xanh (NavMesh)
            // Nó sẽ tự động leo dốc, tránh cây cối để đến chỗ Player
            agent.SetDestination(target.position);
        }
    }
    
    /// <summary>
    /// Set mục tiêu mới (có thể gọi từ script khác)
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log($"[MonsterMovement] Đã đổi mục tiêu: {newTarget.name}");
    }
    
    /// <summary>
    /// Set tốc độ di chuyển (có thể gọi từ script khác)
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        if (agent != null)
        {
            agent.speed = newSpeed;
            Debug.Log($"[MonsterMovement] Đã đổi tốc độ: {newSpeed}");
        }
    }
    
    /// <summary>
    /// Dừng di chuyển (dừng agent)
    /// </summary>
    public void Stop()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            Debug.Log("[MonsterMovement] Đã dừng di chuyển");
        }
    }
    
    /// <summary>
    /// Tiếp tục di chuyển
    /// </summary>
    public void Resume()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            Debug.Log("[MonsterMovement] Đã tiếp tục di chuyển");
        }
    }
}

