using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHp = 20f;
    public float currentHp;
    public int reward = 5;

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
        if (GameManager.Instance != null)
            GameManager.Instance.AddMoney(reward);

        Destroy(gameObject);
    }
}