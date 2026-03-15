using UnityEngine;

public class EndZone : MonoBehaviour
{
    public int damagePerEnemy = 1;

    private void OnTriggerEnter(Collider other)
    {
        var mover = other.GetComponent<EnemyMover>();
        if (mover == null) return;

        // TODO later: GameManager.Instance.LoseLives(damagePerEnemy);
        Debug.Log($"Enemy reached the base! -{damagePerEnemy} lives");

        Destroy(other.gameObject);
    }
}