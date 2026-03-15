using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public WaypointPath path;

    [Header("Wave Settings")]
    public float timeBetweenSpawns = 0.6f;

    [Header("Difficulty")]
    public int baseEnemyCount = 8;
    public int enemyCountPerWave = 3;
    public float hpMultiplierPerWave = 1.15f;
    public float speedMultiplierPerWave = 1.05f;

    private bool waveRunning;
    private int aliveEnemies;

    public bool IsWaveRunning => waveRunning;

    private void Update()
    {
        // Auto-start next wave if enabled and no wave is running
        if (!waveRunning && GameManager.Instance != null && GameManager.Instance.AutoWaves)
        {
            StartNextWave();
        }
    }

    public void StartNextWave()
    {
        if (waveRunning) return;
        StartCoroutine(RunWave());
    }

    private IEnumerator RunWave()
    {
        waveRunning = true;

        // Increment wave counter
        GameManager.Instance.NextWave();
        int wave = GameManager.Instance.Wave;

        int count = baseEnemyCount + (wave - 1) * enemyCountPerWave;

        for (int i = 0; i < count; i++)
        {
            SpawnEnemy(wave);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        // Wait until all spawned enemies are dead or reached end
        while (aliveEnemies > 0)
            yield return null;

        waveRunning = false;
    }

    private void SpawnEnemy(int wave)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // Hook up path
        var mover = enemy.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.path = path;
            mover.startWaypointIndex = 0;
            mover.speed *= Mathf.Pow(speedMultiplierPerWave, wave - 1);
        }

        // Scale HP
        var hp = enemy.GetComponent<EnemyHealth>();
        if (hp != null)
        {
            hp.maxHp *= Mathf.Pow(hpMultiplierPerWave, wave - 1);
            hp.currentHp = hp.maxHp;
        }

        // Track alive enemies
        aliveEnemies++;

        // Register callbacks
        var tracker = enemy.AddComponent<WaveEnemyTracker>();
        tracker.manager = this;
    }

    public void NotifyEnemyGone()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
    }
}