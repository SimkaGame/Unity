using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private AudioClip hitSound;

    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hitSound != null)
        {
            PlaySound(hitSound, transform.position);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
    }

    private void PlaySound(AudioClip clip, Vector3 position)
    {
        GameObject soundObject = new GameObject("ArrowSound");
        soundObject.transform.position = position;
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialBlend = 0f;
        audioSource.Play();
        Destroy(soundObject, clip.length);
    }
}