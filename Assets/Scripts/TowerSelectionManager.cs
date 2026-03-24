using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TowerSelectionManager : MonoBehaviour
{
    public LayerMask towerMask;
    public TowerInstance selected;

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            return;
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // Ignore clicks on UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, towerMask))
        {
            TowerInstance clickedTower = hit.collider.GetComponentInParent<TowerInstance>();

            if (clickedTower != null)
            {
                Select(clickedTower);
                return;
            }
        }

        // Clicked somewhere else, not a tower
        ClearSelection();
    }

    public void Select(TowerInstance tower)
    {
        if (tower == null)
        {
            ClearSelection();
            return;
        }

        if (selected == tower)
        {
            // Same tower clicked again -> keep it selected
            ShowSelectedRange(selected, true);
            return;
        }

        ShowSelectedRange(selected, false);
        selected = tower;
        ShowSelectedRange(selected, true);
    }

    public void ClearSelection()
    {
        ShowSelectedRange(selected, false);
        selected = null;
    }

    private void ShowSelectedRange(TowerInstance tower, bool show)
    {
        if (tower == null) return;

        var lr = tower.GetComponentInChildren<LineRenderer>(true);
        if (lr != null)
            lr.enabled = show;
    }

    public void UpgradeSelected()
    {
        if (selected == null) return;
        selected.TryUpgrade();
    }

    public void SellSelected()
    {
        if (selected == null) return;

        int refund = selected.SellRefund();
        GameManager.Instance.AddMoney(refund);

        Destroy(selected.gameObject);
        selected = null;
    }
}