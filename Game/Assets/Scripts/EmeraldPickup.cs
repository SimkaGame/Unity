using UnityEngine;

public class EmeraldPickup : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource;
    private bool isPicked;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPicked)
        {
            isPicked = true;
            anim.SetTrigger("pickupTrigger");
            audioSource.PlayOneShot(audioSource.clip);
            GameManager.Instance.AddEmerald();
            Destroy(gameObject, 0.5f);
        }
    }
}