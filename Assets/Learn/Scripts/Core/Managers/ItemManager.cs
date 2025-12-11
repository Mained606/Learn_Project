using System;
using UnityEngine;

/// <summary>
/// 아이템 도메인 중앙 관리. 정의 조회, 드롭 스폰, 글로벌 이벤트 브로드캐스트 담당.
/// </summary>
public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] private ItemDefinitionDatabase definitionDatabase;
    [SerializeField] private GameObject dropPrefab;

    public event Action<ItemData> OnItemDropped;
    public event Action<ItemData> OnItemConsumed;
    public event Action<ItemData> OnItemEquipped;
    public event Action<ItemData> OnItemUnequipped;

    public ItemDefinition GetDefinition(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return null;
        if (definitionDatabase == null) return null;
        return definitionDatabase.GetDefinition(itemId);
    }

    /// <summary>
    /// 바닥에 아이템 드롭 스폰(현재는 프리팹 Instantiate, 추후 풀링/Addressables 교체 가능).
    /// </summary>
    public void SpawnDrop(ItemData data, Vector3 position)
    {
        if (data == null) return;
        if (dropPrefab == null)
        {
            Debug.LogWarning("[ItemManager] DropPrefab이 설정되지 않았습니다.");
            return;
        }

        GameObject go = Instantiate(dropPrefab, position, Quaternion.identity);
        ItemPickup pickup = go.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            ItemDefinition def = GetDefinition(data.itemId);
            ItemData spawnData = def != null ? def.ToItemData(data.quantity) : data;
            pickup.Setup(def, spawnData.quantity);
        }

        OnItemDropped?.Invoke(data);
    }

    public void RaiseDropped(ItemData data) => OnItemDropped?.Invoke(data);
    public void RaiseConsumed(ItemData data) => OnItemConsumed?.Invoke(data);
    public void RaiseEquipped(ItemData data) => OnItemEquipped?.Invoke(data);
    public void RaiseUnequipped(ItemData data) => OnItemUnequipped?.Invoke(data);
}
