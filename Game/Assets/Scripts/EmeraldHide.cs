using UnityEngine;

public class EmeraldHide : MonoBehaviour
{
    private AudioSource audioSource;
    private bool isPicked = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void TriggerPickup()
    {
        if (!isPicked)
        {
            isPicked = true;

            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }

            Invoke("DestroySelf", 0.5f);
        }
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPicked)
        {
            TriggerPickup();
        }
    }
}
