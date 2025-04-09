using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] musicTracks;
    private int currentTrackIndex = 0;
    private float delayBeforePlay = 5f;
    private float fadeTime = 2f;
    private float maxVolume = 1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null || musicTracks.Length == 0)
            return;

        audioSource.volume = 0f;
        Invoke("PlayMusic", delayBeforePlay);
    }

    void PlayMusic()
    {
        if (audioSource == null || musicTracks.Length == 0)
            return;

        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.loop = false;
        audioSource.Play();

        StartCoroutine(FadeIn());
        Invoke("PrepareNextTrack", musicTracks[currentTrackIndex].length - fadeTime);
    }

    void PrepareNextTrack()
    {
        StartCoroutine(FadeOutAndSwitch());
    }

    System.Collections.IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, maxVolume, elapsedTime / fadeTime);
            yield return null;
        }
        audioSource.volume = maxVolume;
    }

    System.Collections.IEnumerator FadeOutAndSwitch()
    {
        float elapsedTime = 0f;
        float startVolume = audioSource.volume;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeTime);
            yield return null;
        }
        audioSource.volume = 0f;

        currentTrackIndex = Random.Range(0, musicTracks.Length);
        PlayMusic();
    }
}