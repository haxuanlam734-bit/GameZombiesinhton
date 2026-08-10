// File: IDamageable.cs

/// <summary>
/// Interface cho bất kỳ đối tượng nào có thể nhận sát thương trong game (VD: Player, Enemy, vật thể có thể phá hủy).
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Hàm được gọi khi đối tượng nhận một lượng sát thương.
    /// </summary>
    /// <param name="amount">Lượng sát thương nhận vào.</param>
    void TakeDamage(float amount);
}
