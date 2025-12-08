using UnityEngine;

/// <summary>
/// 드랍 시 수용 가능한 대상.
/// </summary>
public interface IDragDestination<T> where T : class
{
    // 해당 데이터를 최대 몇 개까지 수용 가능한지
    int MaxAcceptable(T item);

    // 드랍된 데이터를 추가
    void AddItem(T item, int count);
}
