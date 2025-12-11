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
    [SerializeField] private ItemDefinitionDatabase definitionDatabase;
    [SerializeField] private ItemActionResolver actionResolver;

    private PlayerInventory inventory;
    private PlayerStats stats;
    private readonly System.Collections.Generic.Dictionary<string, int> equippedByItemId = new();
    private readonly System.Collections.Generic.Dictionary<EquipmentSlot, int> equippedBySlot = new();

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

        ItemDefinition def = definitionDatabase != null ? definitionDatabase.GetDefinition(data.itemId) : null;
        actionResolver.Consume(data, def, inventory, stats, slotIndex);
    }

    public void Equip(int slotIndex)
    {
        if (actionResolver == null) return;
        if (slotIndex < 0 || slotIndex >= inventory.CurrentSlotCapacity) return;
        ItemData data = inventory.Items.Count > slotIndex ? inventory.Items[slotIndex] : null;
        if (data == null) return;
        if (!actionResolver.CanEquip(data)) return;

        ItemDefinition def = definitionDatabase != null ? definitionDatabase.GetDefinition(data.itemId) : null;
        EquipmentSlot eqSlotType = def != null ? def.EquipmentSlot : EquipmentSlot.None;

        // 동일 아이템 다시 장착 시 무시
        if (equippedByItemId.TryGetValue(data.itemId, out int prevSlotForItem))
        {
            if (prevSlotForItem == slotIndex)
            {
                Debug.LogWarning("[ItemActionRunner] 이미 해당 아이템이 장착된 슬롯입니다.");
                return;
            }
            Unequip(prevSlotForItem);
        }

        // 동일 부위에 다른 아이템이 장착되어 있으면 해제
        if (eqSlotType != EquipmentSlot.None && equippedBySlot.TryGetValue(eqSlotType, out int prevSlotForSlot))
        {
            if (prevSlotForSlot == slotIndex)
            {
                Debug.LogWarning("[ItemActionRunner] 이미 해당 부위에 장착된 슬롯입니다.");
                return;
            }
            Unequip(prevSlotForSlot);
        }

        ItemDefinition defEquip = definitionDatabase != null ? definitionDatabase.GetDefinition(data.itemId) : null;
        actionResolver.Equip(data, defEquip, inventory, stats, slotIndex);
        equippedByItemId[data.itemId] = slotIndex;
        if (eqSlotType != EquipmentSlot.None)
            equippedBySlot[eqSlotType] = slotIndex;
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

        // 슬롯에 있는 아이템이 장착 목록에 없으면 패스
        ItemData data = inventory.Items.Count > slotIndex ? inventory.Items[slotIndex] : null;
        if (data == null) return;
        if (!equippedByItemId.TryGetValue(data.itemId, out int eqSlot) || eqSlot != slotIndex)
            return;

        ItemDefinition def = definitionDatabase != null ? definitionDatabase.GetDefinition(data.itemId) : null;

        actionResolver.Unequip(data, def, stats);
        equippedByItemId.Remove(data.itemId);

        EquipmentSlot slotType = def != null ? def.EquipmentSlot : EquipmentSlot.None;
        if (slotType != EquipmentSlot.None && equippedBySlot.TryGetValue(slotType, out int storedSlot) && storedSlot == slotIndex)
            equippedBySlot.Remove(slotType);
    }

    public bool IsEquipped(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventory.CurrentSlotCapacity) return false;
        ItemData data = inventory.Items.Count > slotIndex ? inventory.Items[slotIndex] : null;
        if (data == null) return false;
        if (equippedByItemId.TryGetValue(data.itemId, out int eqSlot) && eqSlot == slotIndex)
            return true;

        ItemDefinition def = definitionDatabase != null ? definitionDatabase.GetDefinition(data.itemId) : null;
        EquipmentSlot slotType = def != null ? def.EquipmentSlot : EquipmentSlot.None;
        return slotType != EquipmentSlot.None && equippedBySlot.TryGetValue(slotType, out int slotIdx) && slotIdx == slotIndex;
    }
}
