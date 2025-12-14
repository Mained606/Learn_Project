using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("기본 스탯")]
    [SerializeField] private int baseMaxHP = 100;
    [SerializeField] private int baseMaxMP = 50;
    [SerializeField] private int baseAttack = 10;
    [SerializeField] private float baseAttackSpeed = 1.0f;
    [SerializeField] private float baseMoveSpeed = 5.0f;
    [SerializeField] private int baseStrength = 10;
    [SerializeField] private int baseAgility = 10;
    [SerializeField] private int baseIntellect = 10;
    [SerializeField] private int baseVitality = 10;

    [Header("현재 HP")]
    [SerializeField] private int currentHP;
    [Header("현재 MP")]
    [SerializeField] private int currentMP;

    [Header("디버그용 최종 스탯(읽기 전용)")]
    [SerializeField] private int debugMaxHP;
    [SerializeField] private int debugMaxMP;
    [SerializeField] private int debugAttack;
    [SerializeField] private float debugAttackSpeed;
    [SerializeField] private float debugMoveSpeed;
    [SerializeField] private int debugStrength;
    [SerializeField] private int debugAgility;
    [SerializeField] private int debugIntellect;
    [SerializeField] private int debugVitality;

    // 보너스 스탯 합산용
    private readonly Dictionary<StatType, int> bonusStats = new Dictionary<StatType, int>();

    public int MaxHP => baseMaxHP + GetBonus(StatType.MaxHP);
    public int MaxMP => baseMaxMP + GetBonus(StatType.MaxMP);
    public int Attack => baseAttack + GetBonus(StatType.Attack);
    public float AttackSpeed => baseAttackSpeed + GetBonus(StatType.AttackSpeed) * 0.01f;
    public float MoveSpeed => baseMoveSpeed + GetBonus(StatType.MoveSpeed) * 0.01f;
    public int Strength => baseStrength + GetBonus(StatType.Strength);
    public int Agility => baseAgility + GetBonus(StatType.Agility);
    public int Intellect => baseIntellect + GetBonus(StatType.Intellect);
    public int Vitality => baseVitality + GetBonus(StatType.Vitality);
    public int CurrentHP => currentHP;
    public int CurrentMP => currentMP;

    private void Awake()
    {
        currentHP = MaxHP;
        currentMP = MaxMP;
        RefreshDebugStats();
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        if (currentHP <= 0)
        {
            currentHP = 0;
            // 사망 처리 이벤트
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        currentHP = Mathf.Clamp(currentHP + amount, 0, MaxHP);
        Debug.Log($"[PlayerStats] HP 회복: +{amount} → {currentHP}/{MaxHP}");
    }

    /// <summary>
    /// 스탯 모디파이어를 적용하거나 해제한다.
    /// </summary>
    public void ApplyModifier(StatsModifier modifier, bool add)
    {
        if (modifier == null) return;
        int delta = add ? modifier.amount : -modifier.amount;
        AddBonus(modifier.statType, delta);
        ClampHealth();
    }

    /// <summary>
    /// 여러 모디파이어를 일괄 적용/해제한다.
    /// </summary>
    public void ApplyModifiers(IEnumerable<StatsModifier> modifiers, bool add)
    {
        if (modifiers == null) return;
        foreach (StatsModifier mod in modifiers)
        {
            ApplyModifier(mod, add);
        }
        RefreshDebugStats();
    }

    private void AddBonus(StatType statType, int delta)
    {
        if (!bonusStats.ContainsKey(statType))
        {
            bonusStats[statType] = 0;
        }

        bonusStats[statType] += delta;
    }

    private int GetBonus(StatType statType)
    {
        return bonusStats.TryGetValue(statType, out int value) ? value : 0;
    }

    private void ClampHealth()
    {
        // 최대 체력이 내려가도 현재 체력이 초과하지 않도록 보정
        currentHP = Mathf.Clamp(currentHP, 0, MaxHP);
        currentMP = Mathf.Clamp(currentMP, 0, MaxMP);
        RefreshDebugStats();
    }

    // 인스펙터에서 최종 스탯을 확인할 수 있도록 디버그 필드를 갱신
    private void RefreshDebugStats()
    {
        debugMaxHP = MaxHP;
        debugMaxMP = MaxMP;
        debugAttack = Attack;
        debugAttackSpeed = AttackSpeed;
        debugMoveSpeed = MoveSpeed;
        debugStrength = Strength;
        debugAgility = Agility;
        debugIntellect = Intellect;
        debugVitality = Vitality;
    }
}
