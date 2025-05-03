using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerAudioController>()?.PlayDamageSound();
            other.GetComponent<DamageFlash>()?.PlayFlash();
            other.GetComponent<PlayerHealth>()?.ResetAirTime();
            StartCoroutine(RespawnPlayer(other));
        }
        else if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }

    private IEnumerator RespawnPlayer(Collider2D player)
    {
        yield return new WaitForSeconds(0.2f);

        Vector3 checkpoint = CheckpointManager.Instance.GetLastCheckpointPosition();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        
        if (rb)
            rb.linearVelocity = Vector2.zero;
        
        player.transform.position = checkpoint;
        player.GetComponent<PlayerHealth>()?.ResetHealth();
    }
}