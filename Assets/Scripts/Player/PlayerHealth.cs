
using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;


    public int CurrentHp { get; private set; }
    public int MaxHp => maxHp;


    public event Action<int, int> OnHpChanged;
    public event Action OnDamaged;
    public event Action OnDead;


    private void OnEnable()
    {
        CurrentHp = maxHp;

        OnHpChanged?.Invoke(CurrentHp, maxHp);
    }


    public void TakeDamage(int damage)
    {
        if (damage <= 0 || CurrentHp <= 0)
        {
            return;
        }

        CurrentHp = Mathf.Max(CurrentHp - damage, 0);

        OnHpChanged?.Invoke(CurrentHp, maxHp);
        OnDamaged?.Invoke();

        if (CurrentHp <= 0)
        {
            Die();
        }
    }


    public void Heal(int amount)
    {
        if (amount <= 0 || CurrentHp <= 0)
        {
            return;
        }

        CurrentHp = Mathf.Min(CurrentHp + amount, maxHp);

        OnHpChanged?.Invoke(CurrentHp, maxHp);
    }


    private void Die()
    {
        OnDead?.Invoke();
    }
}
