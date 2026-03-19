using UnityEngine;

[CreateAssetMenu(menuName = "TD/Tower Definition")]
public class TowerDefinition : ScriptableObject
{
    public string displayName = "Tower";
    public GameObject towerPrefab;

    [Header("Economy")]
    public int buildCost = 25;
    public int upgradeCostL2 = 40;
    public int upgradeCostL3 = 70;
    [Range(0f, 1f)] public float sellRefundRate = 0.5f;

    [Header("Level 1 Stats")]
    public float range = 8f;
    public float damage = 5f;
    public float fireRate = 2f;          // shots / second
    public float projectileSpeed = 20f;

    [Header("Burst (multi-target)")]
    public int maxTargetsPerShot = 1;     // Burst tower: e.g. 4
    public int projectilesPerTarget = 1;  // optional (keep 1)

    [Header("Slow (optional)")]
    public bool appliesSlow = false;
    [Range(0f, 0.95f)] public float slowPercent = 0.35f;  // 0.35 = 35% slow
    public float slowDuration = 1.5f;

    [Header("Upgrade multipliers")]
    public float l2_damageMult = 1.35f;
    public float l2_rangeMult  = 1.10f;
    public float l2_fireRateMult = 1.15f;
    public float l2_projectileSpeedMult = 1.10f;
    public float l2_slowPercentAdd = 0.10f; // frost only

    public float l3_damageMult = 1.75f;
    public float l3_rangeMult  = 1.20f;
    public float l3_fireRateMult = 1.30f;
    public float l3_projectileSpeedMult = 1.20f;
    public float l3_slowPercentAdd = 0.20f; // frost only
}