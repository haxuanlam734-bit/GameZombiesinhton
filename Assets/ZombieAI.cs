using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// 1. Yêu cầu đối tượng phải có các component cần thiết
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class ZombieAI : MonoBehaviour
{
    #region Biến và Tham chiếu

    [Header("Thông số Tấn công")]
    [Tooltip("Khoảng cách Zombie bắt đầu tấn công")]
    [SerializeField] private float attackRange = 2f;
    [Tooltip("Sát thương mỗi cú đánh")]
    [SerializeField] private float attackDamage = 15f;
    [Tooltip("Thời gian nghỉ giữa mỗi lần tấn công")]
    [SerializeField] private float attackCooldown = 1.5f;

    [Header("Thông số Di chuyển")]
    [Tooltip("Tốc độ di chuyển của Zombie")]
    [SerializeField] private float moveSpeed = 3.5f;

    // Tham chiếu đến các components
    private NavMeshAgent agent;
    private Health health; // Health của chính Zombie này
    private Transform playerTransform;
    private Health playerHealth; // Health của Player

    // Biến nội bộ
    private float nextAttackTime = 0f;
    private const float pathUpdateInterval = 0.25f; // Tần suất cập nhật đường đi

    #endregion

    #region Vòng đời Unity

    private void Awake()
    {
        // Lấy các component trên cùng một GameObject
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        // Cài đặt tốc độ cho NavMeshAgent
        agent.speed = moveSpeed;

        // 3. Tự động tìm kiếm Mục tiêu (Player)
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerHealth = playerObject.GetComponent<Health>();
        }
        else
        {
            Debug.LogError("Không tìm thấy đối tượng có tag 'Player' trong Scene!");
            // Vô hiệu hóa AI nếu không tìm thấy Player
            enabled = false;
            return;
        }

        // 4. Bắt đầu Coroutine tối ưu việc cập nhật đường đi
        StartCoroutine(UpdatePath());

        // Đăng ký hàm OnDeath của Health component
        health.OnDeath.AddListener(HandleDeath);
    }

    private void Update()
    {
        // 5. Xử lý Trạng thái & Tấn công

        // Nếu Zombie hoặc Player đã chết, dừng mọi hành động
        if (health.IsDead || playerHealth == null || playerHealth.IsDead)
        {
            return; // Dừng hàm Update tại đây
        }

        // Tính khoảng cách đến Player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Nếu Player ở ngoài tầm đánh, tiếp tục đuổi theo
        if (distanceToPlayer > attackRange)
        {
            agent.isStopped = false; // Cho phép agent di chuyển
        }
        // Nếu Player trong tầm đánh, dừng lại và tấn công
        else
        {
            agent.isStopped = true; // Dừng di chuyển để tấn công

            // Kiểm tra xem đã đến lúc tấn công chưa
            if (Time.time >= nextAttackTime)
            {
                Attack();
                // Đặt lại thời gian cho lần tấn công tiếp theo
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    #endregion

    #region Logic AI

    /// <summary>
    /// Coroutine để cập nhật đường đi của Zombie một cách tối ưu.
    /// </summary>
    private IEnumerator UpdatePath()
    {
        // Vòng lặp vô tận để liên tục cập nhật đường đi cho đến khi Zombie chết
        while (!health.IsDead)
        {
            // Chỉ cập nhật khi agent còn hoạt động và có mục tiêu
            if (agent.isOnNavMesh && playerTransform != null)
            {
                agent.SetDestination(playerTransform.position);
            }
            
            // Chờ một khoảng thời gian trước khi lặp lại
            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }

    /// <summary>
    /// Thực hiện hành động tấn công Player.
    /// </summary>
    private void Attack()
    {
        //Debug.Log("Zombie tấn công Player!");
        // Quay mặt về phía Player khi tấn công
        if(playerTransform != null) 
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = lookRotation;
        }
         
        playerHealth.TakeDamage(attackDamage);

        // Hiển thị vệt máu trên UI
        DamageIndicator.Instance?.Show(transform.position);

        // TODO: Thêm animation tấn công ở đây
    }

    /// <summary>
    /// Hàm được gọi khi Zombie chết (thông qua sự kiện OnDeath).
    /// </summary>
    private void HandleDeath()
    {
        // Dừng di chuyển ngay lập tức
        agent.isStopped = true;
        // Vô hiệu hóa component AI này để dừng xử lý Update
        enabled = false; 
        // Bạn có thể thêm các logic khác ở đây, ví dụ như bật ragdoll
    }

    #endregion
}
