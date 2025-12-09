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
    [SerializeField] private InventoryContextMenu contextMenu;
    [SerializeField] private TooltipPresenter tooltipPresenter;
    [SerializeField] private InventoryDetailPanel detailPanel;

    private readonly List<InventorySlotView> slotViews = new List<InventorySlotView>();
    private InventorySlotView currentDetailSlot;

    public PlayerInventory Inventory => playerInventory;

    private void Awake()
    {
        // 캔버스가 지정되지 않은 경우 상위에서 찾아 설정
        if (rootCanvas == null)
            rootCanvas = GetComponentInParent<Canvas>();

        // 컨텍스트 메뉴에 자기 참조 전달
        if (contextMenu != null)
            contextMenu.Initialize(this);
    }

    private void OnEnable()
    {
        if (playerInventory == null) return;

        playerInventory.OnInventoryChanged += Refresh;
        Refresh(playerInventory.Items);

        // 패널이 열릴 때 컨텍스트/툴팁/상세 초기화
        contextMenu?.Hide();
        tooltipPresenter?.Hide();
        detailPanel?.Hide();
        currentDetailSlot = null;
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

            // 컨텍스트 메뉴/툴팁 주입
            if (contextMenu != null)
                slot.SetContextMenu(contextMenu);
            if (tooltipPresenter != null)
                slot.SetTooltipPresenter(tooltipPresenter);
            if (detailPanel != null)
                slot.SetDetailPanel(detailPanel);
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

    // 컨텍스트 메뉴에서 호출될 Drop/ Split 훅 (현재는 로그만)
    public void RequestDrop(InventorySlotView slot)
    {
        if (slot == null) return;

        // TODO: Drop 로직(바닥에 스폰) 추가 예정
        Debug.Log("[InventoryUI] Drop 요청 - 추후 구현 필요");
        HideDetail();
        contextMenu?.Hide();
        tooltipPresenter?.Hide();
    }

    public void RequestSplit(InventorySlotView slot)
    {
        if (slot == null) return;

        if (slot.GetPayload() is not ItemData item || item.quantity <= 1)
        {
            Debug.LogWarning("[InventoryUI] 분할할 수 없는 스택입니다.");
            return;
        }

        // 간단한 분할: 절반을 새로운 슬롯에 시도
        int half = item.quantity / 2;
        Debug.Log($"[InventoryUI] Split 요청: {item.displayName} {item.quantity} -> {half} / {item.quantity - half}");

        bool success = playerInventory.TrySplitStack(slot.Index, half);
        if (!success)
        {
            Debug.LogWarning("[InventoryUI] 분할에 실패했습니다. (빈 슬롯 필요)");
        }

        HideDetail();
        contextMenu?.Hide();
        tooltipPresenter?.Hide();
    }

    public void RequestUse(InventorySlotView slot)
    {
        if (slot == null) return;
        Debug.Log("[InventoryUI] Use 요청 - 아이템 사용 로직은 추후 구현");
        HideDetail();
        contextMenu?.Hide();
        tooltipPresenter?.Hide();
    }

    public void RequestEquip(InventorySlotView slot)
    {
        if (slot == null) return;
        Debug.Log("[InventoryUI] Equip 요청 - 장착 시스템은 추후 구현");
        HideDetail();
        contextMenu?.Hide();
        tooltipPresenter?.Hide();
    }

    public void ShowDetail(InventorySlotView slot)
    {
        if (detailPanel == null || slot == null) return;

        ItemData item = slot.GetPayload() as ItemData;
        ItemDefinition def = null;
        if (item != null && definitionDatabase != null)
            def = definitionDatabase.GetDefinition(item.itemId);

        detailPanel.Show(item, def);
        currentDetailSlot = slot;
        // 상세 패널이 열릴 때 컨텍스트 메뉴/툴팁 닫기
        contextMenu?.Hide();
        tooltipPresenter?.Hide();
    }

    public void HideDetail()
    {
        detailPanel?.Hide();
        currentDetailSlot = null;
    }

    public void ToggleDetail(InventorySlotView slot)
    {
        if (detailPanel == null || slot == null) return;

        if (currentDetailSlot == slot && detailPanel.gameObject.activeSelf)
        {
            HideDetail();
        }
        else
        {
            ShowDetail(slot);
        }
    }

}
