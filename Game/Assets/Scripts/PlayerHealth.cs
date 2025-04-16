using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;
    public GameObject heartPrefab;
    public Transform heartsContainer;
    private Image[] heartImages;

    private void Start()
    {
        currentHealth = maxHealth;
        InitializeHearts();
    }

    private void InitializeHearts()
    {
        heartImages = new Image[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            heartImages[i] = heart.GetComponent<Image>();
            heartImages[i].enabled = true;

            RectTransform heartRect = heart.GetComponent<RectTransform>();
            float xOffset = 15 + i * 45;
            float yOffset = -15;
            heartRect.anchoredPosition = new Vector2(xOffset, yOffset);
        }
        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHearts();

        if (currentHealth <= 0)
        {
            Respawn();
        }
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = i < currentHealth;
        }
    }

    private void Respawn()
    {
        transform.position = CheckpointManager.Instance.GetLastCheckpointPosition();
        currentHealth = maxHealth;
        UpdateHearts();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHearts();
    }
}