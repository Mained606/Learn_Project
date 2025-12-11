using System;

/// <summary>
/// 컨텍스트 메뉴에서 보여줄 액션 정보.
/// </summary>
public class ContextMenuAction
{
    public string label;
    public Action callback;

    public ContextMenuAction(string label, Action callback)
    {
        this.label = label;
        this.callback = callback;
    }
}
