using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody rb;

    private SkeletalEntity owner;

    private int damage;

    private bool isInitialized;
    private bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
    }

    // EnemyCombat에서 호출
    public void Initialize(
        SkeletalEntity owner,
        int damage,
        Vector3 direction)
    {
        if (owner == null || direction.sqrMagnitude <= 0.001f)
        {
            Destroy(gameObject);
            return;
        }

        this.owner = owner;
        this.damage = damage;

        // 발사체와 자신의 충돌 방지
        IgnoreOwnerCollision();

        transform.rotation = Quaternion.LookRotation(direction.normalized);

        rb.linearVelocity = direction.normalized * moveSpeed;

        isInitialized = true;

        Destroy(gameObject, lifeTime);
    }

    // 자신의 충돌체와 Enemy의 충돌체 무시
    private void IgnoreOwnerCollision()
    {
        Collider[] projectileColliders =
            GetComponentsInChildren<Collider>();

        Collider[] ownerColliders =
            owner.GetComponentsInChildren<Collider>();

        foreach (Collider projectileCollider in projectileColliders)
        {
            foreach (Collider ownerCollider in ownerColliders)
            {
                if (projectileCollider != null && ownerCollider != null)
                {
                    Physics.IgnoreCollision(
                        projectileCollider,
                        ownerCollider
                    );
                }
            }
        }
    }

    // 투사체 충돌
    private void OnTriggerEnter(Collider other)
    {
        if (!isInitialized || hasHit)
            return;

        SkeletalEntity target =
            other.GetComponentInParent<SkeletalEntity>();

        // 발사자 자신은 공격하지 않음
        if (target == owner)
            return;

        hasHit = true;

        if (target != null)
        {
            target.TakeDamage(damage);

            Debug.Log(
                $"{target.name}에게 투사체 피해 {damage}"
            );
        }

        // 대상 또는 환경에 충돌하면 제거
        Destroy(gameObject);
    }
}