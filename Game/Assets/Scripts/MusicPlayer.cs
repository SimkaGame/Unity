using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] musicTracks;
    [SerializeField] private float fadeTime = 2f;
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float delayBeforePlay = 5f;

    private AudioSource sourceA;
    private AudioSource sourceB;
    private AudioSource currentSource;
    private AudioSource nextSource;
    private int currentIndex = -1;
    private int previousIndex = -1;
    private bool isFading;

    private void Awake()
    {
        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();
        sourceA.playOnAwake = sourceB.playOnAwake = false;
        sourceA.loop = sourceB.loop = false;

        currentSource = sourceA;
        nextSource = sourceB;
    }

    private void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu" || musicTracks.Length == 0)
            return;

        maxVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SetVolume(maxVolume);
        Invoke(nameof(PlayNextTrack), delayBeforePlay);
    }

    private void PlayNextTrack()
    {
        if (isFading || musicTracks.Length == 0)
            return;

        currentIndex = Random.Range(0, musicTracks.Length);
        currentIndex = musicTracks.Length > 1 && currentIndex == previousIndex ? (currentIndex + 1) % musicTracks.Length : currentIndex;
        previousIndex = currentIndex;

        if (musicTracks[currentIndex] == null)
            return;

        nextSource.clip = musicTracks[currentIndex];
        nextSource.volume = 0f;
        nextSource.Play();

        StartCoroutine(Crossfade());
        Invoke(nameof(PlayNextTrack), Mathf.Max(musicTracks[currentIndex].length - fadeTime, musicTracks[currentIndex].length * 0.9f));
    }

    private IEnumerator Crossfade()
    {
        isFading = true;
        float t = 0f;
        float startVolume = currentSource.volume;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / fadeTime;
            currentSource.volume = Mathf.Lerp(startVolume, 0f, normalized);
            nextSource.volume = Mathf.Lerp(0f, maxVolume, normalized);
            yield return null;
        }

        currentSource.Stop();
        currentSource.volume = 0f;
        nextSource.volume = maxVolume;

        (currentSource, nextSource) = (nextSource, currentSource);
        isFading = false;
    }

    public void SetVolume(float volume)
    {
        maxVolume = Mathf.Clamp01(volume);
        if (!isFading)
        {
            currentSource.volume = maxVolume;
            nextSource.volume = 0f;
        }
        PlayerPrefs.SetFloat("MusicVolume", maxVolume);
        PlayerPrefs.Save();
    }

    public float GetVolume() => maxVolume;
}