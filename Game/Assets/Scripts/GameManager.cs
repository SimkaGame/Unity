using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int emeraldCount;
    private int playerHealth = 10; // начальное здоровье

    [SerializeField] private TextMeshProUGUI emeraldCountText;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        UpdateEmeraldUI();
    }

    public void AddEmerald()
    {
        emeraldCount++;
        UpdateEmeraldUI();
    }

    private void UpdateEmeraldUI()
    {
        if (emeraldCountText != null)
            emeraldCountText.text = $" * {emeraldCount}";
    }

    public int GetEmeraldCount() => emeraldCount;

    public void SetEmeraldCount(int value)
    {
        emeraldCount = value;
        UpdateEmeraldUI();
    }

    public int GetPlayerHealth() => playerHealth;

    public void SetPlayerHealth(int value) => playerHealth = value;

    public void SetEmeraldCountText(TextMeshProUGUI text)
    {
        emeraldCountText = text;
        UpdateEmeraldUI();
    }

    public void ResetGame()
    {
        emeraldCount = 0;
        playerHealth = 10;
        UpdateEmeraldUI();
    }
}
