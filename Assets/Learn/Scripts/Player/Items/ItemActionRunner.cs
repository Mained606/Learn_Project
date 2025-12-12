using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 사용/장착/드롭을 오케스트레이션한다.
/// UI 등 외부에서는 슬롯 인덱스와 액션만 요청하고,
/// 실제 처리와 스탯 적용은 런너가 담당한다.
/// </summary>
[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(PlayerStats))]
public class ItemActionRunner : MonoBehaviour
{
    [SerializeField] private ItemActionResolver actionResolver;

    private PlayerInventory inventory;
    private PlayerStats stats;
    // 장착된 아이템 인스턴스 추적(참조 기준)
    private readonly HashSet<ItemData> equippedItems = new();
    // 장착 부위 -> 아이템 인스턴스
    private readonly Dictionary<EquipmentSlot, ItemData> equippedBySlot = new();
    [Header("디버그용 장착 리스트(읽기 전용)")]
    [SerializeField] private List<string> equippedDebug = new();

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        stats = GetComponent<PlayerStats>();

        if (actionResolver == null)
        {
            Debug.LogWarning("[ItemActionRunner] ItemActionResolver가 할당되지 않았습니다.");
        }
    }

    public void Use(int slotIndex)
    {
        if (actionResolver == null) return;
        if (slotIndex < 0 || slotIndex >= inventory.CurrentSlotCapacity) return;

        ItemData data = inventory.Items.Count > slotIndex ? inventory.Items[slotIndex] : null;
        if (data == null) return;
        if (!actionResolver.CanConsume(data)) return;

        ItemDefinition def = ItemManager.Instance != null ? ItemManager.Instance.GetDefinition(data.itemId) : null;
        actionResolver.Consume(data, def, inventory, stats, slotIndex);
    }

    public void Equip(int slotIndex)
    {
        if (actionResolver == null) return;
        if (slotIndex < 0 || slotIndex >= inventory.CurrentSlotCapacity) return;
        ItemData data = inventory.Items.Count > slotIndex ? inventory.Items[slotIndex] : null;
        if (data == null) return;
        if (!actionResolver.CanEquip(data)) return;

        ItemDefinition def = ResolveDefinition(data);
        EquipmentSlot eqSlotType = def != null ? def.EquipmentSlot : EquipmentSlot.None;

        if (eqSlotType == EquipmentSlot.None)
        {
            Debug.LogWarning("[ItemActionRunner] 장착 불가 아이템입니다.");
            return;
        }

        // 동일 아이템 인스턴스가 이미 장착되어 있으면 무시
        if (equippedItems.Contains(data))
        {
            Debug.LogWarning("[ItemActionRunner] 이미 장착된 아이템입니다.");
            return;
        }

        // 동일 부위에 다른 아이템이 장착되어 있으면 먼저 해제
        if (equippedBySlot.TryGetValue(eqSlotType, out ItemData equippedItem))
        {
            UnequipItem(equippedItem);
        }

        actionResolver.Equip(data, def, inventory, stats, slotIndex);
        equippedItems.Add(data);
        equippedBySlot[eqSlotType] = data;
        RefreshDebugList();
    }

    public void Drop(int slotIndex)
    {
        if (actionResolver == null) return;
        if (slotIndex < 0 || slotIndex >= inventory.CurrentSlotCapacity) return;

        ItemData data = inventory.Items.Count > slotIndex ? inventory.Items[slotIndex] : null;
        if (data == null) return;
        if (!actionResolver.CanDrop(data)) return;

        // 장착 상태였다면 먼저 해제
        Unequip(slotIndex);

        actionResolver.Drop(data, inventory, slotIndex);
    }

    public void Unequip(int slotIndex)
    {
        if (actionResolver == null) return;
        if (slotIndex < 0 || slotIndex >= inventory.CurrentSlotCapacity) return;

        ItemData data = inventory.Items.Count > slotIndex ? inventory.Items[slotIndex] : null;
        if (data == null) return;
        UnequipItem(data);
    }

    public bool IsEquipped(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.CurrentSlotCapacity) return false;
        ItemData data = inventory.Items.Count > slotIndex ? inventory.Items[slotIndex] : null;
        if (data == null) return false;
        return equippedItems.Contains(data);
    }

    // 아이템 인스턴스 단위로 장착 해제 처리
    private void UnequipItem(ItemData item)
    {
        if (item == null) return;
        if (!equippedItems.Contains(item)) return;

        ItemDefinition def = ResolveDefinition(item);
        EquipmentSlot slotType = def != null ? def.EquipmentSlot : EquipmentSlot.None;

        actionResolver.Unequip(item, def, stats);

        equippedItems.Remove(item);
        if (slotType != EquipmentSlot.None && equippedBySlot.TryGetValue(slotType, out ItemData stored) && stored == item)
        {
            equippedBySlot.Remove(slotType);
        }
        RefreshDebugList();
    }

    private ItemDefinition ResolveDefinition(ItemData item)
    {
        if (item == null) return null;
        return ItemManager.Instance != null ? ItemManager.Instance.GetDefinition(item.itemId) : null;
    }

    // 인스펙터에서 장착 상태를 눈으로 확인하기 위한 디버그 리스트 갱신
    private void RefreshDebugList()
    {
        equippedDebug.Clear();
        foreach (KeyValuePair<EquipmentSlot, ItemData> kv in equippedBySlot)
        {
            ItemData data = kv.Value;
            string name = data != null ? (!string.IsNullOrEmpty(data.displayName) ? data.displayName : data.itemId) : "None";
            equippedDebug.Add($"{kv.Key}: {name}");
        }
    }
}
