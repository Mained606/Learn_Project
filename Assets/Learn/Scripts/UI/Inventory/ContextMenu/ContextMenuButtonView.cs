using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 컨텍스트 메뉴 버튼 프리팹 뷰.
/// </summary>
public class ContextMenuButtonView : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI labelText;

    public void Bind(ContextMenuAction action)
    {
        if (labelText != null) labelText.text = action.label;
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            if (action.callback != null)
                button.onClick.AddListener(() => action.callback.Invoke());
        }
    }
}
