using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private AudioClip hitSound;
    [SerializeField, Range(0f, 1f)] private float hitSoundVolume = 0.3f;
    [SerializeField] private int damage = 4;

    private void Update()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject target)
    {
        if (target.CompareTag("Enemy"))
        {
            if (target.TryGetComponent<EnemyAI>(out var enemy))
            {
                enemy.TakeDamage(damage);
                target.GetComponent<EnemyAudioController>()?.PlayHitSound();
            }
            Destroy(gameObject);
            return;
        }

        if (target.layer == LayerMask.NameToLayer("Ground"))
        {
            if (hitSound)
                PlaySound(hitSound, transform.position);
            Destroy(gameObject);
        }
    }

    private void PlaySound(AudioClip clip, Vector3 position)
    {
        GameObject soundObject = new GameObject("ArrowSound");
        soundObject.transform.position = position;
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = hitSoundVolume;
        audioSource.Play();
        Destroy(soundObject, clip.length);
    }
}