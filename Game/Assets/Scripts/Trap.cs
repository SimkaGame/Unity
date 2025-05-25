using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{
    [SerializeField] private int damage = 2;
    [SerializeField] private float damageCooldown = 0.5f;

    private float lastDamageTime;
    private bool isPlayerInTrap;
    private GameObject player;
    private bool isEnemyInTrap;
    private GameObject enemy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrap = true;
            player = other.gameObject;
        }
        else if (other.CompareTag("Enemy"))
        {
            isEnemyInTrap = true;
            enemy = other.gameObject;
            enemy.GetComponent<EnemyAI>().SetIsInTrap(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrap = false;
            player = null;
        }
        else if (other.CompareTag("Enemy"))
        {
            isEnemyInTrap = false;
            enemy.GetComponent<EnemyAI>().SetIsInTrap(false);
            enemy = null;
        }
    }

    private void Update()
    {
        if (isPlayerInTrap && player != null && Time.time >= lastDamageTime + damageCooldown)
            ApplyDamage(player.GetComponent<Collider2D>());

        if (isEnemyInTrap && enemy != null && Time.time >= lastDamageTime + damageCooldown)
            ApplyDamageToEnemy(enemy.GetComponent<Collider2D>());
    }

    private void ApplyDamage(Collider2D playerCollider)
    {
        if (playerCollider == null)
        {
            isPlayerInTrap = false;
            player = null;
            return;
        }

        var playerHealth = playerCollider.GetComponent<PlayerHealth>();
        if (playerHealth == null || playerHealth.CurrentHealth <= 0)
        {
            isPlayerInTrap = false;
            player = null;
            Debug.Log("[Trap] ApplyDamage: Игрок мертв или PlayerHealth отсутствует, урон не наносится");
            return;
        }

        bool isLethal = playerHealth.CurrentHealth <= damage;

        playerHealth.TakeDamage(damage);
        lastDamageTime = Time.time;

        if (!isLethal)
        {
            var audioController = playerCollider.GetComponent<PlayerAudioController>();
            audioController.PlayDamageSound();
            audioController.PlayBurnSound();
            playerCollider.GetComponent<DamageFlash>().PlayFlash();
        }

        if (playerHealth.CurrentHealth <= 0)
        {
            isPlayerInTrap = false;
            player = null;
        }
    }

    private void ApplyDamageToEnemy(Collider2D enemyCollider)
    {
        if (enemyCollider == null)
        {
            isEnemyInTrap = false;
            enemy = null;
            return;
        }

        var enemyAI = enemyCollider.GetComponent<EnemyAI>();
        if (enemyAI.CurrentHealth <= 0) return;

        bool isLethal = enemyAI.CurrentHealth <= damage;

        enemyAI.TakeDamage(damage, true);
        lastDamageTime = Time.time;

        if (!isLethal && enemyAI.CurrentHealth > 0)
            enemyCollider.GetComponent<EnemyAudioController>().PlayBurnSound();

        if (enemyAI.CurrentHealth <= 0)
        {
            isEnemyInTrap = false;
            enemy = null;
        }
    }
}