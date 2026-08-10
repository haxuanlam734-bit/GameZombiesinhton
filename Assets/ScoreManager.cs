using UnityEngine;

/// <summary>
/// Quản lý điểm số, số lượng kill và điểm cao nhất của người chơi.
/// Sử dụng Singleton pattern để dễ dàng truy cập từ bất kỳ đâu.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    #region Singleton

    // Biến static để lưu trữ instance duy nhất của ScoreManager
    public static ScoreManager Instance { get; private set; }

    private void Awake()
    {
        // Kiểm tra xem đã có instance nào tồn tại chưa
        if (Instance != null && Instance != this)
        {
            // Nếu có, hủy GameObject này đi để đảm bảo chỉ có 1 ScoreManager
            Destroy(gameObject);
        }
        else
        {
            // Nếu chưa có, gán instance là chính nó
            Instance = this;
            // (Tùy chọn) Giữ lại đối tượng này khi chuyển scene
            // DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    #region Biến và Thuộc tính

    // Biến private để lưu trữ điểm và kill
    private int score;
    private int killCount;
    private int highScore;

    // Thuộc tính public để các script khác có thể đọc giá trị một cách an toàn
    public int Score { get { return score; } }
    public int KillCount { get { return killCount; } }
    public int HighScore { get { return highScore; } }

    private const string HighScoreKey = "HighScore"; // Key để lưu high score vào PlayerPrefs

    #endregion

    private void Start()
    {
        // Lấy điểm cao nhất đã lưu khi bắt đầu game
        highScore = GetHighScore();
    }

    /// <summary>
    /// Cộng điểm và số kill khi người chơi tiêu diệt một mục tiêu.
    /// </summary>
    /// <param name="points">Số điểm nhận được.</param>
    public void AddKill(int points)
    {
        // Cộng điểm và kill
        killCount++;
        score += points;

        // Cập nhật điểm cao nhất nếu cần
        if (score > highScore)
        {
            highScore = score;
            // Lưu điểm cao nhất mới vào bộ nhớ của thiết bị
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save(); // Đảm bảo dữ liệu được ghi ngay lập tức
            Debug.Log("New High Score: " + highScore);
        }

        // Cập nhật UI ở đây (nếu cần)
        // Ví dụ: UIManager.Instance.UpdateScoreUI(score, killCount);
    }

    /// <summary>
    /// Lấy điểm cao nhất đã được lưu trong PlayerPrefs.
    /// </summary>
    /// <returns>Điểm cao nhất.</returns>
    private int GetHighScore()
    {
        // Đọc giá trị từ PlayerPrefs, nếu chưa có thì mặc định là 0
        return PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    /// <summary>
    /// (Tùy chọn) Hàm để reset điểm cao nhất.
    /// </summary>
    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
        highScore = 0;
        Debug.Log("High Score has been reset.");
    }
}
