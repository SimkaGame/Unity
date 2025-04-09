using System.Collections;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Настройка стрельбы")]
    public GameObject arrowPrefab;
    public Transform firePointLeft;
    public Transform firePointRight;
    public float fireRate = 1f;
    public float arrowSpeed = 10f;

    private bool canShoot = true;
    private bool facingRight = true;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

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
        Vector2 direction = (mouseWorldPos - firePointRight.position).normalized;

        if (playerController.isMovingRight)
        {
            direction = (mouseWorldPos - firePointRight.position).normalized;
        }
        else
        {
            direction = (mouseWorldPos - firePointLeft.position).normalized;
        }

        Vector2 spawnPosition = playerController.isMovingRight ? firePointRight.position : firePointLeft.position;

        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);

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

    public void SetFacingRight(bool isFacingRight)
    {
        facingRight = isFacingRight;
    }
}
