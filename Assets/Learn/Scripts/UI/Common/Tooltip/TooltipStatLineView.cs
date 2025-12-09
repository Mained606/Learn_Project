using TMPro;
using UnityEngine;

/// <summary>
/// 툴팁 추가 정보 한 줄 표시 뷰.
/// </summary>
public class TooltipStatLineView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI valueText;

    public void Bind(TooltipStatLine line)
    {
        if (labelText != null) labelText.text = line.label;
        if (valueText != null) valueText.text = line.value;
    }
}
