using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;
    public GameObject heartPrefab;
    public Transform heartsContainer;
    private Image[] heartImages;

    [Header("Fall Damage Settings")]
    [SerializeField] private float airTimeThreshold = 1f;
    [SerializeField] private float damagePerSecond = 2f;

    private float airTime = 0f;
    private bool wasGroundedLastFrame = true;
    private PlayerController playerController;
    private DamageFlash damageFlash;
    private PlayerAudioController audioController;

    public int CurrentHealth => currentHealth;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        damageFlash = GetComponent<DamageFlash>();
        audioController = GetComponent<PlayerAudioController>();

        currentHealth = maxHealth;
        InitializeHearts();
    }

    private void Update()
    {
        if (!Application.isPlaying && heartImages == null)
        {
            InitializeHearts();
        }
    }

    private void FixedUpdate()
    {
        if (playerController == null) return;

        bool isGrounded = playerController.IsGrounded;

        if (!isGrounded)
        {
            airTime += Time.deltaTime;
        }
        else
        {
            if (!wasGroundedLastFrame && airTime >= airTimeThreshold)
            {
                int fallDamage = Mathf.FloorToInt(airTime * damagePerSecond);
                TakeDamage(fallDamage, isFromTrap: false);
            }
            airTime = 0f;
        }

        wasGroundedLastFrame = isGrounded;
    }

    private void InitializeHearts()
    {
        if (heartsContainer == null || heartPrefab == null) return;

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
            Image heartImage = heart.GetComponent<Image>();
            if (heartImage == null) continue;

            heartImages[i] = heartImage;
            heartImages[i].enabled = true;

            RectTransform heartRect = heart.GetComponent<RectTransform>();
            float xOffset = 15 + i * 45;
            float yOffset = -15;
            heartRect.anchoredPosition = new Vector2(xOffset, yOffset);
        }

        UpdateHearts();
    }

    public void TakeDamage(int damage, bool isFromTrap = true)
    {
        if (currentHealth <= 0) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHearts();

        if (!isFromTrap)
        {
            if (damageFlash != null)
            {
                damageFlash.PlayFlash();
            }
            if (audioController != null)
            {
                audioController.PlayDamageSound();
            }
        }

        if (currentHealth <= 0)
        {
            Respawn();
        }
    }

    private void UpdateHearts()
    {
        if (heartImages == null || heartImages.Length == 0) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;
            heartImages[i].enabled = i < currentHealth;
        }
    }

    private void Respawn()
    {
        transform.position = CheckpointManager.Instance.GetLastCheckpointPosition();
        currentHealth = maxHealth;
        airTime = 0f;
        UpdateHearts();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        UpdateHearts();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHearts();
    }
}