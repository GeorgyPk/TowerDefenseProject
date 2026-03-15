using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public TMP_Text moneyText;
    public TMP_Text livesText;
    public TMP_Text waveText;

    public Toggle autoWavesToggle;

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

        if (moneyText != null) moneyText.text = $"$ {gm.Money}";
        if (livesText != null) livesText.text = $"♥ {gm.Lives}";
        if (waveText != null) waveText.text = $"Wave {gm.Wave}";
    }
}