using UnityEngine;

public class TowerInstance : MonoBehaviour
{
    public TowerDefinition definition;
    [Range(1, 3)] public int level = 1;

    private TowerWeapon weapon;
    private RangeIndicator rangeIndicator;

    private int totalSpent;

    private void Awake()
    {
        weapon = GetComponent<TowerWeapon>();
        rangeIndicator = GetComponentInChildren<RangeIndicator>(true);
    }

    private void Start()
    {
        ApplyStats();
        totalSpent = definition != null ? definition.buildCost : 0;

        // Make sure placed towers do NOT show their range by default
        var lr = GetComponentInChildren<LineRenderer>(true);
        if (lr != null && gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
            lr.enabled = false;
    }

    public void ApplyStats()
    {
        if (definition == null || weapon == null) return;

        float dmg = definition.damage;
        float rng = definition.range;
        float fr = definition.fireRate;
        float ps = definition.projectileSpeed;

        float slowPct = definition.slowPercent;

        if (level >= 2)
        {
            dmg *= definition.l2_damageMult;
            rng *= definition.l2_rangeMult;
            fr *= definition.l2_fireRateMult;
            ps *= definition.l2_projectileSpeedMult;
            slowPct += definition.l2_slowPercentAdd;
        }
        if (level >= 3)
        {
            dmg *= definition.l3_damageMult;
            rng *= definition.l3_rangeMult;
            fr *= definition.l3_fireRateMult;
            ps *= definition.l3_projectileSpeedMult;
            slowPct += definition.l3_slowPercentAdd;
        }

        weapon.range = rng;
        weapon.damage = dmg;
        weapon.fireRate = fr;
        weapon.projectileSpeed = ps;
        weapon.maxTargetsPerShot = definition.maxTargetsPerShot;
        weapon.projectilesPerTarget = definition.projectilesPerTarget;
        weapon.appliesSlow = definition.appliesSlow;
        weapon.slowPercent = Mathf.Clamp01(slowPct);
        weapon.slowDuration = definition.slowDuration;

        if (rangeIndicator != null)
        {
            rangeIndicator.SetRadius(rng);
        }
    }

    public bool CanUpgrade => definition != null && level < 3;

    public int NextUpgradeCost()
    {
        if (definition == null) return 0;
        if (level == 1) return definition.upgradeCostL2;
        if (level == 2) return definition.upgradeCostL3;
        return 0;
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade) return false;

        int cost = NextUpgradeCost();
        if (!GameManager.Instance.TrySpendMoney(cost)) return false;

        level++;
        totalSpent += cost;
        ApplyStats();
        return true;
    }

    public int SellRefund()
    {
        if (definition == null) return 0;
        return Mathf.RoundToInt(totalSpent * definition.sellRefundRate);
    }

    private struct PreviewStats
    {
        public float damage;
        public float range;
        public float fireRate;
        public float projectileSpeed;
        public float slowPercent;
    }

    private PreviewStats GetStatsForLevel(int previewLevel)
    {
        PreviewStats stats = new PreviewStats();

        if (definition == null)
            return stats;

        float dmg = definition.damage;
        float rng = definition.range;
        float fr = definition.fireRate;
        float ps = definition.projectileSpeed;
        float slowPct = definition.slowPercent;

        if (previewLevel >= 2)
        {
            dmg *= definition.l2_damageMult;
            rng *= definition.l2_rangeMult;
            fr *= definition.l2_fireRateMult;
            ps *= definition.l2_projectileSpeedMult;
            slowPct += definition.l2_slowPercentAdd;
        }

        if (previewLevel >= 3)
        {
            dmg *= definition.l3_damageMult;
            rng *= definition.l3_rangeMult;
            fr *= definition.l3_fireRateMult;
            ps *= definition.l3_projectileSpeedMult;
            slowPct += definition.l3_slowPercentAdd;
        }

        stats.damage = dmg;
        stats.range = rng;
        stats.fireRate = fr;
        stats.projectileSpeed = ps;
        stats.slowPercent = Mathf.Clamp01(slowPct);

        return stats;
    }

    private string FormatFloat(float value)
    {
        return value.ToString("0.##");
    }

    public string GetUpgradeSummaryText()
    {
        if (definition == null) return "No tower data.";
        if (!CanUpgrade) return "Tower is at max level.";

        int nextLevel = level + 1;

        PreviewStats current = GetStatsForLevel(level);
        PreviewStats next = GetStatsForLevel(nextLevel);

        string text = $"Next upgrade to L{nextLevel}:\n";
        text += $"Damage: {FormatFloat(current.damage)} -> {FormatFloat(next.damage)}\n";
        text += $"Range: {FormatFloat(current.range)} -> {FormatFloat(next.range)}\n";
        text += $"Fire Rate: {FormatFloat(current.fireRate)} -> {FormatFloat(next.fireRate)}";

        if (definition.appliesSlow)
        {
            text += $"\nSlow: {FormatFloat(current.slowPercent * 100f)}% -> {FormatFloat(next.slowPercent * 100f)}%";
        }

        return text;
    }
}