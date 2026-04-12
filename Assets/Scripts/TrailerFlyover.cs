using UnityEngine;

public class TrailerFlyover : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float duration = 8f;

    private float timer = 0f;

    private void Start()
    {
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
        }
    }

    private void Update()
    {
        if (startPoint == null || endPoint == null || duration <= 0f)
            return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        transform.position = Vector3.Lerp(startPoint.position, endPoint.position, t);
        transform.rotation = Quaternion.Slerp(startPoint.rotation, endPoint.rotation, t);
    }
}