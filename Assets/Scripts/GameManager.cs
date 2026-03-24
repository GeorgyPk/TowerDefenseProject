using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Won,
        Lost
    }

    [Header("Starting Values")]
    public int startingMoney = 75;
    public int startingLives = 20;

    [Header("Runtime")]
    [SerializeField] private int money;
    [SerializeField] private int lives;
    [SerializeField] private int wave;
    [SerializeField] private bool autoWaves;
    [SerializeField] private GameState state = GameState.MainMenu;

    public int Money => money;
    public int Lives => lives;
    public int Wave => wave;
    public bool AutoWaves => autoWaves;
    public GameState State => state;

    public bool IsPlaying => state == GameState.Playing;
    public bool IsPaused => state == GameState.Paused;
    public bool IsMainMenu => state == GameState.MainMenu;
    public bool IsWon => state == GameState.Won;
    public bool IsLost => state == GameState.Lost;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ResetRun();
        state = GameState.MainMenu;
        Time.timeScale = 0f;
    }

    public void ResetRun()
    {
        money = startingMoney;
        lives = startingLives;
        wave = 0;
        autoWaves = false;
    }

    public void StartGame()
    {
        ResetRun();
        state = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        if (state != GameState.Playing) return;

        state = GameState.Paused;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (state != GameState.Paused) return;

        state = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        if (state == GameState.Playing) PauseGame();
        else if (state == GameState.Paused) ResumeGame();
    }

    public void WinGame()
    {
        state = GameState.Won;
        Time.timeScale = 0f;
        Debug.Log("You Win!");
    }

    public void LoseGame()
    {
        state = GameState.Lost;
        Time.timeScale = 0f;
        Debug.Log("Game Over!");
    }

    public void RestartScene()
    {
        ResetRun();
        state = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        state = GameState.MainMenu;
        autoWaves = false;
        Time.timeScale = 0f;
    }

    public void AddMoney(int amount)
    {
        money += Mathf.Max(0, amount);
    }

    public bool TrySpendMoney(int amount)
    {
        if (amount <= 0) return true;
        if (money < amount) return false;

        money -= amount;
        return true;
    }

    public void LoseLives(int amount)
    {
        if (state != GameState.Playing) return;

        lives -= Mathf.Max(0, amount);
        if (lives <= 0)
        {
            lives = 0;
            LoseGame();
        }
    }

    public void SetWave(int newWave)
    {
        wave = Mathf.Max(0, newWave);
    }

    public void NextWave()
    {
        wave++;
    }

    public void SetAutoWaves(bool enabled)
    {
        autoWaves = enabled;
    }
}