using UnityEngine;

public class EnemyAudioController : MonoBehaviour
{
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip burnSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float soundVolume = 1f;
    [SerializeField] private float deathSoundVolume = 1f;

    public void PlayDamageSound()
    {
        if (damageSound != null)
        {
            float volume = Mathf.Clamp01(soundVolume) * PlayerPrefs.GetFloat("SFXVolume", 1f);
            AudioSource.PlayClipAtPoint(damageSound, transform.position, volume);
        }
    }

    public void PlayBurnSound()
    {
        if (burnSound != null)
        {
            float volume = Mathf.Clamp01(soundVolume) * PlayerPrefs.GetFloat("SFXVolume", 1f);
            AudioSource.PlayClipAtPoint(burnSound, transform.position, volume);
        }
    }

    public void PlayHitSound()
    {
        if (hitSound != null)
        {
            float volume = Mathf.Clamp01(soundVolume) * PlayerPrefs.GetFloat("SFXVolume", 1f);
            AudioSource.PlayClipAtPoint(hitSound, transform.position, volume);
        }
    }

    public void PlayDeathSound()
    {
        if (deathSound != null)
        {
            float volume = Mathf.Clamp01(deathSoundVolume) * PlayerPrefs.GetFloat("SFXVolume", 1f);
            GameObject tempAudio = new GameObject("TempAudio");
            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f;
            audioSource.PlayOneShot(deathSound, volume);
            Destroy(tempAudio, deathSound.length);
        }
    }
}