using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 슬롯 UI. 아이콘/수량 표시와 드래그 앤 드롭 데이터 제공/수신을 처리한다.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class InventorySlotView : MonoBehaviour, IDragSlot
{
    [Header("UI 참조")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;

    private InventoryUI owner;
    private int slotIndex;
    private ItemData itemData;
    private ItemDefinition itemDefinition;
    private Canvas rootCanvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private DragHandler dragHandler;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        dragHandler = GetComponent<DragHandler>();
    }

    public void Bind(InventoryUI owner, int index, ItemData data, ItemDefinition definition, Canvas canvas)
    {
        this.owner = owner;
        slotIndex = index;
        itemData = data;
        itemDefinition = definition;
        rootCanvas = canvas;

        RefreshVisual();

        SetupDragHandler();
    }

    public void BindEmpty(InventoryUI owner, int index, Canvas canvas)
    {
        this.owner = owner;
        slotIndex = index;
        itemData = null;
        itemDefinition = null;
        rootCanvas = canvas;

        RefreshVisual();

        SetupDragHandler();
    }

    private void RefreshVisual()
    {
        if (itemData == null)
        {
            if (iconImage != null) iconImage.enabled = false;
            if (quantityText != null) quantityText.text = "";
            return;
        }

        if (iconImage != null)
        {
            iconImage.enabled = true;
            iconImage.sprite = itemDefinition != null ? itemDefinition.Icon : null;
            iconImage.color = iconImage.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0.4f);
        }

        if (quantityText != null)
        {
            if (itemData.stackable && itemData.quantity > 1)
                quantityText.text = $"x{itemData.quantity}";
            else
                quantityText.text = "";
        }
    }

    private void SetupDragHandler()
    {
        if (dragHandler == null)
        {
            dragHandler = gameObject.AddComponent<DragHandler>();
        }

        if (dragHandler != null)
        {
            if (rootCanvas != null)
                dragHandler.SetOverrideCanvas(rootCanvas);
        }
    }

    // IDragSlot 구현
    public object GetPayload()
    {
        return itemData;
    }

    public int GetCount()
    {
        return itemData != null ? itemData.quantity : 0;
    }

    public void Remove(int count)
    {
        if (owner == null || owner.Inventory == null || itemData == null) return;

        // 개수 차감 후 0 이하이면 빈 슬롯 처리
        if (itemData.quantity > count)
        {
            itemData.quantity -= count;
            owner.Inventory.UpdateSlot(slotIndex, itemData);
        }
        else
        {
            owner.Inventory.ClearSlot(slotIndex);
        }
    }

    public int MaxAcceptable(object payload)
    {
        // 인벤토리는 어떤 아이템이든 수용 가능하도록 설정 (스왑/이동 허용)
        return payload is ItemData ? int.MaxValue : 0;
    }

    public void Add(object payload, int count)
    {
        if (owner == null || owner.Inventory == null) return;
        if (payload is not ItemData item) return;

        // 동일 아이디는 스택 합산, 아니면 교체
        if (itemData != null && itemData.itemId == item.itemId)
        {
            itemData.quantity += count;
            owner.Inventory.UpdateSlot(slotIndex, itemData);
        }
        else
        {
            ItemData copy = new ItemData(item.itemId, item.displayName, item.description, count, item.iconKey);
            owner.Inventory.UpdateSlot(slotIndex, copy);
        }
    }

    public Image GetPreviewImage()
    {
        return iconImage;
    }
}
