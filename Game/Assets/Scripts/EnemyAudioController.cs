using UnityEngine;

public class EnemyAudioController : MonoBehaviour
{
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip burnSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField, Range(0f, 1f)] private float soundVolume = 1f;

    private void PlaySound(AudioClip clip, Vector3 position)
    {
        if (!clip) return;

        AudioSource.PlayClipAtPoint(clip, position, soundVolume);
    }

    public void PlayDamageSound() => PlaySound(damageSound, transform.position);
    public void PlayBurnSound() => PlaySound(burnSound, transform.position);
    public void PlayHitSound() => PlaySound(hitSound, transform.position);
    public void PlayDeathSound() => PlaySound(deathSound, transform.position);
}