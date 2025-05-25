using UnityEngine;

public class Emerald : MonoBehaviour
{
    [SerializeField] private bool useAnimation = false;
    [SerializeField] private string emeraldId;

    private Animator anim;
    private AudioSource audioSource;
    private bool isPicked;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();

        if (string.IsNullOrEmpty(emeraldId))
            emeraldId = $"{gameObject.name}_{gameObject.scene.name}";

        if (GameManager.Instance != null && GameManager.Instance.IsEmeraldPicked(emeraldId))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPicked)
            TriggerPickup();
    }

    private void TriggerPickup()
    {
        isPicked = true;

        if (useAnimation)
        {
            if (anim != null)
                anim.SetTrigger("pickupTrigger");
            if (audioSource != null && audioSource.clip != null)
                audioSource.PlayOneShot(audioSource.clip,5f);
            Destroy(gameObject, 0.5f);
        }
        else
        {
            if (audioSource != null && audioSource.clip != null)
                audioSource.PlayOneShot(audioSource.clip,5f);
            Destroy(gameObject, 0.5f);
        }

        GameManager.Instance?.AddEmerald(emeraldId);
    }
}
