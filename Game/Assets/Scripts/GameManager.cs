using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int emeraldCount;
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
        UpdateEmeraldUI();
    }

    public void AddEmerald()
    {
        emeraldCount++;
        UpdateEmeraldUI();
    }

    private void UpdateEmeraldUI()
    {
        emeraldCountText.text = $"x {emeraldCount}";
    }
}