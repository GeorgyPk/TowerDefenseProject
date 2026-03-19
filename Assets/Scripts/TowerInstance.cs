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

    /* public string UpgradeDescription()
    {
        if (definition == null || !CanUpgrade) return "Max Level";

        float nextDamage = damagePreview();
        float nextRange = rangePreview();
        float nextFireRate = fireRatePreview();
        float nextSlow = slowPreview();

        string text = $"Upgrade (${NextUpgradeCost()})\n";
        text += $"+DMG +RNG +SPD";

        if (definition.appliesSlow)
            text += $" +SLOW";

        return text;
    } */
}