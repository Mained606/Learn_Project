using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스크롤 뷰 기반 인벤토리 UI. 아이콘, 수량 표시 및 드래그 앤 드롭 순서 변경을 지원.
/// UI 문자열은 영어, 주석/로그는 한글로 유지.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private ItemDefinitionDatabase definitionDatabase;
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private InventorySlotView slotPrefab;
    [SerializeField] private Canvas rootCanvas;

    private readonly List<InventorySlotView> slotViews = new List<InventorySlotView>();

    public PlayerInventory Inventory => playerInventory;

    private void Awake()
    {
        // 캔버스가 지정되지 않은 경우 상위에서 찾아 설정
        if (rootCanvas == null)
            rootCanvas = GetComponentInParent<Canvas>();
    }

    private void OnEnable()
    {
        if (playerInventory == null) return;

        playerInventory.OnInventoryChanged += Refresh;
        Refresh(playerInventory.Items);
    }

    private void OnDisable()
    {
        if (playerInventory == null) return;

        playerInventory.OnInventoryChanged -= Refresh;
    }

    // 인벤토리 데이터 변경 시 슬롯 리스트를 갱신
    private void Refresh(IReadOnlyList<ItemData> items)
    {
        if (contentRoot == null || slotPrefab == null) return;

        int capacity = playerInventory != null ? playerInventory.CurrentSlotCapacity : (items?.Count ?? 0);
        int itemCount = items?.Count ?? 0;

        // 총 슬롯 용량만큼 생성하고, 아이템이 없는 슬롯은 빈 상태로 표시
        EnsureSlotCount(capacity);

        for (int i = 0; i < slotViews.Count; i++)
        {
            InventorySlotView slot = slotViews[i];
            ItemData data = (items != null && i < items.Count) ? items[i] : null;
            ItemDefinition def = null;

            if (data != null && definitionDatabase != null)
                def = definitionDatabase.GetDefinition(data.itemId);

            if (data != null)
                slot.Bind(this, i, data, def, rootCanvas);
            else
                slot.BindEmpty(this, i, rootCanvas);
        }
    }

    // 슬롯 수를 데이터에 맞춰 조정
    private void EnsureSlotCount(int targetCount)
    {
        // 부족한 슬롯 생성
        while (slotViews.Count < targetCount)
        {
            InventorySlotView slot = Instantiate(slotPrefab, contentRoot);
            slotViews.Add(slot);
        }

        // 남는 슬롯은 비활성화
        for (int i = 0; i < slotViews.Count; i++)
        {
            bool active = i < targetCount;
            if (slotViews[i].gameObject.activeSelf != active)
            {
                slotViews[i].gameObject.SetActive(active);
            }
        }
    }

}
