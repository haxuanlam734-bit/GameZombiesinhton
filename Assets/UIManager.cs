using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý và cập nhật toàn bộ các thành phần Giao diện Người dùng (UI/HUD) trong game.
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Tham chiếu (References)

    [Header("Tham chiếu Logic Game")]
    [Tooltip("Component Health của Player")]
    [SerializeField] private Health playerHealth;
    [Tooltip("Component điều khiển súng của Player")]
    [SerializeField] private CrossPlatformGunController gunController;
    [Tooltip("Component quản lý việc sinh Zombie")]
    [SerializeField] private ZombieSpawner zombieSpawner;

    [Header("Giao diện Máu (Health)")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Giao diện Đạn (Ammo)")]
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Giao diện Wave")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI zombiesLeftText;
    [SerializeField] private TextMeshProUGUI waveCountdownText;

    [Header("Màn hình Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    #endregion

    #region Vòng đời Unity

    private void Start()
    {
        // Ẩn màn hình Game Over và text đếm ngược khi bắt đầu game
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (waveCountdownText != null)
        {
            waveCountdownText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Nếu player đã chết, chỉ xử lý logic Game Over
        if (playerHealth != null && playerHealth.IsDead)
        {
            HandleGameOver();
            return; // Dừng các cập nhật khác
        }

        // Cập nhật các thành phần UI
        UpdateHealthUI();
        UpdateAmmoUI();
        UpdateWaveUI();
    }

    #endregion

    #region Cập nhật UI

    /// <summary>
    /// Cập nhật UI hiển thị máu của người chơi.
    /// </summary>
    private void UpdateHealthUI()
    {
        if (playerHealth == null) return;

        if (healthSlider != null)
        {
            // Cập nhật giá trị của thanh slider máu
            healthSlider.value = playerHealth.CurrentHealth / playerHealth.MaxHealth;
        }
        if (healthText != null)
        {
            // Cập nhật text hiển thị số máu
            healthText.text = $"{Mathf.CeilToInt(playerHealth.CurrentHealth)} / {Mathf.CeilToInt(playerHealth.MaxHealth)}";
        }
    }

    /// <summary>
    /// Cập nhật UI hiển thị thông tin đạn.
    /// </summary>
    private void UpdateAmmoUI()
    {
        if (gunController == null || ammoText == null) return;

        // Nếu đang nạp đạn, hiển thị chữ "RELOADING..."
        if (gunController.IsReloading)
        {
            ammoText.text = "RELOADING...";
        }
        else
        {
            // Hiển thị số đạn hiện tại / tổng số đạn
            ammoText.text = $"{gunController.CurrentAmmo} / {gunController.MaxAmmo}";
        }
    }

    /// <summary>
    /// Cập nhật UI hiển thị thông tin về đợt tấn công (wave).
    /// </summary>
    private void UpdateWaveUI()
    {
        if (zombieSpawner == null) return;

        if (waveText != null)
        {
            waveText.text = $"WAVE {zombieSpawner.currentWave}";
        }
        if (zombiesLeftText != null)
        { 
            zombiesLeftText.text = $"Zombies: {zombieSpawner.zombiesLeft}";
        }

        // Xử lý text đếm ngược
        if (waveCountdownText != null)
        {
            // Nếu đang trong thời gian đếm ngược
            if (zombieSpawner.timeToNextWave > 0)
            {
                waveCountdownText.gameObject.SetActive(true);
                waveCountdownText.text = $"Next Wave in: {Mathf.CeilToInt(zombieSpawner.timeToNextWave)}s";
            }
            else
            {
                // Nếu không thì ẩn đi
                waveCountdownText.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Xử lý hiển thị màn hình Game Over.
    /// </summary>
    private void HandleGameOver()
    {
        if (gameOverPanel != null && !gameOverPanel.activeInHierarchy)
        {
            gameOverPanel.SetActive(true);
            // Mở khóa con trỏ chuột để người chơi có thể bấm nút
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    #endregion

    #region Hàm chức năng Public

    /// <summary>
    /// Nạp lại Scene hiện tại, dùng cho nút "Chơi lại".
    /// </summary>
    public void RestartGame()
    {
        // Khóa lại con trỏ chuột trước khi load scene mới nếu cần
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}
