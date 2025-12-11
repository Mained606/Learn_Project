using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 슬롯 우클릭/메뉴 표시.
/// 동적 액션 목록으로 버튼을 생성한다.
/// </summary>
public class InventoryContextMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform buttonContainer;
    [SerializeField] private ContextMenuButtonView buttonPrefab;
    [SerializeField] private Vector2 offset = new Vector2(90f, -90f);
    [SerializeField] private Vector2 padding = new Vector2(10f, 10f);

    private readonly List<ContextMenuButtonView> buttonPool = new();
    private InventorySlotView targetSlot;
    private InventoryUI inventoryUI;

    private void Awake()
    {
        if (inventoryUI == null)
            inventoryUI = GetComponentInParent<InventoryUI>();

        if (panel != null) panel.SetActive(false);
    }

    public void Initialize(InventoryUI ui)
    {
        inventoryUI = ui;
    }

    public void Show(InventorySlotView slot, Vector3 position)
    {
        if (inventoryUI == null)
            inventoryUI = GetComponentInParent<InventoryUI>();

        // 패널이 꺼져 있으면 먼저 켜서 레이아웃 계산을 보장
        if (panel != null && !panel.activeSelf)
            panel.SetActive(true);

        targetSlot = slot;
        ItemData item = slot?.GetPayload() as ItemData;
        ItemType type = item != null ? item.itemType : ItemType.Misc;
        bool canSplit = item != null && item.stackable && item.quantity > 1;
        bool isEquipped = slot != null && inventoryUI != null && inventoryUI.IsEquipped(slot);

        var actions = new List<ContextMenuAction>();

        if (type == ItemType.Consumable)
            actions.Add(new ContextMenuAction("Use", () => { inventoryUI?.RequestUse(targetSlot); Hide(); }));

        if (type == ItemType.Equipment && !isEquipped)
            actions.Add(new ContextMenuAction("Equip", () => { inventoryUI?.RequestEquip(targetSlot); Hide(); }));

        if (type == ItemType.Equipment && isEquipped)
            actions.Add(new ContextMenuAction("Unequip", () => { inventoryUI?.RequestUnequip(targetSlot); Hide(); }));

        if (canSplit)
            actions.Add(new ContextMenuAction("Split", () => { inventoryUI?.RequestSplit(targetSlot); Hide(); }));

        if (type != ItemType.Quest)
            actions.Add(new ContextMenuAction("Drop", () => { inventoryUI?.RequestDrop(targetSlot); Hide(); }));

        BuildButtons(actions);

        if (panel != null)
        {
            if (buttonContainer != null)
                LayoutRebuilder.ForceRebuildLayoutImmediate(buttonContainer);

            Vector3 targetPos = AdjustToScreen(position + (Vector3)offset);
            panel.transform.position = targetPos;
            panel.SetActive(true);
        }
    }

    public void Hide()
    {
        targetSlot = null;
        if (panel != null)
            panel.SetActive(false);
    }

    public bool IsVisible => panel != null && panel.activeSelf;

    private void BuildButtons(List<ContextMenuAction> actions)
    {
        if (buttonContainer == null || buttonPrefab == null) return;

        while (buttonPool.Count < actions.Count)
        {
            var btn = Instantiate(buttonPrefab, buttonContainer);
            buttonPool.Add(btn);
        }

        for (int i = 0; i < buttonPool.Count; i++)
        {
            bool active = i < actions.Count;
            buttonPool[i].gameObject.SetActive(active);
            if (active)
                buttonPool[i].Bind(actions[i]);
        }
    }

    private Vector3 AdjustToScreen(Vector3 position)
    {
        Vector3 targetPos = position;

        RectTransform panelRect = panel.transform as RectTransform;
        Canvas canvas = panel.GetComponentInParent<Canvas>();
        if (panelRect != null && canvas != null && canvas.renderMode != RenderMode.WorldSpace)
        {
            Vector2 size = panelRect.rect.size;
            float screenW = Screen.width;
            float screenH = Screen.height;

            targetPos.x = Mathf.Clamp(targetPos.x, padding.x, screenW - size.x - padding.x);
            targetPos.y = Mathf.Clamp(targetPos.y, padding.y + size.y, screenH - padding.y);
        }

        return targetPos;
    }
}
