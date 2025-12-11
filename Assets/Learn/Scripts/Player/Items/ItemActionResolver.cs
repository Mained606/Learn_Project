using UnityEngine;

/// <summary>
/// 아이템 타입/속성에 따라 소비/장착/드롭 동작을 처리하는 기본 구현.
/// 추후 스킬/장비 시스템과 연동 시 교체/확장 가능.
/// </summary>
[CreateAssetMenu(menuName = "Learn/Items/ItemActionResolver")]
public class ItemActionResolver : ScriptableObject
{
    public bool CanConsume(ItemData data) => data != null && data.itemType == ItemType.Consumable;
    public bool CanEquip(ItemData data) => data != null && data.itemType == ItemType.Equipment;
    public bool CanDrop(ItemData data) => data != null && data.itemType != ItemType.Quest;

    public void Consume(ItemData data, PlayerInventory inventory, int slotIndex)
    {
        if (data == null || inventory == null) return;
        Debug.Log($"[ItemActionResolver] {data.displayName} 사용");
        // TODO: 실제 효과 적용(회복/버프 등)
        data.quantity -= 1;
        if (data.quantity <= 0)
            inventory.ClearSlot(slotIndex);
        else
            inventory.UpdateSlot(slotIndex, data);
    }

    public void Equip(ItemData data, PlayerInventory inventory, int slotIndex)
    {
        if (data == null || inventory == null) return;
        Debug.Log($"[ItemActionResolver] {data.displayName} 장착 요청");
        // TODO: 장비 시스템 연동 후 실제 장착 처리
    }

    public void Drop(ItemData data, PlayerInventory inventory, int slotIndex, GameObject dropPrefab = null)
    {
        if (data == null || inventory == null) return;
        Debug.Log($"[ItemActionResolver] {data.displayName} 드롭 요청");
        // TODO: dropPrefab 사용해 바닥에 스폰, 현재는 단순 제거
        inventory.ClearSlot(slotIndex);
    }
}
