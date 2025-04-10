using System.Collections;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Настройка стрельбы")]
    public GameObject arrowPrefab;
    public float fireRate = 1f;
    public float arrowSpeed = 20f;
    public float fireOffset = 0.8f;
    public float fireHeightOffset = 0.4f;

    private bool canShoot = true;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (canShoot)
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    IEnumerator ShootCoroutine()
    {
        canShoot = false;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector2 spawnPosition;
        Vector2 direction = (mouseWorldPos - transform.position).normalized;

        if (mouseWorldPos.x < transform.position.x)
        {
            spawnPosition = new Vector2(transform.position.x - fireOffset, transform.position.y + fireHeightOffset);
        }
        else
        {
            spawnPosition = new Vector2(transform.position.x + fireOffset, transform.position.y + fireHeightOffset);
        }

        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
        if (arrow == null)
        {
            yield break;
        }

        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            arrowRb.linearVelocity = direction * arrowSpeed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Collider2D[] ignoreColliders = GameObject.FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
        foreach (var col in ignoreColliders)
        {
            if (col.CompareTag("Player") || col.CompareTag("Collectible") || col.CompareTag("DeathZone") || col.CompareTag("Checkpoint"))
            {
                Physics2D.IgnoreCollision(arrow.GetComponent<Collider2D>(), col);
            }
        }

        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }
}