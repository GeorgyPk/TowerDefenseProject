using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHp = 20f;
    public float currentHp;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float amount)
    {
        currentHp -= amount;
        if (currentHp <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}