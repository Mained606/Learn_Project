using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 범용 툴팁 표시자. TooltipData를 받아 UI에 바인딩한다.
/// </summary>
public class TooltipPresenter : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private RectTransform statContainer;
    [SerializeField] private TooltipStatLineView statLinePrefab;

    [Header("표시 옵션")]
    [SerializeField] private Vector2 offset = new Vector2(16f, -16f);
    [SerializeField] private Vector2 padding = new Vector2(10f, 10f);
    [SerializeField] private float hoverDelay = 0.7f;
    [SerializeField] private bool clampToScreen = false;

    private readonly List<TooltipStatLineView> statLinePool = new();

    public float HoverDelay => hoverDelay;

    private void Awake()
    {
        if (panel != null)
        {
            CanvasGroup cg = panel.GetComponent<CanvasGroup>();
            if (cg == null) cg = panel.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;
            cg.interactable = false;

            foreach (var g in panel.GetComponentsInChildren<Graphic>(true))
            {
                g.raycastTarget = false;
            }
        }
    }

    public void Show(TooltipData data, Vector3 position)
    {
        if (panel == null || data == null) return;

        if (iconImage != null)
        {
            iconImage.sprite = data.icon;
            iconImage.enabled = data.icon != null;
        }
        if (titleText != null) titleText.text = data.title;
        if (subtitleText != null) subtitleText.text = data.subtitle;
        if (descriptionText != null) descriptionText.text = data.description;

        BindStatLines(data.statLines);

        panel.transform.position = AdjustPosition(position);
        panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void BindStatLines(List<TooltipStatLine> lines)
    {
        if (statContainer == null || statLinePrefab == null) return;

        int needed = lines != null ? lines.Count : 0;
        // 확장
        while (statLinePool.Count < needed)
        {
            TooltipStatLineView view = Instantiate(statLinePrefab, statContainer);
            statLinePool.Add(view);
        }

        for (int i = 0; i < statLinePool.Count; i++)
        {
            bool active = i < needed;
            statLinePool[i].gameObject.SetActive(active);
            if (active)
            {
                statLinePool[i].Bind(lines[i]);
            }
        }
    }

    private Vector3 AdjustPosition(Vector3 position)
    {
        Vector3 targetPos = position + (Vector3)offset;
        if (!clampToScreen) return targetPos;

        RectTransform rect = panel.transform as RectTransform;
        Canvas canvas = panel.GetComponentInParent<Canvas>();
        Vector2 size = rect != null ? rect.rect.size : Vector2.zero;
        float scale = canvas != null ? canvas.scaleFactor : 1f;
        Vector2 scaled = size * scale;

        float screenW = Screen.width;
        float screenH = Screen.height;

        targetPos.x = Mathf.Clamp(targetPos.x, padding.x, screenW - scaled.x - padding.x);
        targetPos.y = Mathf.Clamp(targetPos.y, padding.y + scaled.y, screenH - padding.y);

        if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
        {
            Camera cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, targetPos, cam, out Vector3 worldPoint))
                return worldPoint;
        }

        return targetPos;
    }
}
