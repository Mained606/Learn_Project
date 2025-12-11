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

    public void Consume(ItemData data, ItemDefinition definition, PlayerInventory inventory, PlayerStats stats, int slotIndex)
    {
        if (data == null || inventory == null) return;
        int heal = definition != null ? definition.HealAmount : 0;
        Debug.Log($"[ItemActionResolver] {data.displayName} 사용 (회복: {heal})");

        if (heal > 0 && stats != null)
            stats.Heal(heal);

        data.quantity -= 1;
        if (data.quantity <= 0)
            inventory.ClearSlot(slotIndex);
        else
            inventory.UpdateSlot(slotIndex, data);
    }

    public void Equip(ItemData data, ItemDefinition definition, PlayerInventory inventory, PlayerStats stats, int slotIndex)
    {
        if (data == null || inventory == null) return;
        int atkBonus = definition != null ? definition.AttackBonus : 0;
        Debug.Log($"[ItemActionResolver] {data.displayName} 장착 (공격력 +{atkBonus})");
        if (stats != null && atkBonus != 0)
            stats.attack += atkBonus;

        // TODO: 장비 슬롯/모델 연동 추가 필요
    }

    public void Unequip(ItemData data, ItemDefinition definition, PlayerStats stats)
    {
        if (data == null) return;
        int atkBonus = definition != null ? definition.AttackBonus : 0;
        Debug.Log($"[ItemActionResolver] {data.displayName} 장착 해제 (공격력 -{atkBonus})");
        if (stats != null && atkBonus != 0)
            stats.attack -= atkBonus;
    }

    public void Drop(ItemData data, PlayerInventory inventory, int slotIndex, GameObject dropPrefab = null)
    {
        if (data == null || inventory == null) return;
        Debug.Log($"[ItemActionResolver] {data.displayName} 드롭 요청");
        // TODO: dropPrefab 사용해 바닥에 스폰, 현재는 단순 제거
        inventory.ClearSlot(slotIndex);
    }
}
