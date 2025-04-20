using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damage = 2;
    public float damageCooldown = 0.5f;
    private float lastDamageTime;
    private bool isPlayerInTrap = false;
    private GameObject player;
    private int soundPlayCount = 0;
    private const int maxSoundPlays = 4;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrap = true;
            player = other.gameObject;
            soundPlayCount = 0;
            ApplyDamage(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrap = false;
            player = null;
            soundPlayCount = 0;
        }
    }

    private void Update()
    {
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

            if (soundPlayCount < maxSoundPlays)
            {
                PlayerAudioController audioController = playerCollider.GetComponent<PlayerAudioController>();
                if (audioController != null)
                {
                    audioController.PlayDamageSound();
                    audioController.PlayBurnSound();
                    soundPlayCount++;
                }
            }

            DamageFlash flash = playerCollider.GetComponent<DamageFlash>();
            if (flash != null)
            {
                flash.PlayFlash();
            }
        }
    }
}