using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public AudioSource musicSource;
    private bool isPaused = false;

    void Start()
    {
        masterVolumeSlider.value = AudioListener.volume;
        musicVolumeSlider.value = musicSource.volume;

        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

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
    }

    public void ContinueGame()
    {
        menuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // public void ExitGame()
    // {
    //     Application.Quit();
    //     Debug.Log("Выход из игры");
    // }

    public void ExitGame()
{
    Application.Quit();
    #if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
    #endif
    Debug.Log("Выход из игры");
}
    void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
}