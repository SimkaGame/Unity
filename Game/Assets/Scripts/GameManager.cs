using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private int emeraldCount = 0;

    [SerializeField] private TextMeshProUGUI emeraldCountText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
        {
            emeraldCountText.text = $"x {emeraldCount}";
        }
    }
}