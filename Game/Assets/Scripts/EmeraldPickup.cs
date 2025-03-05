using UnityEngine;

public class EmeraldPickup : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource;
    private bool isPicked = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TriggerPickupAnimation()
    {
        if (!isPicked)
        {
            isPicked = true;
            Debug.Log("Анимация подбора изумруда запускается!");

            anim.SetTrigger("pickupTrigger");

            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
                Debug.Log("Звук подбора воспроизведен!");
            }
            else
            {
                Debug.LogWarning("AudioSource или AudioClip не установлены!");
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
            TriggerPickupAnimation();
        }
    }
}
