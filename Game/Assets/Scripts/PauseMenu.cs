using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;

    private MusicPlayer musicPlayer;
    private PlayerShooting playerShooting;
    private bool isPaused;

    private void Start()
    {
        musicPlayer = FindFirstObjectByType<MusicPlayer>();
        playerShooting = FindFirstObjectByType<PlayerShooting>();
        SetupSliders();
        menuPanel.SetActive(false);
    }

    private void SetupSliders()
    {
        masterVolumeSlider.minValue = musicVolumeSlider.minValue = 0f;
        masterVolumeSlider.maxValue = musicVolumeSlider.maxValue = 1f;

        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        masterVolumeSlider.value = sfxVolume;
        masterVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        SetSFXVolume(sfxVolume);

        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicVolumeSlider.value = musicVolume;
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        SetMusicVolume(musicVolume);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ContinueGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        menuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        masterVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        playerShooting.OnPause();
    }

    public void ContinueGame()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        playerShooting.OnResume();
    }

    public static bool IsPaused() => FindFirstObjectByType<PauseMenu>().isPaused;

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void SetSFXVolume(float volume)
    {
        var musicSources = musicPlayer ? musicPlayer.GetComponents<AudioSource>().ToList() : new System.Collections.Generic.List<AudioSource>();
        foreach (var source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            if (!musicSources.Contains(source))
                source.volume = volume;
        }

        foreach (var wind in FindObjectsByType<Wind>(FindObjectsSortMode.None))
            wind.UpdateVolume();

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    private void SetMusicVolume(float volume)
    {
        musicPlayer.SetVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}