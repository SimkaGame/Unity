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
        sourceA.playOnAwake = false;
        sourceB.playOnAwake = false;
        sourceA.loop = false;
        sourceB.loop = false;

        currentSource = sourceA;
        nextSource = sourceB;
    }

    private void Start()
    {
        if (musicTracks.Length == 0) return;

        maxVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SetVolume(maxVolume);
        Invoke(nameof(PlayNextTrack), delayBeforePlay);
    }

    private void PlayNextTrack()
    {
        if (musicTracks.Length == 0 || isFading) return;

        do
        {
            currentIndex = Random.Range(0, musicTracks.Length);
        } while (musicTracks.Length > 1 && currentIndex == previousIndex);

        previousIndex = currentIndex;

        nextSource.clip = musicTracks[currentIndex];
        nextSource.volume = 0f;
        nextSource.Play();

        StartCoroutine(Crossfade(currentSource, nextSource));

        float nextDelay = Mathf.Max(musicTracks[currentIndex].length - fadeTime, musicTracks[currentIndex].length * 0.9f);
        Invoke(nameof(PlayNextTrack), nextDelay);
    }

    private IEnumerator Crossfade(AudioSource from, AudioSource to)
    {
        isFading = true;
        float t = 0f;
        float startVolumeFrom = from.volume;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / fadeTime;
            from.volume = Mathf.Lerp(startVolumeFrom, 0f, normalized);
            to.volume = Mathf.Lerp(0f, maxVolume, normalized);
            yield return null;
        }

        from.Stop();
        from.volume = 0f;
        to.volume = maxVolume;

        (currentSource, nextSource) = (to, from);
        isFading = false;
    }

    public void SetVolume(float volume)
    {
        maxVolume = Mathf.Clamp01(volume);
        if (!isFading) currentSource.volume = maxVolume;
        if (!isFading) nextSource.volume = 0f;
        PlayerPrefs.SetFloat("MusicVolume", maxVolume);
        PlayerPrefs.Save();
    }

    public float GetVolume() => maxVolume;
}