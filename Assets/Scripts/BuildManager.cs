using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject towerPrefab;

    [Header("Layers")]
    public LayerMask buildZoneMask;    // BuildZone only
    public LayerMask blockedMask;      // Path + Tower (+ NoBuild if you add later)

    [Header("Placement")]
    public bool useGridSnap = false;
    public float gridSize = 1f;
    public float placeHeight = 0f;

    [Tooltip("How much space the tower needs around its center.")]
    public float placementRadius = 0.8f;

    private Camera cam;
    private GameObject ghost;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (towerPrefab == null || cam == null) return;
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = cam.ScreenPointToRay(mousePos);
        if (!Physics.Raycast(ray, out RaycastHit hit, 500f, buildZoneMask))
        {
            if (ghost != null) ghost.SetActive(false);
            return;
        }

        Vector3 pos = hit.point;
        pos.y = placeHeight;

        if (useGridSnap)
        {
            pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
            pos.z = Mathf.Round(pos.z / gridSize) * gridSize;
        }

        EnsureGhost();
        ghost.SetActive(true);
        ghost.transform.position = pos;

        bool valid = !Physics.CheckSphere(pos, placementRadius, blockedMask);

        TintGhost(valid);

        if (valid && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Instantiate(towerPrefab, pos, Quaternion.identity);
        }
    }

    private void EnsureGhost()
    {
        if (ghost != null) return;
        ghost = Instantiate(towerPrefab);

        // Make ghost not interfere with collision checks (optional but nice):
        ghost.layer = LayerMask.NameToLayer("Ignore Raycast");
        foreach (Transform t in ghost.GetComponentsInChildren<Transform>())
            t.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void TintGhost(bool valid)
    {
        if (ghost == null) return;
        var renderers = ghost.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            if (!r.material.HasProperty("_Color")) continue;
            var c = valid ? Color.green : Color.red;
            c.a = 0.5f;
            r.material.color = c;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (ghost == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ghost.transform.position, placementRadius);
    }
}