using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerAudioController audioController = other.GetComponent<PlayerAudioController>();
        if (audioController != null)
        {
            audioController.PlayDamageSound();
        }

        DamageFlash flash = other.GetComponent<DamageFlash>();
        if (flash != null)
        {
            flash.PlayFlash();
        }

        StartCoroutine(RespawnPlayer(other));
    }

    private IEnumerator RespawnPlayer(Collider2D player)
    {
        yield return new WaitForSeconds(0.2f);

        if (CheckpointManager.Instance != null)
        {
            Vector3 checkpoint = CheckpointManager.Instance.GetLastCheckpointPosition();

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                player.transform.position = checkpoint;
            }
            else
            {
                player.transform.position = checkpoint;
            }
        }

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.ResetHealth();
        }
    }
}