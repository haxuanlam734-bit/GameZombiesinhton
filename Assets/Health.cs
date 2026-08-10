using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Quản lý hệ thống máu cho một đối tượng, có thể là Player, Enemy, hoặc bất cứ thứ gì có thể nhận sát thương.
/// Đồng thời triển khai interface IDamageable để chuẩn hóa việc nhận sát thương.
/// </summary>
public class Health : MonoBehaviour, IDamageable
{
    [Header("Thiết lập Máu")]
    [Tooltip("Lượng máu tối đa của đối tượng.")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Sự kiện")]
    [Tooltip("Sự kiện này được kích hoạt khi đối tượng chết. Dùng để gọi animation, âm thanh, hiệu ứng,...")]
    public UnityEvent OnDeath;

    // Biến nội bộ
    private float currentHealth;

    // Thuộc tính Public để các script khác có thể đọc giá trị
    public float CurrentHealth { get { return currentHealth; } }
    public float MaxHealth { get { return maxHealth; } }

    /// <summary>
    /// Thuộc tính (property) để kiểm tra xem đối tượng đã chết hay chưa. Chỉ có thể đọc (read-only).
    /// </summary>
    public bool IsDead { get; private set; } = false;

    private void Awake()
    {        
        // Khởi tạo máu ban đầu bằng máu tối đa
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Hàm nhận sát thương, được triển khai từ interface IDamageable.
    /// </summary>
    /// <param name="amount">Lượng sát thương nhận vào.</param>
    public void TakeDamage(float amount)
    {
        // Nếu đã chết rồi thì không nhận thêm sát thương
        if (IsDead)
        {
            return;
        }

        currentHealth -= amount;
        //Debug.Log(gameObject.name + " nhận " + amount + " sát thương, còn lại " + currentHealth + " máu.");

        // Kiểm tra xem máu đã hết chưa
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    /// <summary>
    /// Xử lý logic khi đối tượng chết.
    /// </summary>
    private void Die()
    {
        IsDead = true;
        Debug.Log(gameObject.name + " đã chết.");

        // Kích hoạt sự kiện OnDeath. Bất kỳ hàm nào được gán vào sự kiện này trong Inspector sẽ được gọi.
        OnDeath?.Invoke();
    }
}
