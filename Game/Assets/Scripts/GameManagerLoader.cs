using UnityEngine;
using TMPro;

public class GameManagerLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI emeraldCountText;

    private void Start()
    {
        if (GameManager.Instance != null && emeraldCountText != null)
        {
            GameManager.Instance.SetEmeraldCountText(emeraldCountText);
        }
    }
}
