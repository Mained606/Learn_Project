using UnityEngine;

/// <summary>
/// 씬에 배치되는 바닥 아이템. IInteractable과 분리하여 트리거 수집만 담당.
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [Header("아이템 정의")]
    [SerializeField] private ItemDefinition itemDefinition;
    [SerializeField] private int quantity = 1;

    public ItemDefinition Definition => itemDefinition;
    public int Quantity => quantity;

    // 바닥 아이템 태그 강제
    private void Reset()
    {
        gameObject.tag = "Item";
    }

    /// <summary>
    /// 트리거 수집 시 호출. 실제 인벤토리 연동은 이후 Inventory 시스템에서 처리.
    /// </summary>
    public void Collect(GameObject collector)
    {
        if (itemDefinition == null)
        {
            Debug.LogWarning("[ItemPickup] ItemDefinition이 비어 있습니다. 수집을 건너뜁니다.");
            return;
        }

        ItemData runtimeData = itemDefinition.ToItemData(quantity);

        PlayerInventory inventory = collector.GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            Debug.LogWarning("[ItemPickup] PlayerInventory를 찾을 수 없어 수집을 건너뜁니다.");
            return;
        }

        bool added = inventory.TryAddItem(runtimeData);
        if (!added)
        {
            Debug.LogWarning("[ItemPickup] 인벤토리에 추가하지 못했습니다.");
            return;
        }

        Debug.Log($"[ItemPickup] {collector.name} 인벤토리에 {runtimeData.displayName} x{runtimeData.quantity} 추가");

        Destroy(gameObject);
    }
}
