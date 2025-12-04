using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private ItemPickup currentItem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ItemPickup item))
            currentItem = item;
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentItem != null && other.TryGetComponent(out ItemPickup item))
            currentItem = null;
    }

    public void TryPickup()
    {
        if (currentItem == null) return;

        currentItem.Pickup();
        currentItem = null;
    }
}