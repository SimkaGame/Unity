using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
    [SerializeField] private int damage = 4;
    
    private AudioSource audioSource;
    private Rigidbody2D rb;
    private Camera mainCamera;
    private bool isStuck;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (isStuck) return;
        
        var screenPos = mainCamera.WorldToViewportPoint(transform.position);
        if (screenPos.x < 0 || screenPos.x > 1 || screenPos.y < 0 || screenPos.y > 1)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleHit(collision.gameObject, collision.contacts[0].normal);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject, Vector2.zero);
    }

    private void HandleHit(GameObject target, Vector2 normal)
    {
        if (target.CompareTag("Enemy") && target.TryGetComponent<EnemyHealth>(out var enemy))
        {
            enemy.TakeDamage(damage, false, true);
            Destroy(gameObject);
        }
        else if (target.layer == LayerMask.NameToLayer("Ground") && audioSource != null && audioSource.clip != null)
        {
            StartCoroutine(StickToTilemap(normal));
        }
    }

    private IEnumerator StickToTilemap(Vector2 normal)
    {
        isStuck = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (normal != Vector2.zero)
        {
            transform.position += (Vector3)(normal * -0.1f);
        }
        else
        {
            transform.position += (Vector3)(rb.linearVelocity.normalized * 0.1f);
        }

        audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        audioSource.spatialBlend = 0f;
        audioSource.Play();

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}