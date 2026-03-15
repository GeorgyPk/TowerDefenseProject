using UnityEngine;

public class WaveEnemyTracker : MonoBehaviour
{
    public WaveManager manager;

    private void OnDestroy()
    {
        if (manager != null)
            manager.NotifyEnemyGone();
    }
}