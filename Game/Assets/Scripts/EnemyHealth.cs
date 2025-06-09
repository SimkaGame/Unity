using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 10;
    
    private bool isInTrap;
    private EnemyAudioController audioController;
    private DamageFlash damageFlash;
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public event Action OnDamageTaken;

    private void Start()
    {
        audioController = GetComponent<EnemyAudioController>();
        damageFlash = GetComponent<DamageFlash>();
        currentHealth = health;
    }

    public void TakeDamage(int damage, bool isFromTrap = false, bool isFromArrow = false)
    {
        if (!isFromTrap && isInTrap)
        {
            return;
        }

        currentHealth -= damage;

        if (isFromArrow)
        {
            OnDamageTaken?.Invoke();
        }

        if (currentHealth <= 0)
        {
            audioController?.PlayDeathSound();
            Destroy(gameObject);
        }
        else
        {
            if (isFromTrap)
            {
                audioController?.PlayBurnSound();
            }
            else
            {
                audioController?.PlayHitSound();
            }
            damageFlash?.PlayFlash();
        }
    }

    public void SetIsInTrap(bool value)
    {
        isInTrap = value;
    }
}