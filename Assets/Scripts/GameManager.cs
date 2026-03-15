using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Starting Values")]
    public int startingMoney = 150;
    public int startingLives = 20;

    [Header("Runtime")]
    [SerializeField] private int money;
    [SerializeField] private int lives;
    [SerializeField] private int wave;
    [SerializeField] private bool autoWaves;

    public int Money => money;
    public int Lives => lives;
    public int Wave => wave;
    public bool AutoWaves => autoWaves;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        money = startingMoney;
        lives = startingLives;
        wave = 0;
        autoWaves = false;
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
        lives -= Mathf.Max(0, amount);
        if (lives <= 0)
        {
            lives = 0;
            Debug.Log("Game Over!");
            // Later: trigger game over UI / stop waves
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