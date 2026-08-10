using System.Collections;
using UnityEngine;

public class CrossPlatformGunController : MonoBehaviour
{
    #region Các chỉ số của súng

    [Header("Thông số Súng")]
    [Tooltip("Sát thương mỗi viên đạn")]
    [SerializeField] private float damage = 25f;
    [Tooltip("Tầm bắn xa nhất của súng")]
    [SerializeField] private float range = 100f;
    [Tooltip("Thời gian nghỉ giữa mỗi lần bắn (giây)")]
    [SerializeField] private float fireRate = 0.15f;
    [Tooltip("Số lượng đạn tối đa trong một băng")]
    [SerializeField] private int maxAmmo = 30;
    [Tooltip("Thời gian nạp lại đạn (giây)")]
    [SerializeField] private float reloadTime = 2f;

    [Header("Hiệu ứng & Prefabs")]
    [Tooltip("Prefab hiệu ứng khi đạn va chạm")]
    [SerializeField] private GameObject bulletImpactPrefab;
    [Tooltip("Hiệu ứng nổ ở đầu nòng súng")]
    [SerializeField] private ParticleSystem muzzleFlashFX;

    // Biến nội bộ
    private int currentAmmo;
    private float nextTimeToFire = 0f;
    private bool isReloading = false;

    // Thuộc tính public để UIManager có thể truy cập
    public int CurrentAmmo { get { return currentAmmo; } }
    public int MaxAmmo { get { return maxAmmo; } }
    public bool IsReloading { get { return isReloading; } }
    private bool isMobileFiring = false; // Cờ để kiểm tra nút bắn trên mobile có đang được giữ không
    private Camera mainCamera;

    #endregion

    #region Vòng đời Unity

    private void Start()
    {
        // Khởi tạo số đạn ban đầu
        currentAmmo = maxAmmo;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 3. Xử lý Logic Bắn đạn
        // Điều kiện để bắn: (giữ chuột trái hoặc nút mobile) VÀ không nạp đạn VÀ còn đạn
        bool canFire = (Input.GetButton("Fire1") || isMobileFiring) && !isReloading && currentAmmo > 0;
        
        if (canFire && Time.time >= nextTimeToFire)
        {
            // Cập nhật thời gian cho lần bắn tiếp theo
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }

        // 4. Tự động nạp đạn khi hết
        if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }

        // Cho phép người chơi PC chủ động nạp đạn
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    #endregion

    #region Logic Bắn & Nạp Đạn

    /// <summary>
    /// Thực hiện hành động bắn.
    /// </summary>
    private void Shoot()
    {
        currentAmmo--;

        // Kích hoạt hiệu ứng nổ ở nòng súng (nếu có)
        if (muzzleFlashFX != null)
        {
            muzzleFlashFX.Play();
        }

        // Bắn một tia từ tâm màn hình
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, range))
        {
            // Kiểm tra xem đối tượng trúng đạn có thể nhận sát thương không
            IDamageable damageableTarget = hit.collider.GetComponent<IDamageable>();
            if (damageableTarget != null)
            {
                // Nếu có, gây sát thương
                damageableTarget.TakeDamage(damage);
            }

            // Tạo hiệu ứng va chạm (nếu có)
            if (bulletImpactPrefab != null)
            {
                Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }

    /// <summary>
    /// Coroutine xử lý quá trình nạp đạn.
    /// </summary>
    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Đang nạp đạn...");

        // Chờ hết thời gian nạp đạn
        yield return new WaitForSeconds(reloadTime);

        // Nạp đầy băng đạn
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Nạp đạn thành công!");
    }

    #endregion

    #region Hàm Public cho UI

    /// <summary>
    /// Được gọi bởi EventTrigger (PointerDown) trên nút bắn của Mobile.
    /// </summary>
    public void StartFiring()
    {
        isMobileFiring = true;
    }

    /// <summary>
    /// Được gọi bởi EventTrigger (PointerUp) trên nút bắn của Mobile.
    /// </summary>
    public void StopFiring()
    {
        isMobileFiring = false;
    }

    /// <summary>
    /// Cho phép gọi nạp đạn từ một nút bấm trên UI.
    /// </summary>
    public void ReloadWeapon()
    {
        // Chỉ cho phép nạp đạn khi đạn chưa đầy và không đang trong quá trình nạp rồi
        if (currentAmmo < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    #endregion
}
