using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public WaypointPath path;
    public float spawnInterval = 2f;
    public int spawnCount = 5;

    private float timer;
    private int spawned;

    private void Update()
    {
        if (enemyPrefab == null || path == null) return;
        if (spawned >= spawnCount) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            Spawn();
        }
    }

    private void Spawn()
    {
        GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        var mover = enemy.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.path = path;
            mover.startWaypointIndex = 0;
        }
        spawned++;
    }
}