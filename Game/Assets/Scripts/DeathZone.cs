using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            Animator playerAnimator = other.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("IsDead");
            }
            StartCoroutine(RespawnPlayerWithDelay(other));
        }
    }

    private System.Collections.IEnumerator RespawnPlayerWithDelay(Collider2D player)
    {
        yield return new WaitForSeconds(0.2f);
        if (CheckpointManager.Instance != null)
        {
            Vector3 respawnPosition = CheckpointManager.Instance.GetLastCheckpointPosition();
            player.transform.position = respawnPosition;
        }
    }
}
