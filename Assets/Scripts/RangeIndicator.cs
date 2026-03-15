using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeIndicator : MonoBehaviour
{
    public float radius = 8f;
    public int segments = 64;
    public float yOffset = 0.02f;

    private LineRenderer lr;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.loop = true;
        lr.useWorldSpace = true;
    }

    public void SetRadius(float r)
    {
        radius = Mathf.Max(0.1f, r);
        Draw();
    }

    public void Draw()
    {
        if (lr == null) lr = GetComponent<LineRenderer>();

        lr.positionCount = segments;
        float step = 2f * Mathf.PI / segments;

        Vector3 center = transform.position;
        center.y += yOffset;

        for (int i = 0; i < segments; i++)
        {
            float a = step * i;
            float x = Mathf.Cos(a) * radius;
            float z = Mathf.Sin(a) * radius;
            lr.SetPosition(i, center + new Vector3(x, 0f, z));
        }
    }
}