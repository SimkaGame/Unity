using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI emeraldCountText;
    private int emeraldCount = 0;
    private HashSet<string> pickedEmeralds = new();
    private int playerHealth = 10;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGame();
    }

    private void Start()
    {
        UpdateEmeraldUI();
        FindEmeraldCountText();
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void OnApplicationQuit() => SaveGame();

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu")
        {
            FindEmeraldCountText();
            UpdateEmeraldUI();
        }
    }

    private void FindEmeraldCountText()
    {
        if (emeraldCountText || SceneManager.GetActiveScene().name == "MainMenu") return;

        emeraldCountText =
            GameObject.Find("EmeraldCountText")?.GetComponent<TextMeshProUGUI>() ??
            GameObject.FindWithTag("EmeraldCountText")?.GetComponent<TextMeshProUGUI>() ??
            FindFirstObjectByType<Canvas>()?.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void AddEmerald(string emeraldId)
    {
        if (pickedEmeralds.Add(emeraldId))
        {
            emeraldCount++;
            SaveGame();
            UpdateEmeraldUI();
        }
    }

    public bool IsEmeraldPicked(string emeraldId) => pickedEmeralds.Contains(emeraldId);

    private void UpdateEmeraldUI()
    {
        if (emeraldCountText)
            emeraldCountText.text = $" * {emeraldCount}";
    }

    public void CompleteLevel(int currentLevelIndex)
    {
        PlayerPrefs.SetInt("UnlockedLevel", currentLevelIndex + 1);
        PlayerPrefs.Save();
    }

    public void ResetGame()
    {
        emeraldCount = 0;
        pickedEmeralds.Clear();
        playerHealth = 10;

        PlayerPrefs.DeleteKey("PickedEmeralds");
        PlayerPrefs.SetInt("PlayerHealth", playerHealth);
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.Save();

        UpdateEmeraldUI();
    }

    public int GetPlayerHealth() => playerHealth;

    public void SetPlayerHealth(int health)
    {
        playerHealth = Mathf.Max(0, health);
        PlayerPrefs.SetInt("PlayerHealth", playerHealth);
        PlayerPrefs.Save();
    }

    public void SaveGame()
    {
        PlayerPrefs.SetInt("EmeraldCount", emeraldCount);
        PlayerPrefs.SetString("PickedEmeralds", string.Join(";", pickedEmeralds));
        PlayerPrefs.SetInt("PlayerHealth", playerHealth);
        PlayerPrefs.Save();
    }

    private void LoadGame()
    {
        emeraldCount = PlayerPrefs.GetInt("EmeraldCount", 0);
        playerHealth = PlayerPrefs.GetInt("PlayerHealth", 10);

        pickedEmeralds.Clear();
        string emeraldData = PlayerPrefs.GetString("PickedEmeralds", "");
        if (!string.IsNullOrEmpty(emeraldData))
        {
            foreach (string id in emeraldData.Split(';'))
                if (!string.IsNullOrEmpty(id)) pickedEmeralds.Add(id);
        }

        UpdateEmeraldUI();
    }
}
