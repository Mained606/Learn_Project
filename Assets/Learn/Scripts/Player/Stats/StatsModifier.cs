using System;

/// <summary>
/// 특정 스탯에 대한 가감 수치.
/// </summary>
[Serializable]
public class StatsModifier
{
    public StatType statType;
    public int amount;
}
