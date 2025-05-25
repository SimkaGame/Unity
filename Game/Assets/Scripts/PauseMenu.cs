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

        masterVolumeSlider.minValue = 0f;
        masterVolumeSlider.maxValue = 1f;
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        masterVolumeSlider.value = sfxVolume;
        masterVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        SetSFXVolume(sfxVolume);

        musicVolumeSlider.minValue = 0f;
        musicVolumeSlider.maxValue = 1f;
        float musicVolume = musicPlayer ? musicPlayer.GetVolume() : PlayerPrefs.GetFloat("MusicVolume", 1f);
        musicVolumeSlider.value = musicVolume;
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        SetMusicVolume(musicVolume);

        menuPanel.SetActive(false);
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
        if (musicPlayer) musicVolumeSlider.value = musicPlayer.GetVolume();

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
        var sfxSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        if (musicPlayer)
        {
            var musicSources = musicPlayer.GetComponents<AudioSource>();
            sfxSources = sfxSources.Where(source => source && !musicSources.Contains(source)).ToArray();
        }

        foreach (var source in sfxSources)
        {
            if (source) source.volume = volume;
        }

        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
    }

    private void SetMusicVolume(float volume)
    {
        if (musicPlayer) musicPlayer.SetVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}