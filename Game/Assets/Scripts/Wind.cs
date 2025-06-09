using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour
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
        sourceA.playOnAwake = sourceB.playOnAwake = false;
        sourceA.loop = sourceB.loop = false;
        sourceA.clip = sourceB.clip = windClip;

        currentSource = sourceA;
        nextSource = sourceB;
    }

    private void Start()
    {
        UpdateVolume();
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
        float startVolume = currentSource.volume;
        float targetVolume = PlayerPrefs.GetFloat("SFXVolume", 1f) * maxVolume;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / fadeTime;
            currentSource.volume = Mathf.Lerp(startVolume, 0f, normalized);
            nextSource.volume = Mathf.Lerp(0f, targetVolume, normalized);
            yield return null;
        }

        currentSource.Stop();
        currentSource.volume = 0f;
        nextSource.volume = targetVolume;

        (currentSource, nextSource) = (nextSource, currentSource);
    }

    public void UpdateVolume()
    {
        currentSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f) * maxVolume;
        nextSource.volume = 0f;
    }
}