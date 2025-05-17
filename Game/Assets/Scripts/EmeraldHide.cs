using UnityEngine;

public class EmeraldHide : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private string emeraldId; // Уникальный ID изумруда
    private bool isPicked;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // Если ID не задан, используем имя объекта + сцену
        if (string.IsNullOrEmpty(emeraldId))
            emeraldId = $"{gameObject.name}_{gameObject.scene.name}";

        // Проверяем, был ли изумруд уже подобран
        if (GameManager.Instance != null && GameManager.Instance.IsEmeraldPicked(emeraldId))
        {
            Destroy(gameObject); // Уничтожаем, если уже подобран
        }
    }

    public void TriggerPickup()
    {
        if (isPicked) return;

        isPicked = true;
        if (audioSource?.clip)
            audioSource.PlayOneShot(audioSource.clip);

        GameManager.Instance.AddEmerald(emeraldId);
        Destroy(gameObject, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPicked)
            TriggerPickup();
    }
}