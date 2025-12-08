using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 ID로 ItemDefinition을 조회하기 위한 간단한 레지스트리.
/// Addressables/Firestore 매핑 전 단계에서 에디터 설정으로 사용.
/// </summary>
[CreateAssetMenu(menuName = "Learn/Items/ItemDefinitionDatabase")]
public class ItemDefinitionDatabase : ScriptableObject
{
    [SerializeField] private List<ItemDefinition> definitions = new List<ItemDefinition>();

    private readonly Dictionary<string, ItemDefinition> cache = new Dictionary<string, ItemDefinition>();

    private void OnEnable()
    {
        BuildCache();
    }

    private void BuildCache()
    {
        cache.Clear();

        foreach (ItemDefinition def in definitions)
        {
            if (def == null || string.IsNullOrEmpty(def.ItemId))
                continue;

            cache[def.ItemId] = def;
        }
    }

    public ItemDefinition GetDefinition(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return null;

        if (cache.TryGetValue(itemId, out ItemDefinition def))
            return def;

        // 정의가 추가되었지만 캐시가 안 맞을 경우 재빌드 시도
        BuildCache();
        cache.TryGetValue(itemId, out def);
        return def;
    }
}
