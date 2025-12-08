using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 아이템 툴팁 표시(이름/설명/타입).
/// </summary>
public class InventoryTooltip : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Vector2 offset = new Vector2(170f, -170f);
    [SerializeField] private Vector2 padding = new Vector2(10f, 10f);
    [SerializeField] private float hoverDelay = 0.7f;

    private void Awake()
    {
        if (panel != null)
        {
            // 툴팁이 포인터 이벤트를 가로채지 않도록 CanvasGroup 보장
            CanvasGroup cg = panel.GetComponent<CanvasGroup>();
            if (cg == null)
                cg = panel.AddComponent<CanvasGroup>();

            cg.blocksRaycasts = false;
            cg.interactable = false;

            // 모든 그래픽의 RaycastTarget 비활성화
            var graphics = panel.GetComponentsInChildren<Graphic>(true);
            foreach (var g in graphics)
            {
                g.raycastTarget = false;
            }
        }
    }

    public float HoverDelay => hoverDelay;

    public void Show(ItemData item, ItemDefinition definition, Vector3 position)
    {
        if (panel == null || item == null) return;

        if (iconImage != null)
        {
            iconImage.sprite = definition != null ? definition.Icon : null;
            iconImage.enabled = iconImage.sprite != null;
        }
        if (nameText != null) nameText.text = item.displayName;
        if (typeText != null) typeText.text = item.itemType.ToString();
        if (descText != null) descText.text = item.description;

        panel.transform.position = AdjustToScreen(position);
        panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private Vector3 AdjustToScreen(Vector3 position)
    {
        Vector3 targetPos = position + (Vector3)offset;

        // 화면 클램프 제거: 마우스 위치 + 오프셋만 사용
        return targetPos;
    }
}
