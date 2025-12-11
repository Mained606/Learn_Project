/// <summary>
/// 아이템 정의 공급자 인터페이스. Addressables/DB/ScriptableObject 등 구현 교체 가능.
/// </summary>
public interface IItemDefinitionProvider
{
    ItemDefinition GetDefinition(string itemId);
}
