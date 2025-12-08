using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 인벤토리 슬롯 우클릭/메뉴 표시.
/// Drop/Split/Use/Equip 액션을 제공하며, 실제 로직은 InventoryUI로 위임.
/// </summary>
public class InventoryContextMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform buttonContainer;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button splitButton;
    [SerializeField] private Button useButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Vector2 offset = new Vector2(90f, -90f);
    [SerializeField] private Vector2 padding = new Vector2(10f, 10f);

    private InventorySlotView targetSlot;
    private InventoryUI inventoryUI;

    private void Awake()
    {
        // 인벤토리 UI 참조가 비어 있으면 상위에서 자동으로 찾기
        if (inventoryUI == null)
            inventoryUI = GetComponentInParent<InventoryUI>();

        if (panel != null) panel.SetActive(false);
        if (dropButton != null) dropButton.onClick.AddListener(HandleDrop);
        if (splitButton != null) splitButton.onClick.AddListener(HandleSplit);
        if (useButton != null) useButton.onClick.AddListener(HandleUse);
        if (equipButton != null) equipButton.onClick.AddListener(HandleEquip);
    }

    public void Initialize(InventoryUI ui)
    {
        inventoryUI = ui;
    }

    public void Show(InventorySlotView slot, Vector3 position)
    {
        if (inventoryUI == null)
            inventoryUI = GetComponentInParent<InventoryUI>();

        targetSlot = slot;
        ItemData item = slot?.GetPayload() as ItemData;
        ItemType type = item != null ? item.itemType : ItemType.Misc;
        bool canSplit = item != null && item.stackable && item.quantity > 1;

        // 버튼 노출/순서 결정
        SetButtonActive(useButton, type == ItemType.Consumable);
        SetButtonActive(equipButton, type == ItemType.Equipment);
        SetButtonActive(splitButton, canSplit);
        // 퀘스트 아이템은 드롭 비활성화
        SetButtonActive(dropButton, type != ItemType.Quest);

        // 순서: Use/Eqip -> Split -> Drop
        int order = 0;
        if (useButton != null && useButton.gameObject.activeSelf) useButton.transform.SetSiblingIndex(order++);
        if (equipButton != null && equipButton.gameObject.activeSelf) equipButton.transform.SetSiblingIndex(order++);
        if (splitButton != null && splitButton.gameObject.activeSelf) splitButton.transform.SetSiblingIndex(order++);
        if (dropButton != null && dropButton.gameObject.activeSelf) dropButton.transform.SetSiblingIndex(order++);

        if (panel != null)
        {
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

    private void HandleDrop()
    {
        if (targetSlot == null)
        {
            Hide();
            return;
        }

        inventoryUI?.RequestDrop(targetSlot);
        Hide();
    }

    private void HandleSplit()
    {
        Debug.Log("[InventoryContextMenu] Split 클릭");

        if (targetSlot == null)
        {
            Hide();
            return;
        }

        inventoryUI?.RequestSplit(targetSlot);
        Hide();
    }

    private void HandleUse()
    {
        if (targetSlot == null)
        {
            Hide();
            return;
        }

        inventoryUI?.RequestUse(targetSlot);
        Hide();
    }

    private void HandleEquip()
    {
        if (targetSlot == null)
        {
            Hide();
            return;
        }

        inventoryUI?.RequestEquip(targetSlot);
        Hide();
    }

    public bool IsVisible => panel != null && panel.activeSelf;

    private void SetButtonActive(Button button, bool active)
    {
        if (button != null)
            button.gameObject.SetActive(active);
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
