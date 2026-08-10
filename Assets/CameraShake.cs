using UnityEngine;
using System.Collections;

/// <summary>
/// Tạo hiệu ứng rung cho camera. Sử dụng Singleton pattern.
/// Gắn script này vào Main Camera.
/// </summary>
public class CameraShake : MonoBehaviour
{
    #region Singleton

    public static CameraShake Instance { get; private set; }

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

    private Vector3 originalPosition;

    private void Start()
    {
        // Lưu lại vị trí ban đầu của camera
        originalPosition = transform.localPosition;
    }

    /// <summary>
    /// Kích hoạt hiệu ứng rung camera.
    /// </summary>
    /// <param name="duration">Thời gian rung (giây).</param>
    /// <param name="magnitude">Cường độ rung.</param>
    public void Shake(float duration, float magnitude)
    {
        // Bắt đầu một Coroutine để xử lý việc rung
        StartCoroutine(DoShake(duration, magnitude));
    }

    /// <summary>
    /// Coroutine thực hiện việc rung camera.
    /// </summary>
    private IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsedTime = 0f;

        // Vòng lặp trong suốt thời gian rung
        while (elapsedTime < duration)
        {
            // Tạo một vị trí x, y, z ngẫu nhiên bên trong một hình cầu đơn vị, sau đó nhân với cường độ
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            
            // Cập nhật vị trí local của camera bằng vị trí gốc + độ lệch ngẫu nhiên
            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);
            
            // Tăng thời gian đã trôi qua
            elapsedTime += Time.deltaTime;
            
            // Chờ đến frame tiếp theo
            yield return null;
        }

        // Khi hết thời gian, đảm bảo camera trở về đúng vị trí ban đầu
        transform.localPosition = originalPosition;
    }
}
