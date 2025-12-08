using UnityEngine;

/// <summary>
/// 씬에 배치되는 바닥 아이템. IInteractable과 분리하여 트리거 수집만 담당.
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [Header("아이템 정의")]
    [SerializeField] private ItemDefinition itemDefinition;
    [SerializeField] private int quantity = 1;

    public ItemDefinition Definition => itemDefinition;
    public int Quantity => quantity;

    // 바닥 아이템 태그 강제
    private void Reset()
    {
        gameObject.tag = "Item";
    }

    /// <summary>
    /// 트리거 수집 시 호출. 실제 인벤토리 연동은 이후 Inventory 시스템에서 처리.
    /// </summary>
    public void Collect(GameObject collector)
    {
        if (itemDefinition == null)
        {
            Debug.LogWarning("[ItemPickup] ItemDefinition이 비어 있습니다. 수집을 건너뜁니다.");
            return;
        }

        ItemData runtimeData = itemDefinition.ToItemData(quantity);

        // TODO: 인벤토리 시스템이 준비되면 runtimeData 전달
        Debug.Log($"[ItemPickup] {collector.name} collected {runtimeData.displayName} x{runtimeData.quantity}");

        Destroy(gameObject);
    }
}
