using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 아이템 상세 정보 패널. 좌클릭 시 표시.
/// </summary>
public class InventoryDetailPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI quantityText;

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void Show(ItemData item, ItemDefinition definition)
    {
        if (item == null || panel == null) return;

        if (icon != null)
        {
            icon.sprite = definition != null ? definition.Icon : null;
            icon.enabled = icon.sprite != null;
        }
        if (nameText != null) nameText.text = item.displayName;
        if (typeText != null) typeText.text = item.itemType.ToString();
        if (descText != null) descText.text = item.description;
        if (quantityText != null)
        {
            quantityText.text = item.stackable ? $"x{item.quantity}" : "x1";
        }

        panel.SetActive(true);
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
