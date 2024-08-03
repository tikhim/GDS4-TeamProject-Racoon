using UnityEngine;
using DG.Tweening;


public class Item : MonoBehaviour, IInteractable
{
    public Vector3 heldOffset;

    private bool dropped;

    public void OnInteract(GameObject source)
    {
        if (dropped) return;

        Inventory _inv = source.GetComponent<Inventory>();

        if (_inv.heldItem == null)  {
            _inv.heldItem = this;
            return;
        }

        _inv.DiscardItem();
    }

    public void OnDrop() {
        dropped = true;

        transform.DOMoveY(0f, 1f).SetEase(Ease.OutBounce);

        Vector3 rot = transform.rotation.eulerAngles;
        rot.y += (Random.Range(0,2)*2-1) * 45f;

        transform.DORotate(rot, 1f, RotateMode.Fast).SetEase(Ease.InSine).OnComplete(()=>{
            dropped = false;
        });
    }
}
