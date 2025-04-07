using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] musicTracks;
    private int currentTrackIndex = 0;
    private float delayBeforePlay = 5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Получаем компонент AudioSource
        if (audioSource == null)
        {
            Debug.LogError("AudioSource не найден! Добавьте компонент AudioSource на объект с этим скриптом.");
            return;
        }

        if (musicTracks.Length == 0)
        {
            Debug.LogError("Не добавлены аудиотреки в MusicPlayer!");
            return;
        }

        Invoke("PlayMusic", delayBeforePlay);
    }

    void PlayMusic()
    {
        if (audioSource == null || musicTracks.Length == 0)
            return;

        audioSource.clip = musicTracks[currentTrackIndex];
        audioSource.loop = false;
        audioSource.Play();

        Invoke("CheckTrackEnd", musicTracks[currentTrackIndex].length);
    }

    void CheckTrackEnd()
    {
        if (musicTracks.Length == 0)
            return;

        currentTrackIndex = Random.Range(0, musicTracks.Length);
        PlayMusic();
    }
}