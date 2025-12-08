using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 슬롯 UI. 아이콘/수량 표시와 드래그 앤 드롭 데이터 제공/수신을 처리한다.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class InventorySlotView : MonoBehaviour, IDragSlot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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
    private InventoryContextMenu contextMenu;
    private InventoryTooltip tooltip;
    private InventoryDetailPanel detailPanel;
    private Coroutine tooltipRoutine;
    private bool pointerInside;

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

    // 툴팁/컨텍스트 메뉴는 IPointerEnter/Exit/Click으로 처리

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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (itemData != null && contextMenu != null)
            {
                tooltip?.Hide();
                CancelTooltipRoutine();
                contextMenu.Show(this, eventData.position);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Shift+좌클릭으로 빠른 분할
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (itemData != null && itemData.stackable && itemData.quantity > 1)
                {
                    owner?.RequestSplit(this);
                }
            }
            else
            {
                owner?.ToggleDetail(this);
            }
        }
        else
        {
            // 다른 버튼 클릭 시 컨텍스트/툴팁 닫기
            contextMenu?.Hide();
            tooltip?.Hide();
            owner?.HideDetail();
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
        if (payload is not ItemData incoming) return 0;

        // 비어 있는 슬롯이면 자유롭게 수용
        if (itemData == null) return int.MaxValue;

        // 같은 아이템이면 스택 가능 여부와 남은 공간 체크
        if (incoming.itemId == itemData.itemId)
        {
            if (!incoming.stackable) return 0;

            int maxStack = GetMaxStack(incoming);
            int space = Mathf.Max(0, maxStack - itemData.quantity);
            return space;
        }

        // 다른 아이템이면 스왑/교체 허용
        return int.MaxValue;
    }

    public void Add(object payload, int count)
    {
        if (owner == null || owner.Inventory == null) return;
        if (payload is not ItemData item) return;

        // 동일 아이디는 스택 합산, 아니면 교체
        if (itemData != null && itemData.itemId == item.itemId)
        {
            if (!itemData.stackable) return;

            int maxStack = GetMaxStack(itemData);
            int addable = Mathf.Min(count, Mathf.Max(0, maxStack - itemData.quantity));
            if (addable <= 0) return;

            itemData.quantity += addable;
            owner.Inventory.UpdateSlot(slotIndex, itemData);
        }
        else
        {
            int maxStack = GetMaxStack(item);
            int clamped = item.stackable ? Mathf.Min(count, maxStack) : 1;
            ItemData copy = new ItemData(item.itemId, item.displayName, item.description, clamped, item.iconKey, item.itemType, item.stackable, item.maxStack);
            owner.Inventory.UpdateSlot(slotIndex, copy);
        }
    }

    public Image GetPreviewImage() => iconImage;
    public void SetContextMenu(InventoryContextMenu menu) => contextMenu = menu;
    public void SetTooltip(InventoryTooltip tip) => tooltip = tip;
    public void SetDetailPanel(InventoryDetailPanel panel) => detailPanel = panel;
    private int GetMaxStack(ItemData data)
    {
        if (data == null) return 0;

        if (!data.stackable) return 1;

        // 정의 우선, 없으면 데이터의 maxStack, 둘 다 없으면 99
        int definedMax = (itemDefinition != null && itemDefinition.ItemId == data.itemId && itemDefinition.MaxStack > 0)
            ? itemDefinition.MaxStack
            : 0;
        int dataMax = data.maxStack > 0 ? data.maxStack : 0;

        int max = definedMax > 0 ? definedMax : (dataMax > 0 ? dataMax : 99);
        return Mathf.Max(1, max);
    }

    public int Index => slotIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerInside = true;
        if (itemData != null && tooltip != null)
        {
            // 컨텍스트 메뉴가 열려 있으면 툴팁은 표시하지 않음
            if (contextMenu != null && contextMenu.IsVisible) return;

            tooltipRoutine = StartCoroutine(ShowTooltipDelayed(eventData.position));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerInside = false;
        CancelTooltipRoutine();
        if (tooltip != null)
            tooltip.Hide();
        // 패널 밖으로 나갈 때 컨텍스트 메뉴는 그대로 두고, 상세는 유지
    }

    private IEnumerator ShowTooltipDelayed(Vector3 position)
    {
        float delay = tooltip != null ? tooltip.HoverDelay : 0f;
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        if (pointerInside && itemData != null && tooltip != null)
        {
            Vector3 currentPos = Input.mousePosition;
            tooltip.Show(itemData, itemDefinition, currentPos);
        }
        tooltipRoutine = null;
    }

    private void CancelTooltipRoutine()
    {
        if (tooltipRoutine != null)
        {
            StopCoroutine(tooltipRoutine);
            tooltipRoutine = null;
        }
    }
}
