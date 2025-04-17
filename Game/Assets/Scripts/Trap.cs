using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damage = 1;
    public float damageCooldown = 1f;
    private float lastDamageTime;
    private bool isPlayerInTrap = false;
    private GameObject player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrap = true;
            player = other.gameObject;
            ApplyDamage(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrap = false;
            player = null;
        }
    }

    private void Update()
    {
        // Проверка на кулдаун
        if (isPlayerInTrap && Time.time - lastDamageTime >= damageCooldown && player != null)
        {
            ApplyDamage(player.GetComponent<Collider2D>());
        }
    }

    private void ApplyDamage(Collider2D playerCollider)
    {
        PlayerHealth playerHealth = playerCollider.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            lastDamageTime = Time.time;

            DamageFlash flash = playerCollider.GetComponent<DamageFlash>();
            if (flash != null)
            {
                flash.PlayFlash();
            }
        }
    }
}