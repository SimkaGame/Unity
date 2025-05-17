using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerAudioController>()?.PlayDamageSound();
            other.GetComponent<DamageFlash>()?.PlayFlash();
            other.GetComponent<PlayerHealth>()?.ResetAirTime();

            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Die();
                // Блокируем триггер портала на 5 секунд
                Portal spawnPortal = GameObject.FindGameObjectWithTag("SpawnPortal")?.GetComponent<Portal>();
                if (spawnPortal != null)
                    spawnPortal.StartCoroutine(spawnPortal.BlockTriggerForSpawn());
            }
            else
            {
                StartCoroutine(RespawnPlayer(other));
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }

    private IEnumerator RespawnPlayer(Collider2D player)
    {
        yield return new WaitForSeconds(0.5f);
        if (CheckpointManager.Instance == null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield break;
        }

        string checkpointScene = CheckpointManager.Instance.GetLastCheckpointScene();
        if (checkpointScene == SceneManager.GetActiveScene().name)
        {
            Vector3 checkpoint = CheckpointManager.Instance.GetLastCheckpointPosition();
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (rb)
                rb.linearVelocity = Vector2.zero;

            player.transform.position = checkpoint;
            playerHealth?.ResetHealth();
            if (playerController != null)
                playerController.IsTeleporting = false;

            // Блокируем триггер портала на 5 секунд
            Portal spawnPortal = GameObject.FindGameObjectWithTag("SpawnPortal")?.GetComponent<Portal>();
            if (spawnPortal != null)
                spawnPortal.StartCoroutine(spawnPortal.BlockTriggerForSpawn());
        }
        else
        {
            SceneManager.LoadScene(checkpointScene);
        }
    }
}