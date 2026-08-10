using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Điều khiển các trạng thái animation của Zombie dựa trên các hành động của nó.
/// Yêu cầu GameObject phải có Animator, NavMeshAgent, và Health.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Health))]
public class ZombieAnimationController : MonoBehaviour
{
    // Tham chiếu đến các component cần thiết
    private Animator animator;
    private NavMeshAgent agent;
    private Health health;

    private bool hasTriggeredDeath = false; // Cờ để đảm bảo Trigger "Die" chỉ được gọi 1 lần

    private void Awake()
    {
        // Lấy các component trên cùng GameObject
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
    }

    private void Update()
    {
        // Nếu Zombie đã chết
        if (health.IsDead)
        {
            // và chưa gọi trigger "Die"
            if (!hasTriggeredDeath)
            {
                // Thì gọi trigger và bật cờ
                animator.SetTrigger("Die");
                hasTriggeredDeath = true;
            }
            return; // Dừng các xử lý khác khi đã chết
        }

        // --- Xử lý Animation Di chuyển ---
        // Lấy độ lớn của vector vận tốc để biết Zombie có đang di chuyển không
        float speed = agent.velocity.magnitude;
        // Đặt giá trị cho biến "IsMoving" trong Animator
        // Nếu tốc độ > 0.1, IsMoving = true, ngược lại là false.
        animator.SetBool("IsMoving", speed > 0.1f);
    }

    /// <summary>
    /// Kích hoạt animation tấn công. Hàm này sẽ được gọi từ script ZombieAI.
    /// </summary>
    public void TriggerAttack()
    {
        // Chỉ tấn công nếu chưa chết
        if (!health.IsDead)
        {
            animator.SetTrigger("Attack");
        }
    }
}
