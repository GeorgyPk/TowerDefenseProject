using UnityEngine;

public class ProjectileHoming : MonoBehaviour
{
    public float speed = 20f;
    public float turnSpeed = 720f; // degrees/sec (optional)
    public float lifeTime = 3f;

    private Transform target;
    private float damage;
    private bool appliesSlow;
    private float slowPercent;
    private float slowDuration;


    public void Init(Transform targetTransform, float damageAmount)
    {
        Init(targetTransform, damageAmount, false, 0f, 0f);
    }
    
    public void Init(Transform targetTransform, float damageAmount, bool slow, float slowPct, float slowDur)
    {
        target = targetTransform;
        damage = damageAmount;

        appliesSlow = slow;
        slowPercent = slowPct;
        slowDuration = slowDur;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Aim & move toward target
        Vector3 toTarget = target.position - transform.position;
        float distThisFrame = speed * Time.deltaTime;

        // If close enough, "hit"
        if (toTarget.magnitude <= distThisFrame)
        {
            HitTarget();
            return;
        }

        // Move
        Vector3 dir = toTarget.normalized;
        transform.position += dir * distThisFrame;

        // Optional: rotate projectile to face direction
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
        }
    }

    private void HitTarget()
    {
        if (target != null)
        {
            var hp = target.GetComponent<EnemyHealth>();
            if (hp != null) hp.TakeDamage(damage);

            if (appliesSlow)
            {
                var status = target.GetComponent<EnemyStatus>();
                if (status != null) status.ApplySlow(slowPercent, slowDuration);
            }
        }
        Destroy(gameObject);
    }

    // If you prefer trigger hits instead of distance checks:
    private void OnTriggerEnter(Collider other)
    {
        if (target == null) return;
        if (other.transform != target) return;

        HitTarget();
    }
}