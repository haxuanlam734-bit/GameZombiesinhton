using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Quản lý các chức năng của các nút bấm trong màn hình Main Menu.
/// Gắn script này vào một GameObject trong Scene Main Menu (ví dụ: MainMenuCanvas).
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Tên Scene Game")]
    [Tooltip("Nhập chính xác tên file Scene chứa màn chơi chính của bạn.")]
    [SerializeField] private string gameSceneName = "GameScene";

    /// <summary>
    /// Bắt đầu game bằng cách tải Scene màn chơi chính.
    /// Gán hàm này vào sự kiện OnClick của nút "Play".
    /// </summary>
    public void PlayGame()
    {
        // Tải scene có tên đã được chỉ định
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// Điều chỉnh âm lượng tổng của game. 
    /// Gán hàm này vào sự kiện OnValueChanged của một Slider trong màn hình cài đặt.
    /// </summary>
    /// <param name="volume">Giá trị volume từ Slider (0.0 đến 1.0).</param>
    public void SetVolume(float volume)
    {
        // AudioListener.volume ảnh hưởng đến tất cả các nguồn âm thanh trong game
        AudioListener.volume = volume;
    }

    /// <summary>
    /// Thoát ứng dụng.
    /// Gán hàm này vào sự kiện OnClick của nút "Quit".
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Thoát game!"); // Dòng này chỉ để kiểm tra trong Editor

        // Thoát ứng dụng (chỉ hoạt động trong bản build, không hoạt động trong Editor)
        Application.Quit();
    }
}
