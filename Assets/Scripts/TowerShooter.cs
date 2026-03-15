using UnityEngine;

public class TowerShooter : MonoBehaviour
{
    [Header("Targeting")]
    public LayerMask enemyMask;
    public float range = 8f;
    public Transform turretHead; // optional: part that rotates, else use transform

    [Header("Damage")]
    public float damage = 5f;
    public float fireRate = 2f; // shots per second

    [Header("Visual")]
    public LineRenderer tracer; // optional
    public float tracerTime = 0.05f;

    private float fireCooldown;
    private Transform currentTarget;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;

    private void Update()
    {
        AcquireTarget();

        if (currentTarget == null) return;

        AimAtTarget();

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = 1f / fireRate;
        }
    }

    private void AcquireTarget()
    {
        // Keep current target if still valid
        if (currentTarget != null)
        {
            if (Vector3.Distance(transform.position, currentTarget.position) <= range)
                return;
            currentTarget = null;
        }

        // Find closest enemy in range
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyMask);
        float bestDist = float.MaxValue;
        Transform best = null;

        for (int i = 0; i < hits.Length; i++)
        {
            Transform t = hits[i].transform;
            float d = (t.position - transform.position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }

        currentTarget = best;
    }

    private void AimAtTarget()
    {
        Transform head = turretHead != null ? turretHead : transform;

        Vector3 dir = currentTarget.position - head.position;
        dir.y = 0f; // rotate only on Y axis

        if (dir.sqrMagnitude < 0.001f) return;

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        head.rotation = Quaternion.RotateTowards(head.rotation, rot, 720f * Time.deltaTime);
    }

    private void Fire()
    {
        if (projectilePrefab == null || currentTarget == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.up * 1f;
        GameObject projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        var proj = projObj.GetComponent<ProjectileHoming>();
        if (proj != null)
        {
            proj.speed = projectileSpeed;
            proj.Init(currentTarget, damage);
        }
    }

    private System.Collections.IEnumerator ShowTracer(Vector3 from, Vector3 to)
    {
        tracer.enabled = true;
        tracer.SetPosition(0, from);
        tracer.SetPosition(1, to);
        yield return new WaitForSeconds(tracerTime);
        tracer.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}