using UnityEngine;
using UnityEngine.InputSystem;

public class UIStateController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject pausePanel;
    public GameObject winPanel;
    public GameObject losePanel;

    private GameManager.GameState lastState;

    private void Start()
    {
        lastState = GameManager.Instance != null ? GameManager.Instance.State : GameManager.GameState.MainMenu;
        RefreshPanels();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameManager.Instance.IsPlaying || GameManager.Instance.IsPaused)
                GameManager.Instance.TogglePause();
        }

        if (GameManager.Instance.State != lastState)
        {
            lastState = GameManager.Instance.State;
            RefreshPanels();
        }
    }

    public void RefreshPanels()
    {
        if (GameManager.Instance == null) return;

        SetActive(mainMenuPanel, GameManager.Instance.IsMainMenu);
        SetActive(pausePanel, GameManager.Instance.IsPaused);
        SetActive(winPanel, GameManager.Instance.IsWon);
        SetActive(losePanel, GameManager.Instance.IsLost);
    }

    private void SetActive(GameObject go, bool value)
    {
        if (go != null) go.SetActive(value);
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
        RefreshPanels();
    }

    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
        RefreshPanels();
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartScene();
    }

    public void OpenMainMenu()
    {
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
}