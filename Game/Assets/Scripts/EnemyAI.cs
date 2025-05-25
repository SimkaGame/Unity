using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 0.2f;
    [SerializeField] private float nextWaypointDistance = 2f;
    [SerializeField] private float updatePathInterval = 0.5f;
    [SerializeField] private float checkGroundOffsetY = -1.8f;
    [SerializeField] private float checkGroundRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float jumpCheckDistance = 1f;
    [SerializeField] private float jumpHeightThreshold = 0.5f;
    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private Vector2 attackBoxSize = new Vector2(4f, 4f);
    [SerializeField] private Vector2 attackBoxOffset = new Vector2(1f, 0f);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float chaseDistance = 15f;
    [SerializeField] private int health = 10;

    private Seeker seeker;
    private Rigidbody2D rb;
    private EnemyAudioController audioController;
    private DamageFlash damageFlash;
    private Animator animator;
    private float lastJumpTime;
    private float lastAttackTime;
    private int currentHealth;
    private Path path;
    private int currentWaypoint;
    private bool facingRight;
    private bool isChasing;
    private bool isGrounded;
    private bool isInTrap;
    private Vector2 smoothedDirection;

    public int CurrentHealth => currentHealth;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        audioController = GetComponent<EnemyAudioController>();
        damageFlash = GetComponent<DamageFlash>();
        animator = GetComponent<Animator>();

        rb.gravityScale = 2f;
        rb.linearDamping = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (!target)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) target = playerObj.transform;
        }

        currentHealth = health;
        InvokeRepeating(nameof(UpdatePath), 0f, updatePathInterval);

        var col = GetComponent<Collider2D>();
        col.sharedMaterial = new PhysicsMaterial2D { friction = 0f };
    }

    private void UpdatePath()
    {
        if (seeker.IsDone() && target)
            seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
        else
        {
            path = null;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!target)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) target = playerObj.transform;
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                animator.SetBool("isWalking", false);
                return;
            }
        }

        isChasing = Vector2.Distance(transform.position, target.position) <= chaseDistance;

        if (!isChasing || path == null || currentWaypoint >= path.vectorPath.Count)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("isWalking", false);
            UpdatePath();
            return;
        }

        CheckGround();

        var currentPos = rb.position;
        var targetPos = path.vectorPath[currentWaypoint];
        var direction = ((Vector2)targetPos - currentPos).normalized;

        smoothedDirection = Vector2.Lerp(smoothedDirection, direction, Time.fixedDeltaTime * 10f);

        var horizontalMove = smoothedDirection.x * speed;
        rb.linearVelocity = new Vector2(
            Mathf.Lerp(rb.linearVelocity.x, horizontalMove, Time.fixedDeltaTime * 10f),
            rb.linearVelocity.y
        );

        animator.SetBool("isWalking", Mathf.Abs(horizontalMove) > 0.1f);

        if (isGrounded && Time.time >= lastJumpTime + jumpCooldown)
        {
            var hit = Physics2D.Raycast(currentPos, new Vector2(direction.x, 0), jumpCheckDistance, groundLayer);
            var obstacleAhead = hit.collider != null;
            var needsJumpUp = targetPos.y > currentPos.y + jumpHeightThreshold;

            if (obstacleAhead || needsJumpUp)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                lastJumpTime = Time.time;
            }
        }

        if (Vector2.Distance(currentPos, targetPos) < nextWaypointDistance)
            currentWaypoint++;

        if (Mathf.Abs(horizontalMove) > 0.1f)
        {
            var movingRight = horizontalMove > 0;
            if (movingRight != facingRight)
                Flip();
        }

        CheckAttack();
    }

    private void CheckAttack()
    {
        var attackPos = (Vector2)transform.position + (facingRight ? attackBoxOffset : new Vector2(-attackBoxOffset.x, attackBoxOffset.y));
        var hit = Physics2D.OverlapBox(attackPos, attackBoxSize, 0f, playerLayer);

        if (hit && hit.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            var playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth && playerHealth.CurrentHealth > 0)
            {
                playerHealth.TakeDamage(damage, false);
                lastAttackTime = Time.time;
                animator.SetTrigger("attack");
            }
        }
    }

    public void TakeDamage(int damage, bool isFromTrap = false)
    {
        if (!isFromTrap && isInTrap)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            audioController?.PlayDeathSound();
            Destroy(gameObject);
        }
        else
        {
            if (isFromTrap)
            {
                audioController?.PlayBurnSound();
            }
            else
            {
                audioController?.PlayHitSound();
            }
            damageFlash?.PlayFlash();
        }
    }

    public void SetIsInTrap(bool value)
    {
        isInTrap = value;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        var theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY), checkGroundRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        var attackPos = (Vector2)transform.position + (facingRight ? attackBoxOffset : new Vector2(-attackBoxOffset.x, attackBoxOffset.y));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos, attackBoxSize);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}