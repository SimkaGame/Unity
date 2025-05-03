using UnityEngine;

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
            enemy = null;
        }
    }

    private void Update()
    {
        if (isPlayerInTrap && Time.time - lastDamageTime >= damageCooldown)
            ApplyDamage(player.GetComponent<Collider2D>());

        if (isEnemyInTrap && Time.time - lastDamageTime >= damageCooldown)
            ApplyDamageToEnemy(enemy.GetComponent<Collider2D>());
    }

    private void ApplyDamage(Collider2D playerCollider)
    {
        PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
        int previousHealth = playerHealth.CurrentHealth;
        if (previousHealth <= 0) return;

        playerHealth.TakeDamage(damage);
        lastDamageTime = Time.time;

        if (previousHealth > 2)
        {
            PlayerAudioController audioController = playerCollider.GetComponent<PlayerAudioController>();
            audioController.PlayDamageSound();
            audioController.PlayBurnSound();

            DamageFlash flash = playerCollider.GetComponent<DamageFlash>();
            flash.PlayFlash();
        }

        if (playerHealth.CurrentHealth <= 0)
        {
            isPlayerInTrap = false;
            player = null;
        }
    }

    private void ApplyDamageToEnemy(Collider2D enemyCollider)
    {
        EnemyAI enemyAI = enemyCollider.GetComponent<EnemyAI>();
        int previousHealth = enemyAI.CurrentHealth;
        if (previousHealth <= 0) return;

        enemyAI.TakeDamage(damage);
        lastDamageTime = Time.time;

        if (previousHealth > 2)
        {
            EnemyAudioController audioController = enemyCollider.GetComponent<EnemyAudioController>();
            audioController.PlayDamageSound();
            audioController.PlayBurnSound();

            DamageFlash flash = enemyCollider.GetComponent<DamageFlash>();
            flash.PlayFlash();
        }

        if (enemyAI.CurrentHealth <= 0)
        {
            isEnemyInTrap = false;
            enemy = null;
        }
    }
}