using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private string itemName;

    public void Pickup()
    {
        Debug.Log($"{itemName} 획득!");
        Destroy(gameObject); // 이후 인벤토리로 전달
    }
}