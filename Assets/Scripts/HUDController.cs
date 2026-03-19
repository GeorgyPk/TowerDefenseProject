using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Top info")]
    public TMP_Text moneyText;
    public TMP_Text livesText;
    public TMP_Text waveText;
    public Toggle autoWavesToggle;

    [Header("Build buttons")]
    public Button buildBalancedButton;
    public Button buildBurstButton;
    public Button buildFrostButton;

    public TMP_Text buildBalancedButtonText;
    public TMP_Text buildBurstButtonText;
    public TMP_Text buildFrostButtonText;

    [Header("Action buttons")]
    public Button upgradeButton;
    public Button sellButton;

    public TMP_Text upgradeButtonText;
    public TMP_Text sellButtonText;

    [Header("Definitions")]
    public TowerDefinition balancedDefinition;
    public TowerDefinition burstDefinition;
    public TowerDefinition frostDefinition;

    [Header("Selection")]
    public TowerSelectionManager selectionManager;

    private void Start()
    {
        if (autoWavesToggle != null)
        {
            autoWavesToggle.isOn = GameManager.Instance.AutoWaves;
            autoWavesToggle.onValueChanged.AddListener(v => GameManager.Instance.SetAutoWaves(v));
        }

        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        // Top info
        if (moneyText != null) moneyText.text = $"Money: {gm.Money}";
        if (livesText != null) livesText.text = $"Lives: {gm.Lives}";
        if (waveText != null) waveText.text = $"Wave: {gm.Wave}";

        // Build buttons: text + affordability
        RefreshBuildButton(buildBalancedButton, buildBalancedButtonText, balancedDefinition, "Build Turret");
        RefreshBuildButton(buildBurstButton, buildBurstButtonText, burstDefinition, "Build Burst");
        RefreshBuildButton(buildFrostButton, buildFrostButtonText, frostDefinition, "Build Frost");

        // Selected tower actions
        RefreshSelectionButtons();
    }

    private void RefreshBuildButton(Button button, TMP_Text label, TowerDefinition def, string baseName)
    {
        if (button == null || label == null || def == null || GameManager.Instance == null) return;

        label.text = $"{baseName} (${def.buildCost})";
        button.interactable = GameManager.Instance.Money >= def.buildCost;
    }

    private void RefreshSelectionButtons()
    {
        TowerInstance selectedTower = null;

        if (selectionManager != null)
            selectedTower = selectionManager.selected;

        // No tower selected -> both disabled
        if (selectedTower == null)
        {
            if (sellButton != null) sellButton.interactable = false;
            if (upgradeButton != null) upgradeButton.interactable = false;

            if (sellButtonText != null) sellButtonText.text = "Sell";
            if (upgradeButtonText != null) upgradeButtonText.text = "Upgrade";
            return;
        }

        // Sell button
        int refund = selectedTower.SellRefund();
        if (sellButton != null) sellButton.interactable = true;
        if (sellButtonText != null) sellButtonText.text = $"Sell (${refund})";

        // Upgrade button
        bool canUpgrade = selectedTower.CanUpgrade;
        int upgradeCost = selectedTower.NextUpgradeCost();

        bool canAffordUpgrade = GameManager.Instance != null && GameManager.Instance.Money >= upgradeCost;

        if (upgradeButton != null)
            upgradeButton.interactable = canUpgrade && canAffordUpgrade;

        if (upgradeButtonText != null)
        {
            if (!canUpgrade)
                upgradeButtonText.text = "Upgrade (Max)";
            else
                upgradeButtonText.text = $"Upgrade (${upgradeCost})";
        }
    }
}