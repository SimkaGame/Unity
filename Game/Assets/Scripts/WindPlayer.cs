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

    private void Awake()
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

    private void Start()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        currentSource.volume = sfxVolume * maxVolume;
        nextSource.volume = 0f;
        currentSource.Play();
        Invoke(nameof(PlayNextCycle), windClip.length);
    }

    private void PlayNextCycle()
    {
        nextSource.volume = 0f;
        nextSource.Play();
        StartCoroutine(Crossfade());
        Invoke(nameof(PlayNextCycle), windClip.length);
    }

    private IEnumerator Crossfade()
    {
        float t = 0f;
        float startVolumeFrom = currentSource.volume;
        float targetVolume = PlayerPrefs.GetFloat("SFXVolume", 1f) * maxVolume;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / fadeTime;
            currentSource.volume = Mathf.Lerp(startVolumeFrom, 0f, normalized);
            nextSource.volume = Mathf.Lerp(0f, targetVolume, normalized);
            yield return null;
        }

        currentSource.Stop();
        currentSource.volume = 0f;
        nextSource.volume = targetVolume;

        (currentSource, nextSource) = (nextSource, currentSource);
    }
}