using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 여러 인벤토리를 통합 관리하는 매니저 뼈대.
/// - 인벤토리 등록/해제
/// - 기본 인벤토리 조회
/// - 인벤토리 간 아이템 이동(스택 병합은 대상 인벤토리 로직을 그대로 활용)
/// </summary>
public class InventoryManager : Singleton<InventoryManager>
{
    [Header("기본 플레이어 인벤토리(옵션)")]
    [SerializeField] private PlayerInventory defaultInventory;

    private readonly Dictionary<string, PlayerInventory> inventories = new Dictionary<string, PlayerInventory>();

    protected override void Awake()
    {
        base.Awake();

        if (defaultInventory != null)
        {
            RegisterInventory("player", defaultInventory);
        }
    }

    /// <summary>
    /// 인벤토리를 식별자와 함께 등록한다.
    /// </summary>
    public void RegisterInventory(string inventoryId, PlayerInventory inventory)
    {
        if (string.IsNullOrEmpty(inventoryId) || inventory == null) return;

        inventories[inventoryId] = inventory;

        if (defaultInventory == null)
        {
            defaultInventory = inventory;
        }
    }

    /// <summary>
    /// 등록된 인벤토리를 해제한다.
    /// </summary>
    public void UnregisterInventory(string inventoryId, PlayerInventory inventory)
    {
        if (string.IsNullOrEmpty(inventoryId)) return;
        if (!inventories.TryGetValue(inventoryId, out PlayerInventory registered)) return;
        if (registered != inventory) return;

        inventories.Remove(inventoryId);
    }

    /// <summary>
    /// 식별자로 인벤토리를 조회한다. 없으면 기본 인벤토리를 반환.
    /// </summary>
    public PlayerInventory GetInventory(string inventoryId)
    {
        if (!string.IsNullOrEmpty(inventoryId) && inventories.TryGetValue(inventoryId, out PlayerInventory found))
        {
            return found;
        }

        return defaultInventory;
    }

    /// <summary>
    /// 한 인벤토리에서 다른 인벤토리로 스택 전체를 이동한다.
    /// 대상 인벤토리의 병합/스택 정책은 PlayerInventory.TryAddItem을 그대로 사용한다.
    /// </summary>
    public bool TryTransferStack(string fromInventoryId, int fromIndex, string toInventoryId)
    {
        PlayerInventory from = GetInventory(fromInventoryId);
        PlayerInventory to = GetInventory(toInventoryId);

        if (from == null || to == null) return false;
        if (from == to) return false;
        if (fromIndex < 0 || fromIndex >= from.CurrentSlotCapacity) return false;

        ItemData item = from.Items.Count > fromIndex ? from.Items[fromIndex] : null;
        if (item == null) return false;

        // 대상 인벤토리 정책을 활용하기 위해 복사본을 추가 시도
        ItemData copy = new ItemData(item.itemId, item.displayName, item.description, item.quantity, item.iconKey, item.itemType, item.stackable, item.maxStack);
        bool added = to.TryAddItem(copy);

        if (added)
        {
            from.ClearSlot(fromIndex);
        }

        return added;
    }
}
