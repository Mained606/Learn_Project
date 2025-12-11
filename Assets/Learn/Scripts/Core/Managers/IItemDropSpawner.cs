using UnityEngine;

/// <summary>
/// 드롭 스폰 추상화. Addressables/풀링 등으로 교체 가능.
/// </summary>
public interface IItemDropSpawner
{
    void Spawn(ItemData data, Vector3 position);
}
