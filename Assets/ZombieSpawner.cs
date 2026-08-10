using UnityEngine;

// Đây là một script giả lập để UIManager có thể hoạt động.
// Bạn sẽ cần thay thế nó bằng hệ thống Spawner hoàn chỉnh của mình.
public class ZombieSpawner : MonoBehaviour
{
    [Tooltip("Wave hiện tại")]
    public int currentWave = 1;

    [Tooltip("Số lượng zombie còn lại trong wave")]
    public int zombiesLeft = 10;

    [Tooltip("Thời gian đếm ngược đến wave tiếp theo")]
    public float timeToNextWave = 0f;
}
