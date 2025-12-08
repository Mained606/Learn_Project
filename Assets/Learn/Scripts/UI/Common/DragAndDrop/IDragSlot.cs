using UnityEngine.UI;

/// <summary>
/// 범용 드래그/드롭 슬롯 인터페이스. 어떤 페이로드든 object로 취급한다.
/// </summary>
public interface IDragSlot
{
    object GetPayload();
    int GetCount();
    void Remove(int count);

    int MaxAcceptable(object payload);
    void Add(object payload, int count);

    // 드래그 프리뷰용 아이콘
    Image GetPreviewImage();
}
