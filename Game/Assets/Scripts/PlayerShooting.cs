using System.Collections;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Настройка стрельбы")]
    public GameObject arrowPrefab;
    public float fireRate = 1f;
    public float arrowSpeed = 10f;
    public float fireOffset = 0.5f;
    public float fireHeightOffset = 0.2f;

    private Animator bowAnimator;
    private SpriteRenderer bowSpriteRenderer;
    private Sprite idleBowSprite;
    private Transform bowTransform;

    private float bowAngleOffset = 0f;
    private float rightSideAngleOffset = 0f;
    private float leftSideAngleOffset = 0f;

    private bool canShoot = true;
    private PlayerAnimator playerAnimator;
    private PlayerController playerController;
    private float initialBowAngle;

    void Start()
    {
        bowAnimator = transform.Find("Bow")?.GetComponent<Animator>();
        bowSpriteRenderer = transform.Find("Bow")?.GetComponent<SpriteRenderer>();
        bowTransform = transform.Find("Bow")?.GetComponent<Transform>();
        idleBowSprite = bowSpriteRenderer?.sprite;

        if (bowSpriteRenderer != null)
        {
            bowSpriteRenderer.enabled = false;
        }

        playerAnimator = GetComponent<PlayerAnimator>();
        playerController = GetComponent<PlayerController>();
        initialBowAngle = bowTransform.localEulerAngles.z;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    IEnumerator ShootCoroutine()
    {
        canShoot = false;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector2 direction = (mouseWorldPos - transform.position).normalized;


        if (playerController != null)
        {
            bool facingRight = mouseWorldPos.x > transform.position.x;
            if (facingRight != playerController.FacingRight)
            {
                playerController.Flip();
            }
            playerController.SlowDown(0.5f);
        }

        if (bowSpriteRenderer != null && bowTransform != null)
        {
            bowSpriteRenderer.enabled = true;
            bowTransform.localScale = new Vector3(
                Mathf.Abs(bowTransform.localScale.x),
                Mathf.Abs(bowTransform.localScale.y),
                1
            );

            float angle = playerController.FacingRight
                ? Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg + rightSideAngleOffset + 50f
                : (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f) % 360f + leftSideAngleOffset + initialBowAngle;

            angle = (angle + bowAngleOffset) % 360f;
            if (angle < 0) angle += 360f;
            bowTransform.localRotation = Quaternion.Euler(0, 0, angle);
        }

        playerAnimator?.SetShooting(true);

        if (bowAnimator != null)
        {
            bowAnimator.enabled = true;
            bowAnimator.Play("BowPull", -1, 0f);
        }

        yield return new WaitForSeconds(1f);

        Vector2 spawnPosition = new Vector2(
            transform.position.x + (mouseWorldPos.x < transform.position.x ? -fireOffset : fireOffset),
            transform.position.y + fireHeightOffset
        );

        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
        if (arrow != null)
        {
            Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
            if (arrowRb != null)
            {
                arrowRb.linearVelocity = direction * arrowSpeed;
            }
            arrow.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);

            Collider2D arrowCollider = arrow.GetComponent<Collider2D>();
            if (arrowCollider != null)
            {
                foreach (var col in FindObjectsByType<Collider2D>(FindObjectsSortMode.None))
                {
                    if (col.CompareTag("Player") || col.CompareTag("Collectible") || col.CompareTag("DeathZone") || col.CompareTag("Checkpoint"))
                    {
                        Physics2D.IgnoreCollision(arrowCollider, col);
                    }
                }
            }
        }

        if (bowAnimator != null && bowSpriteRenderer != null)
        {
            bowAnimator.enabled = false;
            bowSpriteRenderer.sprite = idleBowSprite;
            bowSpriteRenderer.enabled = false;
            bowTransform.localRotation = Quaternion.Euler(0, 0, initialBowAngle);
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetShooting(false);
            playerAnimator.GetComponent<Animator>()?.Play("Idle", -1, 0f);
        }

        playerController?.RestoreSpeed();

        yield return new WaitForSeconds(fireRate - 1f);
        canShoot = true;
    }
}