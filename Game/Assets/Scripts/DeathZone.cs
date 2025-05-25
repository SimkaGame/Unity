using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController)
            {
                playerController.Die();
            }
            else
            {
                RespawnPlayer(other);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }

    private void RespawnPlayer(Collider2D player)
    {
        if (!CheckpointManager.Instance)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        string checkpointScene = CheckpointManager.Instance.GetLastCheckpointScene();
        if (checkpointScene == SceneManager.GetActiveScene().name)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.Die();
        }
        else
        {
            SceneManager.LoadScene(checkpointScene);
        }
    }
}