using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMover : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 4f; // base speed (can be modified by wave/enemy type)
    public float rotateSpeed = 720f;
    public float waypointReachDistance = 0.2f;

    [Header("Path")]
    public WaypointPath path;
    public int startWaypointIndex = 0;

    private Rigidbody rb;
    private int currentIndex;

    // This is the base speed AFTER wave/type modifiers, before slow debuffs
    private float baseSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnEnable()
    {
        currentIndex = startWaypointIndex;

        baseSpeed = speed;

        if (path != null && path.Count > 0)
            transform.position = path.Get(currentIndex).position;
    }

    private void Update()
    {
        if (path == null || path.Count == 0) return;
        if (currentIndex >= path.Count) return;

        Vector3 targetPos = path.Get(currentIndex).position;
        Vector3 toTarget = targetPos - transform.position;
        toTarget.y = 0f;

        float dist = toTarget.magnitude;
        if (dist <= waypointReachDistance)
        {
            currentIndex++;
            if (currentIndex >= path.Count)
            {
                enabled = false; // EndZone will destroy enemy
            }
            return;
        }

        Vector3 dir = toTarget.normalized;

        // Slow logic
        float slowFactor = 1f;
        var status = GetComponent<EnemyStatus>();
        if (status != null) slowFactor = status.SlowFactor;

        float currentSpeed = baseSpeed * slowFactor;

        transform.position += dir * (currentSpeed * Time.deltaTime);

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
    }

    public void SetBaseSpeed(float newBaseSpeed)
    {
        baseSpeed = newBaseSpeed;
        speed = newBaseSpeed;
    }
}