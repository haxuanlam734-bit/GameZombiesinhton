using UnityEngine;
using System.Collections;

/// <summary>
/// Script điều khiển hành vi của một quả lựu đạn sau khi được ném.
/// Gắn script này vào Prefab của quả lựu đạn.
/// </summary>
public class Grenade : MonoBehaviour
{
    [Header("Thông số Lựu đạn")]
    [Tooltip("Thời gian chờ trước khi nổ (giây)")]
    [SerializeField] private float countdown = 3f;
    [Tooltip("Bán kính vụ nổ")]
    [SerializeField] private float explosionRadius = 5f;
    [Tooltip("Sát thương gây ra tại tâm vụ nổ")]
    [SerializeField] private float explosionDamage = 100f;

    [Header("Hiệu ứng")]
    [Tooltip("Prefab hiệu ứng nổ. Sẽ được sinh ra khi lựu đạn nổ.")]
    [SerializeField] private GameObject explosionEffect;

    void Start()
    {
        // Bắt đầu đếm ngược ngay khi được tạo ra
        StartCoroutine(ExplodeAfterTime());
    }

    private IEnumerator ExplodeAfterTime()
    {
        // Chờ hết thời gian đếm ngược
        yield return new WaitForSeconds(countdown);
        Explode();
    }

    private void Explode()
    {
        // 1. Tạo hiệu ứng nổ
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // 2. Tìm tất cả các đối tượng trong bán kính vụ nổ
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        // 3. Gây sát thương cho các đối tượng có IDamageable
        foreach (Collider nearbyObject in colliders)
        {
            IDamageable damageable = nearbyObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                // (Tùy chọn nâng cao): Giảm sát thương dựa trên khoảng cách từ tâm nổ
                // float distance = Vector3.Distance(transform.position, nearbyObject.transform.position);
                // float damageFalloff = 1 - (distance / explosionRadius);
                // float damageToDeal = explosionDamage * damageFalloff;
                // damageable.TakeDamage(damageToDeal);

                // Gây sát thương tối đa
                damageable.TakeDamage(explosionDamage);
            }
        }

        // 4. Hủy đối tượng lựu đạn
        Destroy(gameObject);
    }

    // (Optional) Vẽ bán kính vụ nổ trong Editor để dễ hình dung
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
