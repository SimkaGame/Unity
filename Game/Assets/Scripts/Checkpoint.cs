using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject checkpointTextPrefab;
    private bool isActive;

    private void Start()
    {
        animator?.Play("CheckpointIdle");

        if (CheckpointManager.Instance != null && CheckpointManager.Instance.IsCheckpointActivated(transform.position))
        {
            isActive = true;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
            ActivateCheckpoint(other.transform);
    }

    private void ActivateCheckpoint(Transform playerTransform)
    {
        isActive = true;
        CheckpointManager.Instance.SetCheckpoint(transform.position, GetComponent<AudioSource>(), gameObject.scene.name);

        if (checkpointTextPrefab)
            Instantiate(checkpointTextPrefab, playerTransform.position + Vector3.up * 1.5f, Quaternion.identity);

        gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(true);
    }
}
