using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button resetProgressButton;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown levelSelectDropdown;
    [SerializeField] private Button backButton;
    [SerializeField] private string[] levelScenes = { "Level_1", "Level_2" };

    private Resolution[] resolutions;
    private MusicPlayer musicPlayer;

    private void Awake()
    {
        playButton.onClick.AddListener(PlayGame);
        exitButton.onClick.AddListener(ExitGame);
        settingsButton.onClick.AddListener(OpenSettings);
        resetProgressButton.onClick.AddListener(ResetProgress);
        backButton.onClick.AddListener(CloseSettings);
        levelSelectDropdown.onValueChanged.AddListener(SaveSelectedLevelIndex);
    }

    private void Start()
    {
        musicPlayer = FindFirstObjectByType<MusicPlayer>();
        SetupSliders();
        SetupResolutions();
        SetupLevelSelect();

        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    private void OnEnable()
    {
        SetupLevelSelect();
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

    private void SetupResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentIndex = 0;
        var options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(new TMP_Dropdown.OptionData($"{resolutions[i].width} x {resolutions[i].height}"));
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", currentIndex);
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void SetupLevelSelect()
    {
        levelSelectDropdown.ClearOptions();
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        var options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>();
        for (int i = unlockedLevel - 1; i >= 0; i--)
            options.Add(new TMP_Dropdown.OptionData($"Уровень {i + 1}"));

        levelSelectDropdown.AddOptions(options);
        levelSelectDropdown.value = 0;
        PlayerPrefs.SetInt("LastLevelIndex", unlockedLevel - 1);
        PlayerPrefs.Save();
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
        if (musicPlayer) musicPlayer.SetVolume(volume);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    private void SetResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    private void PlayGame()
    {
        int dropdownIndex = levelSelectDropdown.value;
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        SceneManager.LoadScene(levelScenes[unlockedLevel - 1 - dropdownIndex]);
    }

    private void ExitGame()
    {
        GameManager.Instance?.SaveGame();
        Application.Quit();
    }

    private void OpenSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void SaveSelectedLevelIndex(int dropdownIndex)
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        PlayerPrefs.SetInt("LastLevelIndex", unlockedLevel - 1 - dropdownIndex);
        PlayerPrefs.Save();
    }

    private void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        GameManager.Instance?.ResetGame();
        CheckpointManager.Instance?.ResetCheckpoint();
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.SetInt("LastLevelIndex", 0);
        PlayerPrefs.SetFloat("SFXVolume", 1f);
        PlayerPrefs.SetFloat("MusicVolume", 1f);
        PlayerPrefs.Save();
        SetupLevelSelect();
        SetupSliders();
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        resetProgressButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        masterVolumeSlider.onValueChanged.RemoveAllListeners();
        musicVolumeSlider.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.RemoveAllListeners();
        levelSelectDropdown.onValueChanged.RemoveAllListeners();
    }
}