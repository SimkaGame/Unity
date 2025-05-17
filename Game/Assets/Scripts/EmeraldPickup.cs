using UnityEngine;

public class EmeraldPickup : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource;
    private bool isPicked;
    [SerializeField] private string emeraldId; // Уникальный ID изумруда

    private void Awake()
    {
        anim = GetComponent<Animator>();
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPicked)
        {
            isPicked = true;
            anim.SetTrigger("pickupTrigger");
            audioSource.PlayOneShot(audioSource.clip);
            GameManager.Instance.AddEmerald(emeraldId);
            Destroy(gameObject, 0.5f);
        }
    }
}