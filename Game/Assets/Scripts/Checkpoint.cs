using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Animator animator;
    private bool isActive = false;
    
    private void Start()
{

}


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            ActivateCheckpoint();
        }
    }

    private void ActivateCheckpoint()
    {
        isActive = true;

        if (animator != null)
        {
            animator.SetBool("IsActive", true);
        }

        CheckpointManager.Instance.SetCheckpoint(transform.position);

        gameObject.SetActive(false);
    }

    public void Deactivate()
    {
        isActive = false;
        if (animator != null)
        {
            animator.SetBool("IsActive", false);
        }
    }
}
