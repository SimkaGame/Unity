using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class EnemyAI : MonoBehaviour
{
    private Seeker seeker;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Transform target;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float nextWaypointDistance = 1.5f;
    [SerializeField] private float updatePathInterval = 0.5f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float checkGroundOffsetY = -1.8f;
    [SerializeField] private float checkGroundRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float jumpCheckDistance = 1f;
    [SerializeField] private float jumpHeightThreshold = 0.5f;
    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private Vector2 attackBoxSize = new Vector2(1.5f, 1.5f);
    [SerializeField] private Vector2 attackBoxOffset = Vector2.zero;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float chaseDistance = 10f;
    [SerializeField] private int health = 10;

    private float lastJumpTime;
    private float lastAttackTime;
    private int currentHealth;
    private Path path;
    private int currentWaypoint;
    private bool facingRight = true;
    private bool isChasing;

    public int CurrentHealth => currentHealth;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 2f;
        rb.linearDamping = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        target ??= GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = health;

        InvokeRepeating(nameof(UpdatePath), 0f, updatePathInterval);

        Collider2D col = GetComponent<Collider2D>();
        col.sharedMaterial = new PhysicsMaterial2D { friction = 0f };
    }

    private void UpdatePath()
    {
        if (seeker.IsDone() && target && isChasing)
            seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!target)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        isChasing = Vector2.Distance(transform.position, target.position) <= chaseDistance;

        if (!isChasing || path == null || currentWaypoint >= path.vectorPath.Count)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        CheckGround();

        Vector2 currentPos = rb.position;
        Vector2 targetPos = path.vectorPath[currentWaypoint];
        Vector2 direction = (targetPos - currentPos).normalized;

        float horizontalMove = direction.x * speed;
        rb.linearVelocity = new Vector2(
            Mathf.Lerp(rb.linearVelocity.x, horizontalMove * 5f, Time.fixedDeltaTime * 10f),
            rb.linearVelocity.y
        );

        if (isGrounded && Time.time >= lastJumpTime + jumpCooldown)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, new Vector2(direction.x, 0), jumpCheckDistance, groundLayer);
            bool obstacleAhead = hit.collider != null;
            bool needsJumpUp = targetPos.y > currentPos.y + jumpHeightThreshold;

            if (obstacleAhead || needsJumpUp)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                lastJumpTime = Time.time;
            }
        }

        if (Vector2.Distance(currentPos, targetPos) < nextWaypointDistance)
            currentWaypoint++;

        if (Mathf.Abs(horizontalMove) > 0.01f)
        {
            bool movingRight = horizontalMove < 0;
            if (movingRight != facingRight)
                Flip();
        }

        CheckAttack();
    }

    private void CheckAttack()
    {
        Vector2 attackPos = (Vector2)transform.position + attackBoxOffset;
        Collider2D hit = Physics2D.OverlapBox(attackPos, attackBoxSize, 0f, playerLayer);

        if (hit && hit.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            hit.GetComponent<PlayerHealth>().TakeDamage(damage, isFromTrap: false);
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            GetComponent<EnemyAudioController>()?.PlayDeathSound();
            Destroy(gameObject);
        }
        else
        {
            GetComponent<DamageFlash>()?.PlayFlash();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY), checkGroundRadius, groundLayer);
    }
}