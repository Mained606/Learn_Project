using UnityEngine;

/// <summary>
/// 클라이언트 전용 아이템 정의. Addressables, Firestore 매핑을 고려한 데이터 브릿지.
/// </summary>
[CreateAssetMenu(menuName = "Learn/Items/ItemDefinition")]
public class ItemDefinition : ScriptableObject
{
    [Header("식별 정보")]
    [SerializeField] private string itemId;

    [Header("표시 정보")]
    [SerializeField] private string displayName;
    [TextArea] [SerializeField] private string description;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public string Description => description;

    /// <summary>
    /// 런타임에서 사용할 순수 데이터로 변환.
    /// </summary>
    public ItemData ToItemData(int quantity = 1)
    {
        return new ItemData(itemId, displayName, description, quantity);
    }
}
