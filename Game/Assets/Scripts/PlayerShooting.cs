using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float arrowSpeed = 10f;
    [SerializeField] private float fireOffset = 0.5f;
    [SerializeField] private float fireHeightOffset = 0.2f;
    
    private float rightAngleOffset = 50f;
    private float leftAngleOffset = -90f;
    private Animator bowAnimator;
    private SpriteRenderer bowSpriteRenderer;
    private Sprite idleBowSprite;
    private Transform bowTransform;
    private PlayerAnimator playerAnimator;
    private PlayerController playerController;
    private PlayerAudioController audioController;
    private float initialBowAngle;
    private bool canShoot = true;
    private bool wasBowEnabled;
    private bool isShooting;
    private bool? lastShootingDirection;

    private void Awake()
    {
        bowAnimator = transform.Find("Bow").GetComponent<Animator>();
        bowSpriteRenderer = transform.Find("Bow").GetComponent<SpriteRenderer>();
        bowTransform = transform.Find("Bow").GetComponent<Transform>();
        idleBowSprite = bowSpriteRenderer.sprite;
        initialBowAngle = bowTransform.localEulerAngles.z;
        bowSpriteRenderer.enabled = false;
        playerAnimator = GetComponent<PlayerAnimator>();
        playerController = GetComponent<PlayerController>();
        audioController = GetComponent<PlayerAudioController>();
    }

    private void Update()
    {
        bool facingRight;

        if (isShooting)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            facingRight = mouseWorldPos.x > transform.position.x;
            lastShootingDirection = facingRight;
        }
        else
        {
            float horizontalMove = playerController.HorizontalMove;
            facingRight = horizontalMove != 0 ? horizontalMove > 0 : lastShootingDirection ?? playerController.FacingRight;
            lastShootingDirection = facingRight;
        }

        if (facingRight != playerController.FacingRight)
        {
            playerController.Flip();
        }

        playerAnimator.UpdateAnimation(playerController.HorizontalMove, playerController.IsGrounded, playerController.FacingRight);

        if (Input.GetMouseButtonDown(0) && canShoot && !PauseMenu.IsPaused())
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    public void OnPause()
    {
        wasBowEnabled = bowSpriteRenderer.enabled;
        bowSpriteRenderer.enabled = false;
        bowAnimator.enabled = false;
    }

    public void OnResume()
    {
        bowSpriteRenderer.enabled = wasBowEnabled;
        if (wasBowEnabled) bowAnimator.enabled = true;
    }

    private IEnumerator ShootCoroutine()
    {
        canShoot = false;
        isShooting = true;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        bool bowFacingRight = mouseWorldPos.x > transform.position.x;

        playerController.SlowDown(0.5f);
        bowSpriteRenderer.enabled = true;
        bowTransform.localScale = new Vector3(
            bowFacingRight ? Mathf.Abs(bowTransform.localScale.x) : -Mathf.Abs(bowTransform.localScale.x),
            Mathf.Abs(bowTransform.localScale.y),
            1
        );

        playerAnimator.SetShooting(true);
        bowAnimator.enabled = true;
        bowAnimator.Play("BowPull", -1, 0f);

        float elapsedTime = 0f;
        float pullDuration = 0.95f;
        while (elapsedTime < pullDuration)
        {
            mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            direction = (mouseWorldPos - transform.position).normalized;
            bowFacingRight = mouseWorldPos.x > transform.position.x;

            float angle = bowFacingRight
                ? Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg + rightAngleOffset
                : Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + leftAngleOffset + initialBowAngle;
            if (angle < 0) angle += 360f;
            bowTransform.localRotation = Quaternion.Euler(0, 0, angle);

            if (bowFacingRight != playerController.FacingRight)
            {
                playerController.Flip();
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        direction = (mouseWorldPos - transform.position).normalized;
        bowFacingRight = mouseWorldPos.x > transform.position.x;

        if (bowFacingRight != playerController.FacingRight)
        {
            playerController.Flip();
        }

        float finalAngle = bowFacingRight
            ? Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg + rightAngleOffset
            : Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + leftAngleOffset + initialBowAngle;
        if (finalAngle < 0) finalAngle += 360f;
        bowTransform.localRotation = Quaternion.Euler(0, 0, finalAngle);

        Vector2 spawnPosition = new Vector2(
            transform.position.x + (bowFacingRight ? fireOffset : -fireOffset),
            transform.position.y + fireHeightOffset
        );

        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        arrowRb.linearVelocity = direction * arrowSpeed;
        arrow.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);

        Collider2D arrowCollider = arrow.GetComponent<Collider2D>();
        foreach (var col in FindObjectsByType<Collider2D>(FindObjectsSortMode.None))
        {
            if (col.CompareTag("Player") || col.CompareTag("Collectible") || col.CompareTag("DeathZone") || col.CompareTag("Checkpoint"))
                Physics2D.IgnoreCollision(arrowCollider, col);
        }

        audioController.PlayShootSound();

        yield return new WaitForSeconds(0.05f);

        bowAnimator.enabled = false;
        bowSpriteRenderer.sprite = idleBowSprite;
        bowSpriteRenderer.enabled = false;
        bowTransform.localRotation = Quaternion.Euler(0, 0, initialBowAngle);
        playerAnimator.SetShooting(false);
        playerAnimator.GetComponent<Animator>().Play("Idle", -1, 0f);
        playerController.RestoreSpeed();
        isShooting = false;

        yield return new WaitForSeconds(fireRate - 1f);
        canShoot = true;
    }
}