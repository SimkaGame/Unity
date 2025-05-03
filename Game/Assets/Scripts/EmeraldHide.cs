using UnityEngine;

public class EmeraldHide : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private bool isPicked;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void TriggerPickup()
    {
        if (isPicked) return;

        isPicked = true;
        if (audioSource?.clip)
            audioSource.PlayOneShot(audioSource.clip);

        GameManager.Instance.AddEmerald();
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPicked)
            TriggerPickup();
    }
}