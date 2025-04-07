using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Animator animator;
    public GameObject checkpointTextPrefab; // Ссылка на префаб текста TextMeshPro
    private bool isActive = false;

    private void Start()
    {
        if (animator != null)
        {
            animator.Play("CheckpointIdle");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            ActivateCheckpoint(other.transform);
        }
    }

    private void ActivateCheckpoint(Transform playerTransform)
    {
        isActive = true;

        CheckpointManager.Instance.SetCheckpoint(transform.position);

        // Создаем текст над персонажем
        if (checkpointTextPrefab != null)
        {
            Vector3 spawnPosition = playerTransform.position + Vector3.up * 1.5f; // Позиция над персонажем
            Instantiate(checkpointTextPrefab, spawnPosition, Quaternion.identity);
        }

        gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        isActive = false;
    }
}