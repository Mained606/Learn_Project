using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 범용 드래그/드롭 핸들러. IDragSlot을 구현한 오브젝트에 부착한다.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Canvas overrideCanvas;

    private IDragSlot slot;
    private Canvas targetCanvas;
    private CanvasGroup canvasGroup;
    private RectTransform previewRect;
    private Image previewImage;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        slot = GetComponent<IDragSlot>();
        targetCanvas = overrideCanvas != null ? overrideCanvas : GetComponentInParent<Canvas>();
    }

    public void SetOverrideCanvas(Canvas canvas)
    {
        overrideCanvas = canvas;
        targetCanvas = canvas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot == null) return;
        if (slot.GetPayload() == null || slot.GetCount() <= 0) return;

        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
        CreatePreview(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdatePreviewPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        DestroyPreview();

        if (slot == null) return;

        IDragSlot destination = GetDestination(eventData);
        if (destination == null || ReferenceEquals(destination, slot)) return;

        TryTransfer(slot, destination);
    }

    private IDragSlot GetDestination(PointerEventData eventData)
    {
        if (eventData.pointerEnter == null) return null;
        return eventData.pointerEnter.GetComponentInParent<IDragSlot>();
    }

    private void TryTransfer(IDragSlot source, IDragSlot destination)
    {
        object payload = source.GetPayload();
        int count = source.GetCount();
        if (payload == null || count <= 0) return;

        object destPayload = destination.GetPayload();
        int destCount = destination.GetCount();

        // 같은 아이템(ItemData)이고 스택 가능하면 병합 시도
        if (payload is ItemData srcItem && destPayload is ItemData destItem)
        {
            bool sameId = destItem.itemId == srcItem.itemId;
            bool stackable = destItem.stackable && srcItem.stackable;
            if (sameId && stackable)
            {
                int acceptableMerge = Mathf.Max(0, destination.MaxAcceptable(payload));
                int toTransferMerge = Mathf.Min(acceptableMerge, count);
                if (toTransferMerge > 0)
                {
                    source.Remove(toTransferMerge);
                    destination.Add(payload, toTransferMerge);
                    return;
                }
            }
        }

        // 스왑 시도 (또는 완전 이동)
        bool canSwap = destination.MaxAcceptable(payload) >= count &&
                       source.MaxAcceptable(destPayload) >= destCount;

        if (canSwap)
        {
            source.Remove(count);
            destination.Remove(destCount);

            destination.Add(payload, count);
            if (destPayload != null && destCount > 0)
                source.Add(destPayload, destCount);
            return;
        }

        // 단순 이동
        int acceptable = destination.MaxAcceptable(payload);
        int toTransfer = Mathf.Min(acceptable, count);
        if (toTransfer <= 0) return;

        source.Remove(toTransfer);
        destination.Add(payload, toTransfer);
    }

    private void CreatePreview(PointerEventData eventData)
    {
        if (targetCanvas == null) return;

        Image src = slot.GetPreviewImage();
        if (src == null || src.sprite == null) return;

        GameObject go = new GameObject("DragPreview");
        go.transform.SetParent(targetCanvas.transform, false);
        previewRect = go.AddComponent<RectTransform>();
        previewRect.localScale = Vector3.one;

        previewImage = go.AddComponent<Image>();
        previewImage.raycastTarget = false;
        previewImage.sprite = src.sprite;
        previewImage.color = src.color;
        previewRect.sizeDelta = src.rectTransform.rect.size;

        previewRect.SetAsLastSibling();
        UpdatePreviewPosition(eventData);
    }

    private void UpdatePreviewPosition(PointerEventData eventData)
    {
        if (previewRect == null || targetCanvas == null) return;

        if (targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            previewRect.position = eventData.position;
        }
        else
        {
            Camera cam = targetCanvas.worldCamera != null ? targetCanvas.worldCamera : Camera.main;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    targetCanvas.transform as RectTransform,
                    eventData.position,
                    cam,
                    out Vector3 worldPoint))
            {
                previewRect.position = worldPoint;
            }
        }
    }

    private void DestroyPreview()
    {
        if (previewRect != null)
        {
            Destroy(previewRect.gameObject);
            previewRect = null;
            previewImage = null;
        }
    }
}
