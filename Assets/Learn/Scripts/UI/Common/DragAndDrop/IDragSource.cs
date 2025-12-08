using UnityEngine;

/// <summary>
/// 드래그 시작 시 제공 가능한 데이터 소스.
/// </summary>
public interface IDragSource<T> where T : class
{
    // 드래그할 데이터 반환
    T GetItem();

    // 드래그할 개수 반환(스택 대응)
    int GetCount();

    // 드래그 후 소스에서 제거
    void Remove(int count);
}
