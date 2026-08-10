using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Quản lý việc hiển thị vệt máu chỉ hướng bị tấn công.
/// Gắn vào một GameObject con của Canvas, GameObject này phải có component Image.
/// </summary>
[RequireComponent(typeof(Image))]
public class DamageIndicator : MonoBehaviour
{
    #region Singleton
    public static DamageIndicator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    [Tooltip("Thời gian hiển thị vệt máu trước khi mờ dần")]
    [SerializeField] private float displayTime = 1f;
    private Image indicatorImage;
    private Coroutine fadeCoroutine; // Để lưu trữ và quản lý coroutine đang chạy

    private Transform playerTransform;

    void Start()
    {
        // Lấy component Image và ẩn nó đi lúc đầu
        indicatorImage = GetComponent<Image>();
        indicatorImage.color = new Color(indicatorImage.color.r, indicatorImage.color.g, indicatorImage.color.b, 0);

        // Tìm transform của player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    /// <summary>
    /// Hàm công khai để hiển thị vệt máu từ một hướng cụ thể.
    /// </summary>
    /// <param name="attackerPosition">Vị trí của kẻ tấn công.</param>
    public void Show(Vector3 attackerPosition)
    {
        // Nếu có coroutine đang chạy, dừng nó lại
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        if (playerTransform == null) return;

        // --- Tính toán góc xoay của vệt máu ---
        // Lấy vector chỉ hướng từ player đến kẻ tấn công
        Vector3 directionToAttacker = (attackerPosition - playerTransform.position).normalized;
        directionToAttacker.y = 0; // Bỏ qua trục y để tính toán trên mặt phẳng 2D

        // Lấy vector hướng nhìn của player
        Vector3 playerForward = playerTransform.forward;
        playerForward.y = 0;

        // Tính góc giữa hướng nhìn của player và hướng của kẻ tấn công
        float angle = Vector3.SignedAngle(playerForward, directionToAttacker, Vector3.up);

        // Áp dụng góc xoay (trừ đi) cho trục Z của RectTransform
        transform.localRotation = Quaternion.Euler(0, 0, -angle);

        // Bắt đầu coroutine để hiển thị và làm mờ
        fadeCoroutine = StartCoroutine(FadeIndicator());
    }

    private IEnumerator FadeIndicator()
    {
        // Hiển thị rõ vệt máu
        indicatorImage.color = new Color(indicatorImage.color.r, indicatorImage.color.g, indicatorImage.color.b, 1);

        // Chờ một chút
        yield return new WaitForSeconds(0.1f);

        // Bắt đầu làm mờ dần trong khoảng thời gian `displayTime`
        float timer = 0;
        while (timer < displayTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, timer / displayTime);
            indicatorImage.color = new Color(indicatorImage.color.r, indicatorImage.color.g, indicatorImage.color.b, alpha);
            yield return null;
        }

        // Đảm bảo alpha về 0 khi kết thúc
        indicatorImage.color = new Color(indicatorImage.color.r, indicatorImage.color.g, indicatorImage.color.b, 0);
    }
}
