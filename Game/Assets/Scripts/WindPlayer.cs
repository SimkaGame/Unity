using UnityEngine;
using System.Collections;

public class WindPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip windClip;
    [SerializeField] private float maxVolume = 1f;
    [SerializeField] private float fadeTime = 1f;

    private AudioSource sourceA;
    private AudioSource sourceB;
    private AudioSource currentSource;
    private AudioSource nextSource;
    private bool isFading = false;

    void Awake()
    {
        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();

        sourceA.playOnAwake = false;
        sourceB.playOnAwake = false;

        sourceA.loop = false;
        sourceB.loop = false;

        sourceA.clip = windClip;
        sourceB.clip = windClip;

        currentSource = sourceA;
        nextSource = sourceB;
    }

    void Start()
    {
        if (windClip == null) return;

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        currentSource.volume = sfxVolume * maxVolume;
        nextSource.volume = 0f;
        currentSource.Play();

        float clipLength = windClip.length;
        float nextDelay = Mathf.Max(clipLength - fadeTime - 0.5f, 0f);
        Invoke(nameof(PlayNextCycle), nextDelay);
    }

    void PlayNextCycle()
    {
        if (windClip == null || isFading) return;

        nextSource.volume = 0f;
        nextSource.Play();

        StartCoroutine(Crossfade(currentSource, nextSource));

        float clipLength = windClip.length;
        float nextDelay = Mathf.Max(clipLength - fadeTime - 0.5f, 0f);
        Invoke(nameof(PlayNextCycle), nextDelay);
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
        float targetVolume = PlayerPrefs.GetFloat("SFXVolume", 1f) * maxVolume;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / fadeTime;

            if (from != null) from.volume = Mathf.Lerp(startVolumeFrom, 0f, normalized);
            if (to != null) to.volume = Mathf.Lerp(0f, targetVolume, normalized);

            yield return null;
        }

        if (from != null)
        {
            from.Stop();
            from.volume = 0f;
        }
        if (to != null)
        {
            to.volume = targetVolume;
        }

        var temp = currentSource;
        currentSource = to;
        nextSource = temp;

        isFading = false;
    }
}
