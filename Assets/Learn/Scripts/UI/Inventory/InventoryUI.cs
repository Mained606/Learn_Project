using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스크롤 뷰 기반 인벤토리 UI. 아이콘, 수량 표시 및 드래그 앤 드롭 순서 변경을 지원.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private RectTransform contentRoot;
    [SerializeField] private InventorySlotView slotPrefab;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private InventoryContextMenu contextMenu;
    [SerializeField] private InventoryTooltip tooltip;
    [SerializeField] private InventoryDetailPanel detailPanel;
    [Header("아이템 액션 런너 (플레이어 도메인)")]
    [SerializeField] private ItemActionRunner itemActionRunner;

    private readonly List<InventorySlotView> slotViews = new List<InventorySlotView>();
    private InventorySlotView currentDetailSlot;
    private ItemManager itemManager;

    public PlayerInventory Inventory => playerInventory;

    private void Awake()
    {
        itemManager = ItemManager.Instance;

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
        tooltip?.Hide();
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

            if (data != null && itemManager != null)
                def = itemManager.GetDefinition(data.itemId);

            if (data != null)
                slot.Bind(this, i, data, def, rootCanvas);
            else
                slot.BindEmpty(this, i, rootCanvas);

            // 컨텍스트 메뉴/툴팁 주입
            if (contextMenu != null)
                slot.SetContextMenu(contextMenu);
            if (tooltip != null)
                slot.SetTooltip(tooltip);
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

    // 컨텍스트 메뉴에서 호출될 Drop/ Split/Use/Equip 훅
    public void RequestDrop(InventorySlotView slot)
    {
        if (slot == null || itemActionRunner == null) return;
        itemActionRunner.Drop(slot.Index);
        HideDetail();
        contextMenu?.Hide();
        tooltip?.Hide();
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
        tooltip?.Hide();
    }

    public void RequestUse(InventorySlotView slot)
    {
        if (slot == null || itemActionRunner == null) return;
        itemActionRunner.Use(slot.Index);
        HideDetail();
        contextMenu?.Hide();
        tooltip?.Hide();
    }

    public void RequestEquip(InventorySlotView slot)
    {
        if (slot == null || itemActionRunner == null) return;
        itemActionRunner.Equip(slot.Index);
        HideDetail();
        contextMenu?.Hide();
        tooltip?.Hide();
    }

    public void RequestUnequip(InventorySlotView slot)
    {
        if (slot == null || itemActionRunner == null) return;
        itemActionRunner.Unequip(slot.Index);
        HideDetail();
        contextMenu?.Hide();
        tooltip?.Hide();
    }

    public bool IsEquipped(InventorySlotView slot)
    {
        if (slot == null || itemActionRunner == null) return false;
        return itemActionRunner.IsEquipped(slot.Index);
    }

    public void ShowDetail(InventorySlotView slot)
    {
        if (detailPanel == null || slot == null) return;

        ItemData item = slot.GetPayload() as ItemData;
        ItemDefinition def = null;
        if (item != null && itemManager != null)
            def = itemManager.GetDefinition(item.itemId);

        detailPanel.Show(item, def);
        currentDetailSlot = slot;
        // 상세 패널이 열릴 때 컨텍스트 메뉴/툴팁 닫기
        contextMenu?.Hide();
        tooltip?.Hide();
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
