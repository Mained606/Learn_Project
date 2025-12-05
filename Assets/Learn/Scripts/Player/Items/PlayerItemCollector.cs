using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private ItemPickup currentItem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ItemPickup>(out var item))
        {
            currentItem = item;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ItemPickup>(out var item))
        {
            if (currentItem == item)
                currentItem = null;
        }
    }

    public void TryPickup()
    {
        if (currentItem != null)
        {
            currentItem.Pickup();
            currentItem = null;
        }
    }
}