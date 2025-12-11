using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    public int attack = 10;

    private void Awake()
    {
        currentHP = maxHP;
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
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        Debug.Log($"[PlayerStats] HP 회복: +{amount} → {currentHP}/{maxHP}");
    }
}
