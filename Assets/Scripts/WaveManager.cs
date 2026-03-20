using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class EnemyEntry
    {
        public GameObject prefab;
        public int count = 1;
    }

    [System.Serializable]
    public class WaveDefinition
    {
        public List<EnemyEntry> enemies = new List<EnemyEntry>();
    }

    [Header("References")]
    public Transform spawnPoint;
    public WaypointPath path;

    [Header("Wave Settings")]
    public float timeBetweenSpawns = 0.6f;
    public List<WaveDefinition> waves = new List<WaveDefinition>();

    private bool waveRunning;
    private int aliveEnemies;

    public bool IsWaveRunning => waveRunning;

    private void Update()
    {
        if (!waveRunning && GameManager.Instance != null && GameManager.Instance.AutoWaves)
        {
            if (GameManager.Instance.Wave < waves.Count)
                StartNextWave();
        }
    }

    public void StartNextWave()
    {
        if (waveRunning) return;
        if (waves == null || waves.Count == 0) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.Wave >= waves.Count) return;

        StartCoroutine(RunWave());
    }

    private IEnumerator RunWave()
    {
        waveRunning = true;

        GameManager.Instance.NextWave();
        int waveIndex = GameManager.Instance.Wave - 1;

        WaveDefinition wave = waves[waveIndex];

        foreach (var entry in wave.enemies)
        {
            if (entry.prefab == null || entry.count <= 0) continue;

            for (int i = 0; i < entry.count; i++)
            {
                SpawnEnemy(entry.prefab);
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }

        while (aliveEnemies > 0)
            yield return null;

        waveRunning = false;
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        var mover = enemy.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.path = path;
            mover.startWaypointIndex = 0;
        }

        aliveEnemies++;

        var tracker = enemy.AddComponent<WaveEnemyTracker>();
        tracker.manager = this;
    }

    public void NotifyEnemyGone()
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
    }
}