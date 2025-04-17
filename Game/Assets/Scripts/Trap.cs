using UnityEngine;

public class Trap : MonoBehaviour
{
    public int damage = 1;
    public float damageCooldown = 1f;
    private float lastDamageTime;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Time.time - lastDamageTime >= damageCooldown)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                lastDamageTime = Time.time;

                // 🔥 Вспышка урона
                DamageFlash flash = other.GetComponent<DamageFlash>();
                if (flash != null)
                {
                    flash.PlayFlash();
                }
            }
        }
    }
}
