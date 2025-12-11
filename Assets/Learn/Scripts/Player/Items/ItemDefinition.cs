using UnityEngine;

/// <summary>
/// 클라이언트 전용 아이템 정의. Addressables, Firestore 매핑을 고려한 데이터 브릿지.
/// </summary>
[CreateAssetMenu(menuName = "Learn/Items/ItemDefinition")]
public class ItemDefinition : ScriptableObject
{
    [Header("식별 정보")]
    [SerializeField] private string itemId;
    [SerializeField] private string iconKey;
    
    [Header("타입/정책")]
    [SerializeField] private ItemType itemType = ItemType.Misc;
    [SerializeField] private bool stackable = true;
    [SerializeField] private int maxStack = 99;

    [Header("표시 정보")]
    [SerializeField] private string displayName;
    [TextArea] [SerializeField] private string description;
    [SerializeField] private Sprite icon;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public string Description => description;
    public string IconKey => iconKey;
    public Sprite Icon => icon;
    public ItemType ItemType => itemType;
    public bool Stackable => stackable;
    public int MaxStack => maxStack;

    /// <summary>
    /// 런타임에서 사용할 순수 데이터로 변환.
    /// </summary>
    public ItemData ToItemData(int quantity = 1)
    {
        return new ItemData(itemId, displayName, description, quantity, iconKey, itemType, stackable, maxStack);
    }
}
