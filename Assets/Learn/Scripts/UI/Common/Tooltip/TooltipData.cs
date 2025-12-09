using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 툴팁에 표시할 공통 데이터.
/// </summary>
public class TooltipData
{
    public string title;
    public string subtitle;
    public string description;
    public Sprite icon;
    public List<TooltipStatLine> statLines;
    
    // 생성자
    public TooltipData(string title = "", string subtitle = "", string description = "", Sprite icon = null, List<TooltipStatLine> statLines = null)
    {
        this.title = title;
        this.subtitle = subtitle;
        this.description = description;
        this.icon = icon;
        this.statLines = statLines ?? new List<TooltipStatLine>();
    }
}
