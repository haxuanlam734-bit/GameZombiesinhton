using UnityEngine;
using System.Collections.Generic;

// Phiên bản nâng cấp của ZombieSpawner để script Editor có thể tham chiếu
public class ZombieSpawner : MonoBehaviour
{
    [Tooltip("Danh sách các điểm sẽ sinh ra Zombie")]
    public List<Transform> spawnPoints = new List<Transform>();

    // Các biến này được UIManager sử dụng
    public int currentWave = 1;
    public int zombiesLeft = 0;
    public float timeToNextWave = 5f;
}
