using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMover : MonoBehaviour
{
    [Header("Movement")]

    public float speed = 4f;
    public float rotateSpeed = 720f; // degrees per second
    public float waypointReachDistance = 0.2f;

    [Header("Path")]
    public WaypointPath path;
    public int startWaypointIndex = 0;

    private Rigidbody rb;
    private int currentIndex;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // For a simple waypoint mover, keep physics stable:
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnEnable()
    {
        currentIndex = startWaypointIndex;
        if (path != null && path.Count > 0)
        {
            transform.position = path.Get(currentIndex).position;
        }
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
                // Reached end - EndZone will destroy the enemy
                enabled = false;
            }
            return;
        }

        Vector3 dir = toTarget.normalized;

        // Move
        transform.position += dir * (speed * Time.deltaTime);

        // Rotate toward movement direction
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRot,
                rotateSpeed * Time.deltaTime
            );
        }
    }
}