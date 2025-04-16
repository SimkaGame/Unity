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
    private bool isFading = false;

    void Awake()
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

    void Start()
    {
        if (musicTracks == null || musicTracks.Length == 0)
        {
            return;
        }

        maxVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SetVolume(maxVolume);
        Invoke(nameof(PlayNextTrack), delayBeforePlay);
    }

    void PlayNextTrack()
    {
        if (musicTracks.Length == 0 || isFading) return;

        int nextIndex;
        do
        {
            nextIndex = Random.Range(0, musicTracks.Length);
        } while (musicTracks.Length > 1 && nextIndex == currentIndex);

        currentIndex = nextIndex;

        if (nextSource != null)
        {
            nextSource.clip = musicTracks[currentIndex];
            nextSource.volume = 0f;
            nextSource.Play();
        }

        StartCoroutine(Crossfade(currentSource, nextSource));

        float trackLength = musicTracks[currentIndex].length;
        float nextDelay = trackLength - fadeTime;
        if (nextDelay < 0)
        {
            nextDelay = trackLength * 0.9f;
        }
        Invoke(nameof(PlayNextTrack), nextDelay);
    }

    IEnumerator Crossfade(AudioSource from, AudioSource to)
    {
        if (from == null || to == null)
        {
            isFading = false;
            yield break;
        }

        isFading = true;
        float t = 0f;
        float startVolumeFrom = from.volume;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / fadeTime;
            if (from != null)
                from.volume = Mathf.Lerp(startVolumeFrom, 0f, normalized);
            if (to != null)
                to.volume = Mathf.Lerp(0f, maxVolume, normalized);
            yield return null;
        }

        if (from != null)
        {
            from.Stop();
            from.volume = 0f;
        }
        if (to != null)
        {
            to.volume = maxVolume;
        }

        var temp = currentSource;
        currentSource = to;
        nextSource = temp;

        isFading = false;
    }

    public void SetVolume(float volume)
    {
        maxVolume = Mathf.Clamp01(volume);
        if (currentSource != null && !isFading)
        {
            currentSource.volume = maxVolume;
        }
        if (nextSource != null && !isFading)
        {
            nextSource.volume = 0f;
        }
        PlayerPrefs.SetFloat("MusicVolume", maxVolume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return maxVolume;
    }
}