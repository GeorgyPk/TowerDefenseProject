using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    [Header("Towers (ScriptableObjects)")]
    public TowerDefinition[] towers;
    public int selectedIndex = 0;

    [Header("Layers")]
    public LayerMask buildZoneMask; // BuildZone only
    public LayerMask blockedMask; // NoBuild + Tower

    [Header("Placement")]
    public bool useGridSnap = false;
    public float gridSize = 1f;
    public float placeHeight = 0f;
    public float placementRadius = 0.8f;

    private Camera cam;
    private GameObject ghost;
    private int ghostIndex = -1;
    public bool buildMode = true;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!buildMode)
        {
            if (ghost != null) ghost.SetActive(false);
            return;
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            DisableBuildMode();
            return;
        }
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            DisableBuildMode();
            return;
        }

        if (cam == null || Mouse.current == null) return;
        if (towers == null || towers.Length == 0) return;

        // Clamp selected index
        selectedIndex = Mathf.Clamp(selectedIndex, 0, towers.Length - 1);

        TowerDefinition def = towers[selectedIndex];
        if (def == null || def.towerPrefab == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);

        // Only allow placement on BuildZone surfaces
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

        EnsureGhost(def);
        ghost.SetActive(true);
        ghost.transform.position = pos;

        // Update range circle position
        var indicator = ghost.GetComponentInChildren<RangeIndicator>();
        if (indicator != null) indicator.Draw();

        // Validate placement: money + no overlap with NoBuild/Tower
        bool canAfford = GameManager.Instance == null || GameManager.Instance.Money >= def.buildCost;
        bool clear = !Physics.CheckSphere(pos, placementRadius, blockedMask);
        bool valid = canAfford && clear;

        TintGhost(valid);

        if (valid && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (GameManager.Instance == null || GameManager.Instance.TrySpendMoney(def.buildCost))
            {
                GameObject placed = Instantiate(def.towerPrefab, pos, Quaternion.identity);

                // Assign definition to the placed tower instance
                var inst = placed.GetComponent<TowerInstance>();
                if (inst != null)
                {
                    inst.definition = def;
                    inst.level = 1;
                    inst.ApplyStats();
                }
            }
        }
    }

    public void SelectNextTower()
    {
        if (towers == null || towers.Length == 0) return;
        selectedIndex = (selectedIndex + 1) % towers.Length;

        // Force ghost rebuild so the preview updates immediately
        DestroyGhost();
    }

    private void EnsureGhost(TowerDefinition def)
    {
        // Rebuild ghost if tower type changed
        if (ghost != null && ghostIndex == selectedIndex) return;

        DestroyGhost();

        ghost = Instantiate(def.towerPrefab);
        ghostIndex = selectedIndex;

        // Disable gameplay scripts on the ghost so it doesn't shoot / act
        foreach (var weapon in ghost.GetComponentsInChildren<TowerWeapon>(true))
            weapon.enabled = false;

        // Disable colliders so ghost doesn't interfere
        foreach (var col in ghost.GetComponentsInChildren<Collider>(true))
            col.enabled = false;

        // Keep ghost out of raycast layers
        int ignore = LayerMask.NameToLayer("Ignore Raycast");
        ghost.layer = ignore;
        foreach (Transform t in ghost.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = ignore;

        // Enable & set range indicator based on tower definition stats (level 1)
        var lr = ghost.GetComponentInChildren<LineRenderer>(true);
        if (lr != null) lr.enabled = true;

        var indicator = ghost.GetComponentInChildren<RangeIndicator>(true);
        if (indicator != null)
        {
            indicator.yOffset = 0.1f;
            indicator.SetRadius(def.range);
        }
    }

    private void DestroyGhost()
    {
        if (ghost != null) Destroy(ghost);
        ghost = null;
        ghostIndex = -1;
    }

    private void TintGhost(bool valid)
    {
        if (ghost == null) return;

        var renderers = ghost.GetComponentsInChildren<Renderer>(true);
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

    public void ToggleBuildMode()
    {
        buildMode = !buildMode;
        if (!buildMode) DestroyGhost();
    }

    public void EnableBuildMode()
    {
        buildMode = true;
    }

    public void DisableBuildMode()
    {
        buildMode = false;
        DestroyGhost();
    }

    public void SelectTower0() { selectedIndex = 0; buildMode = true; DestroyGhost(); }
    public void SelectTower1() { selectedIndex = 1; buildMode = true; DestroyGhost(); }
    public void SelectTower2() { selectedIndex = 2; buildMode = true; DestroyGhost(); }
}