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
        if (isPlayerInTrap && Time.time >= lastDamageTime + damageCooldown)
            ApplyDamage(player.GetComponent<Collider2D>());

        if (isEnemyInTrap && Time.time >= lastDamageTime + damageCooldown)
            ApplyDamageToEnemy(enemy.GetComponent<Collider2D>());
    }

    private void ApplyDamage(Collider2D playerCollider)
{
    var playerHealth = playerCollider.GetComponent<PlayerHealth>();
    if (playerHealth.CurrentHealth <= 0) return;

    int predictedHealth = playerHealth.CurrentHealth - damage;
    bool willSurvive = predictedHealth > 0;

    playerHealth.TakeDamage(damage);
    lastDamageTime = Time.time;

    if (willSurvive)
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
        var enemyAI = enemyCollider.GetComponent<EnemyAI>();
        if (enemyAI.CurrentHealth <= 0) return;

        enemyAI.TakeDamage(damage, true);
        lastDamageTime = Time.time;

        if (enemyAI.CurrentHealth > 0)
            enemyCollider.GetComponent<EnemyAudioController>().PlayBurnSound();

        if (enemyAI.CurrentHealth <= 0)
        {
            isEnemyInTrap = false;
            enemy = null;
        }
    }
}