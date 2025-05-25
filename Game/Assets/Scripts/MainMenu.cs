using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

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
    private int currentResolutionIndex;
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
        InitializeAudioSettings();
        InitializeResolutions();
        InitializeLevelSelect();

        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    private void OnEnable()
    {
        InitializeLevelSelect();
    }

    private void InitializeAudioSettings()
    {
        musicPlayer = FindFirstObjectByType<MusicPlayer>();

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
    }

    private void InitializeResolutions()
{
    resolutions = Screen.resolutions
        .GroupBy(r => new { r.width, r.height })
        .Select(g => g.OrderByDescending(r => r.refreshRateRatio).First())
        .ToArray();

    resolutionDropdown.ClearOptions();

    List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
    currentResolutionIndex = 0;

    for (int i = 0; i < resolutions.Length; i++)
    {
        string option = $"{resolutions[i].width} x {resolutions[i].height}".ToUpper();
        options.Add(new TMP_Dropdown.OptionData(option));

        if (resolutions[i].width == Screen.currentResolution.width &&
            resolutions[i].height == Screen.currentResolution.height)
        {
            currentResolutionIndex = i;
        }
    }

    resolutionDropdown.AddOptions(options);
    resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
    resolutionDropdown.RefreshShownValue();
    resolutionDropdown.onValueChanged.AddListener(SetResolution);
}

    private void InitializeLevelSelect()
    {
        levelSelectDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        for (int i = 0; i < levelScenes.Length && i < unlockedLevel; i++)
        {
            options.Add(new TMP_Dropdown.OptionData($"Уровень {i + 1}"));
        }

        levelSelectDropdown.AddOptions(options);

        int selectedLevelIndex = PlayerPrefs.GetInt("LastLevelIndex", 0);
        selectedLevelIndex = Mathf.Clamp(selectedLevelIndex, 0, options.Count - 1);

        levelSelectDropdown.onValueChanged.RemoveAllListeners();
        levelSelectDropdown.value = selectedLevelIndex;
        levelSelectDropdown.RefreshShownValue();
        levelSelectDropdown.onValueChanged.AddListener(SaveSelectedLevelIndex);
    }

    private void SetSFXVolume(float volume)
    {
        var sfxSources = Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        if (musicPlayer)
        {
            var musicSources = musicPlayer.GetComponents<AudioSource>();
            sfxSources = sfxSources.Where(source => source && !musicSources.Contains(source)).ToArray();
        }

        foreach (var source in sfxSources)
        {
            source.volume = volume;
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

    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }

    private void PlayGame()
    {
        int levelIndex = PlayerPrefs.GetInt("LastLevelIndex", 0);
        SceneManager.LoadScene(levelScenes[levelIndex]);
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

    private void SaveSelectedLevelIndex(int index)
    {
        PlayerPrefs.SetInt("LastLevelIndex", index);
        PlayerPrefs.Save();
    }

    private void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        GameManager.Instance?.ResetGame();
        CheckpointManager.Instance?.ResetCheckpoint();
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.SetInt("LastLevelIndex", 0);
        PlayerPrefs.Save();
        InitializeLevelSelect();
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(PlayGame);
        exitButton.onClick.RemoveListener(ExitGame);
        settingsButton.onClick.RemoveListener(OpenSettings);
        resetProgressButton.onClick.RemoveListener(ResetProgress);
        backButton.onClick.RemoveListener(CloseSettings);
        masterVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
        musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
        levelSelectDropdown.onValueChanged.RemoveListener(SaveSelectedLevelIndex);
    }
}
