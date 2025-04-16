using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    private MusicPlayer musicPlayer;
    private AudioSource[] sfxSources;
    private bool isPaused = false;
    private PlayerShooting playerShooting;

    void Start()
    {
        musicPlayer = FindFirstObjectByType<MusicPlayer>();
        playerShooting = FindFirstObjectByType<PlayerShooting>();

        sfxSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        if (musicPlayer != null)
        {
            AudioSource[] musicSources = musicPlayer.GetComponents<AudioSource>();
            System.Collections.Generic.List<AudioSource> filteredSources = new System.Collections.Generic.List<AudioSource>();
            foreach (var source in sfxSources)
            {
                bool isMusicSource = false;
                foreach (var musicSource in musicSources)
                {
                    if (source == musicSource)
                    {
                        isMusicSource = true;
                        break;
                    }
                }
                if (!isMusicSource)
                {
                    filteredSources.Add(source);
                }
            }
            sfxSources = filteredSources.ToArray();
        }

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;
            float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            masterVolumeSlider.value = sfxVolume;
            masterVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
            SetSFXVolume(sfxVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;
            float musicVolume = musicPlayer != null ? musicPlayer.GetVolume() : PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicVolumeSlider.value = musicVolume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
            SetMusicVolume(musicVolume);
        }

        menuPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ContinueGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }
        if (musicVolumeSlider != null && musicPlayer != null)
        {
            musicVolumeSlider.value = musicPlayer.GetVolume();
        }

        if (playerShooting != null)
        {
            playerShooting.OnPause();
        }
    }

    public void ContinueGame()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (playerShooting != null)
        {
            playerShooting.OnResume();
        }
    }

    public static bool IsPaused()
    {
        return FindFirstObjectByType<PauseMenu>()?.isPaused ?? false;
    }

    public void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void SetSFXVolume(float volume)
    {
        foreach (var source in sfxSources)
        {
            if (source != null)
            {
                source.volume = volume;
            }
        }
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    void SetMusicVolume(float volume)
    {
        if (musicPlayer != null)
        {
            musicPlayer.SetVolume(volume);
        }
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}