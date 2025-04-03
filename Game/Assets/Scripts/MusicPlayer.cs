using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] musicTracks;
    private int currentTrackIndex = 0;
    private float delayBeforePlay = 5f;

    void Start()
    {
        Invoke("PlayMusic", delayBeforePlay);
    }

    void PlayMusic()
    {
        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.loop = false;
        audioSource.Play();

        Invoke("CheckTrackEnd", musicTracks[currentTrackIndex].length);
    }

    void CheckTrackEnd()
    {
        currentTrackIndex = Random.Range(0, musicTracks.Length);
        PlayMusic();
    }
}