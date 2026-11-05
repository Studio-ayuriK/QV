using System;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 8f; // 감지 범위
    [SerializeField] private LayerMask playerLayer;     // 감지 할 플레이어 레이어
    [SerializeField] private float scanInterval = 0.2f; // 스캔 간격

    public Transform Target { get; private set; }

    private readonly Collider[] results = new Collider[32];
    private float scanTimer;

    private void Awake()
    {
        if (playerLayer.value == 0)
        {
            Debug.LogWarning(
                $"{name}: Player LayerMask가 설정되지 않았습니다.",
                this
            );
        }

    }
    // Update is called once per frame
    void Update()
    {
        scanTimer -= Time.deltaTime;

        if (scanTimer > 0f) return;

        scanTimer = scanInterval;
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        // 기존 타깃 유효성 확인
        if (Target != null)
        {
            if (!Target.gameObject.activeInHierarchy ||
                GetHorizontalDistance(transform.position, Target.position) > detectionRange)
            {
                Target = null;
            }
            else
            {
                return;
            }
        }

        int count = Physics.OverlapSphereNonAlloc(
            transform.position, detectionRange,
            results, playerLayer,
            QueryTriggerInteraction.Collide
        );

        Transform nearestTarget = null;
        float nearestDistance = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider hit = results[i];

            if (hit == null) continue;

            Transform candidate = hit.transform;

            // 플레이어 콜라이더가 자식 오브젝트에 있는 경우도 처리
            if (!candidate.CompareTag("Player"))
                candidate = candidate.root;

            if (!candidate.CompareTag("Player"))
                continue;

            float distance = GetHorizontalDistance(transform.position, candidate.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = candidate;
            }
        }
        Target = nearestTarget;
    }

    private float GetHorizontalDistance(Vector3 position1, Vector3 position2)
    {
        position1.y = 0f;
        position2.y = 0f;

        return Vector3.Distance(position1, position2);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
