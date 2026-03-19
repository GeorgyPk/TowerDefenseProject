using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    private float slowFactor = 1f;  // 1 = normal speed
    private float slowTimer = 0f;

    public float SlowFactor => slowFactor;

    private void Update()
    {
        if (slowTimer > 0f)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0f)
            {
                slowFactor = 1f;
                slowTimer = 0f;
            }
        }
    }

    public void ApplySlow(float percent, float duration)
    {
        float factor = Mathf.Clamp01(1f - percent);
        // Take the stronger slow (smaller factor), refresh duration
        slowFactor = Mathf.Min(slowFactor, factor);
        slowTimer = Mathf.Max(slowTimer, duration);
    }
}