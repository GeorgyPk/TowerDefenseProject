using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TowerSelectionManager : MonoBehaviour
{
    public LayerMask towerMask;
    public TowerInstance selected;

    public void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, towerMask))
        {
            Select(hit.collider.GetComponentInParent<TowerInstance>());
        }
    }

    public void Select(TowerInstance tower)
    {
        if (selected == tower) return;

        ShowSelectedRange(selected, false);
        selected = tower;
        ShowSelectedRange(selected, true);
    }

    private void ShowSelectedRange(TowerInstance t, bool show)
    {
        if (t == null) return;
        var lr = t.GetComponentInChildren<LineRenderer>(true);
        if (lr != null) lr.enabled = show;
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