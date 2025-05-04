using UnityEngine;

public class EnemyAudioController : MonoBehaviour
{
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip burnSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField, Range(0f, 1f)] private float soundVolume = 1f;

    public void PlayDamageSound() => AudioSource.PlayClipAtPoint(damageSound, transform.position, soundVolume);
    public void PlayBurnSound() => AudioSource.PlayClipAtPoint(burnSound, transform.position, soundVolume);
    public void PlayHitSound() => AudioSource.PlayClipAtPoint(hitSound, transform.position, soundVolume);
    public void PlayDeathSound() => AudioSource.PlayClipAtPoint(deathSound, transform.position, soundVolume);
}