using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 인벤토리 기본 구현. 슬롯 단위로 아이템을 관리하고, 동일 아이템은 스택으로 합산.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [Header("기본 슬롯 수")]
    [SerializeField] private int baseSlots = 99;

    [Header("추가 슬롯 수 (캐시 아이템 등으로 확장)")]
    [SerializeField] private int extraSlots = 0;

    [Header("디버그 로그 출력 여부")]
    [SerializeField] private bool logPickup = true;

    private readonly List<ItemData> items = new List<ItemData>();

    public IReadOnlyList<ItemData> Items => items;
    public int CurrentSlotCapacity => baseSlots + extraSlots;

    public event Action<IReadOnlyList<ItemData>> OnInventoryChanged;

    private void Awake()
    {
        EnsureCapacity();
    }

    // 현재 슬롯 용량만큼 내부 리스트를 패딩하여 빈 슬롯을 표현
    private void EnsureCapacity()
    {
        int capacity = CurrentSlotCapacity;

        while (items.Count < capacity)
        {
            items.Add(null);
        }

        if (items.Count > capacity)
        {
            items.RemoveRange(capacity, items.Count - capacity);
        }
    }

    /// <summary>
    /// 슬롯 순서 변경 (드래그 앤 드롭 대응)
    /// </summary>
    public bool SwapItems(int fromIndex, int toIndex)
    {
        EnsureCapacity();

        if (fromIndex == toIndex) return false;
        if (fromIndex < 0 || fromIndex >= CurrentSlotCapacity) return false;
        if (toIndex < 0 || toIndex >= CurrentSlotCapacity) return false;

        ItemData temp = items[fromIndex];
        items[fromIndex] = items[toIndex];
        items[toIndex] = temp;

        RaiseChanged();
        return true;
    }

    /// <summary>
    /// 아이템 추가 시도. 동일 ID는 스택으로 합산하고, 슬롯 초과 시 실패.
    /// </summary>
    public bool TryAddItem(ItemData itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("[PlayerInventory] 전달된 아이템 데이터가 없습니다.");
            return false;
        }

        if (string.IsNullOrEmpty(itemData.itemId))
        {
            Debug.LogWarning("[PlayerInventory] 아이템 ID가 비어 있어 추가하지 않습니다.");
            return false;
        }

        if (itemData.quantity <= 0)
        {
            Debug.LogWarning("[PlayerInventory] 수량이 0 이하인 아이템은 추가하지 않습니다.");
            return false;
        }

        EnsureCapacity();

        ItemData existing = items.Find(x => x != null && x.itemId == itemData.itemId);

        if (existing != null)
        {
            existing.quantity += itemData.quantity;
            LogPickup(itemData);
            RaiseChanged();
            return true;
        }

        int emptyIndex = items.FindIndex(x => x == null);

        if (emptyIndex == -1)
        {
            Debug.LogWarning("[PlayerInventory] 인벤토리 슬롯이 가득 찼습니다.");
            return false;
        }

        ItemData stored = new ItemData(itemData.itemId, itemData.displayName, itemData.description, itemData.quantity, itemData.iconKey);
        items[emptyIndex] = stored;
        LogPickup(itemData);
        RaiseChanged();
        return true;
    }

    /// <summary>
    /// 캐시 아이템 등으로 슬롯을 확장.
    /// </summary>
    public bool TryAddExtraSlots(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("[PlayerInventory] 0 이하 슬롯 증가는 무시합니다.");
            return false;
        }

        extraSlots += amount;
        RaiseChanged();
        return true;
    }

    private void LogPickup(ItemData itemData)
    {
        if (!logPickup) return;

        Debug.Log($"[PlayerInventory] {itemData.displayName} x{itemData.quantity} 획득");
    }

    private void RaiseChanged()
    {
        OnInventoryChanged?.Invoke(items);
    }

    /// <summary>
    /// 특정 슬롯을 비웁니다.
    /// </summary>
    public void ClearSlot(int index)
    {
        EnsureCapacity();
        if (index < 0 || index >= CurrentSlotCapacity) return;

        items[index] = null;
        RaiseChanged();
    }

    /// <summary>
    /// 특정 슬롯 값을 갱신합니다.
    /// </summary>
    public void UpdateSlot(int index, ItemData item)
    {
        EnsureCapacity();
        if (index < 0 || index >= CurrentSlotCapacity) return;

        items[index] = item;
        RaiseChanged();
    }
}
