using System;

/// <summary>
/// JSON/Firestore 직렬화를 위한 순수 데이터 클래스.
/// ScriptableObject가 아닌 런타임 데이터 전송 전용.
/// </summary>
[Serializable]
public class ItemData
{
    public string itemId;
    public string displayName;
    public string description;
    public int quantity;

    // Addressables/리소스 키 등 아이콘 식별용 문자열
    public string iconKey;

    public ItemData(string itemId, string displayName, string description, int quantity = 1, string iconKey = "")
    {
        this.itemId = itemId;
        this.displayName = displayName;
        this.description = description;
        this.quantity = quantity;
        this.iconKey = iconKey;
    }
}
