using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float airTimeThreshold = 1f;
    [SerializeField] private float damagePerSecond = 2f;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartsContainer;

    private int currentHealth;
    private float airTime;
    private bool wasGroundedLastFrame = true;
    private Image[] heartImages;
    private PlayerController playerController;
    private DamageFlash damageFlash;
    private PlayerAudioController audioController;
    private bool heartsInitialized;

    public int CurrentHealth => currentHealth;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        damageFlash = GetComponent<DamageFlash>();
        audioController = GetComponent<PlayerAudioController>();

        currentHealth = GameManager.Instance != null ? GameManager.Instance.GetPlayerHealth() : maxHealth;
        if (currentHealth <= 0)
        {
            ResetHealth();
        }
        heartsInitialized = false;
        InitializeHearts();
    }

    private void FixedUpdate()
    {
        bool isGrounded = playerController.IsGrounded;

        if (!isGrounded)
        {
            airTime += Time.deltaTime;
        }
        else
        {
            if (!wasGroundedLastFrame && airTime >= airTimeThreshold)
            {
                TakeDamage(Mathf.FloorToInt(airTime * damagePerSecond), false);
            }
            airTime = 0f;
        }

        wasGroundedLastFrame = isGrounded;
    }

    private void InitializeHearts()
    {
        if (heartsInitialized)
        {
            UpdateHearts();
            return;
        }

        foreach (Transform child in heartsContainer)
        {
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        heartImages = new Image[maxHealth];
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            heartImages[i] = heart.GetComponent<Image>();
            if (heartImages[i] == null)
            {
                continue;
            }
            heartImages[i].enabled = true;

            RectTransform heartRect = heart.GetComponent<RectTransform>();
            heartRect.anchoredPosition = new Vector2(15 + i * 50, -15);
            heartRect.sizeDelta = new Vector2(45, 45);
            heartRect.localScale = Vector3.one;
        }

        heartsInitialized = true;
        UpdateHearts();
    }

    public void TakeDamage(int damage, bool isFromTrap = true)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);
        GameManager.Instance?.SetPlayerHealth(currentHealth);
        UpdateHearts();

        if (!isFromTrap)
        {
            damageFlash.PlayFlash();
            audioController.PlayDamageSound();
        }

        if (currentHealth <= 0)
        {
            Respawn();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        GameManager.Instance?.SetPlayerHealth(currentHealth);
        UpdateHearts();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        GameManager.Instance?.SetPlayerHealth(currentHealth);
        heartsInitialized = false;
        InitializeHearts();
        UpdateHearts();
    }

    public void ResetAirTime()
    {
        airTime = 0f;
    }

    private void UpdateHearts()
    {
        if (heartImages == null || heartImages.Length == 0)
        {
            InitializeHearts();
            return;
        }

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                heartImages[i].enabled = i < currentHealth;
            }
        }
    }

    private void Respawn()
    {
        transform.position = CheckpointManager.Instance.GetLastCheckpointPosition();
        currentHealth = maxHealth;
        GameManager.Instance?.SetPlayerHealth(currentHealth);
        airTime = 0f;
        heartsInitialized = false;
        InitializeHearts();
        UpdateHearts();

        foreach (var portal in Object.FindObjectsByType<Portal>(FindObjectsSortMode.None))
        {
            portal.ResetTriggerCooldown();
        }
    }
}
