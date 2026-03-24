using UnityEngine;

public class TowerWeapon : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;
    public GameObject projectilePrefab;

    [Header("Targeting")]
    public LayerMask enemyMask;
    public float range = 8f;

    [Header("Stats")]
    public float damage = 5f;
    public float fireRate = 2f;
    public float projectileSpeed = 20f;

    [Header("Burst")]
    public int maxTargetsPerShot = 1;
    public int projectilesPerTarget = 1;

    [Header("Slow")]
    public bool appliesSlow = false;
    [Range(0f, 0.95f)] public float slowPercent = 0.35f;
    public float slowDuration = 1.5f;

    private float cd;

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            return;

        cd -= Time.deltaTime;
        if (cd > 0f) return;

        var targets = AcquireTargets();
        if (targets.Length == 0) return;

        FireAtTargets(targets);
        cd = 1f / Mathf.Max(0.01f, fireRate);
    }

    private Transform[] AcquireTargets()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyMask);
        if (hits.Length == 0) return System.Array.Empty<Transform>();

        // Pick up to N closest enemies
        System.Array.Sort(hits, (a, b) =>
            (a.transform.position - transform.position).sqrMagnitude.CompareTo(
            (b.transform.position - transform.position).sqrMagnitude));

        int n = Mathf.Clamp(maxTargetsPerShot, 1, hits.Length);
        Transform[] targets = new Transform[n];
        for (int i = 0; i < n; i++) targets[i] = hits[i].transform;
        return targets;
    }

    private void FireAtTargets(Transform[] targets)
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = firePoint ? firePoint.position : (transform.position + Vector3.up * 1f);

        foreach (var t in targets)
        {
            for (int i = 0; i < Mathf.Max(1, projectilesPerTarget); i++)
            {
                var projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
                var proj = projObj.GetComponent<ProjectileHoming>();
                if (proj != null)
                {
                    proj.speed = projectileSpeed;
                    proj.Init(t, damage, appliesSlow, slowPercent, slowDuration);
                }
            }
        }
    }
}