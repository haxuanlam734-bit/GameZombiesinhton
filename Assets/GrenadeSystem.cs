using UnityEngine;

/// <summary>
/// Hệ thống cho phép người chơi ném lựu đạn.
/// Gắn script này vào Player.
/// </summary>
public class GrenadeSystem : MonoBehaviour
{
    [Header("Thiết lập Lựu đạn")]
    [Tooltip("Prefab của quả lựu đạn sẽ được ném")]
    [SerializeField] private GameObject grenadePrefab;
    [Tooltip("Lực ném ban đầu")]
    [SerializeField] private float throwForce = 15f;
    [Tooltip("Số lựu đạn ban đầu")]
    [SerializeField] private int initialGrenadeCount = 3;

    [Header("Điểm ném")]
    [Tooltip("Vị trí lựu đạn sẽ được sinh ra khi ném. Thường là một điểm rỗng trước mặt Camera.")]
    [SerializeField] private Transform throwPoint;

    private int currentGrenadeCount;
    public int CurrentGrenadeCount { get { return currentGrenadeCount; } } // Để UI có thể đọc

    void Start()
    {
        currentGrenadeCount = initialGrenadeCount;
        if (throwPoint == null)
        {
            // Nếu không có điểm ném, mặc định dùng camera
            throwPoint = Camera.main.transform;
        }
    }

    void Update()
    {
        // Xử lý input từ phím 'G' trên PC
        if (Input.GetKeyDown(KeyCode.G))
        {
            ThrowGrenade();
        }
    }

    /// <summary>
    /// Ném một quả lựu đạn nếu còn.
    /// </summary>
    public void ThrowGrenade()
    {
        // Kiểm tra xem còn lựu đạn không
        if (currentGrenadeCount > 0)
        {
            currentGrenadeCount--;

            // Tạo ra quả lựu đạn tại điểm ném
            GameObject grenade = Instantiate(grenadePrefab, throwPoint.position, throwPoint.rotation);
            
            // Lấy component Rigidbody của lựu đạn
            Rigidbody rb = grenade.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Đẩy một lực về phía trước
                rb.AddForce(throwPoint.forward * throwForce, ForceMode.VelocityChange);
            }

            Debug.Log("Đã ném lựu đạn! Còn lại: " + currentGrenadeCount);
            // Cập nhật UI ở đây
            // UIManager.Instance.UpdateGrenadeCount(currentGrenadeCount);
        }
        else
        {
            Debug.Log("Hết lựu đạn!");
        }
    }

    /// <summary>
    /// (Tùy chọn) Hàm để cộng thêm lựu đạn cho người chơi.
    /// </summary>
    public void AddGrenades(int amount)
    {
        currentGrenadeCount += amount;
    }
}
