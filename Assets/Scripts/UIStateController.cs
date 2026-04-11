using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class UIStateController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject pausePanel;
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Score Texts")]
    public TMP_Text bestKillsMainMenuText;
    public TMP_Text bestKillsWinText;
    public TMP_Text bestKillsLoseText;

    public TMP_Text runKillsWinText;
    public TMP_Text runKillsLoseText;

    private GameManager.GameState lastState;

    private void Start()
    {
        lastState = GameManager.Instance != null
            ? GameManager.Instance.State
            : GameManager.GameState.MainMenu;

        RefreshPanels();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameManager.Instance.IsPlaying || GameManager.Instance.IsPaused)
            {
                GameManager.Instance.TogglePause();
            }
        }

        if (GameManager.Instance.State != lastState)
        {
            lastState = GameManager.Instance.State;
            RefreshPanels();
        }

        // Keeps score texts updated even if best score changes during play
        RefreshScoreTexts();
    }

    public void RefreshPanels()
    {
        if (GameManager.Instance == null) return;

        SetActive(mainMenuPanel, GameManager.Instance.IsMainMenu);
        SetActive(pausePanel, GameManager.Instance.IsPaused);
        SetActive(winPanel, GameManager.Instance.IsWon);
        SetActive(losePanel, GameManager.Instance.IsLost);

        RefreshScoreTexts();
    }

    private void SetActive(GameObject go, bool value)
    {
        if (go != null) go.SetActive(value);
    }

    public void StartGame()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.StartGame();
        RefreshPanels();
    }

    public void ResumeGame()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.ResumeGame();
        RefreshPanels();
    }

    public void RestartGame()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.RestartScene();
        RefreshPanels();
    }

    public void OpenMainMenu()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.ReturnToMainMenu();
        RefreshPanels();
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void RefreshScoreTexts()
    {
        if (GameManager.Instance == null) return;

        int best = GameManager.Instance.BestEnemiesKilled;
        int current = GameManager.Instance.EnemiesKilled;

        if (bestKillsMainMenuText != null)
            bestKillsMainMenuText.text = $"Best Kills: {best}";

        if (bestKillsWinText != null)
            bestKillsWinText.text = $"Best Kills: {best}";

        if (bestKillsLoseText != null)
            bestKillsLoseText.text = $"Best Kills: {best}";

        if (runKillsWinText != null)
            runKillsWinText.text = $"Kills this run: {current}";

        if (runKillsLoseText != null)
            runKillsLoseText.text = $"Kills this run: {current}";
    }
}