using UnityEngine;
using DG.Tweening;

public class Inventory : MonoBehaviour
{
    [Header("Inventory")]
    public Item heldItem;
    [SerializeField] [Rename("Follow Speed")] private float followSpd;
    [SerializeField] [Rename("Rotation Speed")] private float rotSpd;

    private void Update() {
        if (heldItem == null) return;

        Vector3 offset = transform.right * heldItem.heldOffset.x +
                         transform.up * heldItem.heldOffset.y +
                         transform.forward * heldItem.heldOffset.z;
        
        Vector3 heldPos = transform.position + offset;

        heldItem.transform.SetPositionAndRotation(Vector3.Lerp(heldItem.transform.position, heldPos, followSpd * Time.deltaTime), 
        Quaternion.Lerp(heldItem.transform.rotation, transform.rotation, rotSpd * Time.deltaTime));
    }

    public void DiscardItem() {
        if (heldItem == null) return;

        heldItem.OnDrop();
        heldItem = null;    
    }
}
