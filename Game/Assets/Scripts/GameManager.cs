using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int emeraldCount;
    private int playerHealth = 10;
    private HashSet<string> pickedEmeralds = new HashSet<string>(); // Храним ID подобранных изумрудов

    [SerializeField] private TextMeshProUGUI emeraldCountText;

    private void Awake()
    {
        if (Instance)
        {
            Debug.Log($"GameManager: Destroying duplicate instance, keeping emeraldCount = {Instance.emeraldCount}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"GameManager: Initialized with emeraldCount = {emeraldCount}");
    }

    private void Start()
    {
        UpdateEmeraldUI();
    }

    public void AddEmerald(string emeraldId)
    {
        if (!pickedEmeralds.Contains(emeraldId))
        {
            pickedEmeralds.Add(emeraldId);
            emeraldCount++;
            UpdateEmeraldUI();
            Debug.Log($"GameManager: Added emerald {emeraldId}, emeraldCount = {emeraldCount}");
        }
    }

    public bool IsEmeraldPicked(string emeraldId) => pickedEmeralds.Contains(emeraldId);

    private void UpdateEmeraldUI()
    {
        if (emeraldCountText != null)
        {
            emeraldCountText.text = $" * {emeraldCount}";
            Debug.Log($"GameManager: Updated UI, emeraldCount = {emeraldCount}");
        }
        else
        {
            Debug.LogWarning("GameManager: emeraldCountText is null, cannot update UI");
        }
    }

    public int GetEmeraldCount() => emeraldCount;

    public void SetEmeraldCount(int value)
    {
        emeraldCount = value;
        UpdateEmeraldUI();
        Debug.Log($"GameManager: Set emeraldCount = {value}");
    }

    public int GetPlayerHealth() => playerHealth;

    public void SetPlayerHealth(int value) => playerHealth = value;

    public void SetEmeraldCountText(TextMeshProUGUI text)
    {
        emeraldCountText = text;
        UpdateEmeraldUI();
        Debug.Log($"GameManager: Set emeraldCountText, emeraldCount = {emeraldCount}");
    }

    public void ResetGame()
    {
        emeraldCount = 0;
        playerHealth = 10;
        pickedEmeralds.Clear();
        UpdateEmeraldUI();
        Debug.Log("GameManager: Game reset");
    }
}