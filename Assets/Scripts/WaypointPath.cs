using System;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [Tooltip("Waypoints in order. If empty, children named WP_* under this object will be used.")]
    public Transform[] waypoints;

    private void Reset()
    {
        AutoCollectFromChildren();
    }

    private void OnValidate()
    {
        if (waypoints == null || waypoints.Length == 0)
            AutoCollectFromChildren();
    }

    private void AutoCollectFromChildren()
    {
        var list = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("WP_", StringComparison.OrdinalIgnoreCase))
                list.Add(child);
        }
        waypoints = list.ToArray();
    }

    public Transform Get(int index) => waypoints[index];
    public int Count => waypoints != null ? waypoints.Length : 0;

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawSphere(waypoints[i].position, 0.25f);

            if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}