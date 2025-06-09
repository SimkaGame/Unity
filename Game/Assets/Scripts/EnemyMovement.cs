using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker), typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
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
    [SerializeField] private float chaseDistance = 15f;
    [SerializeField] private float chaseDistanceIncrease = 5f;

    private Seeker seeker;
    private Rigidbody2D rb;
    private EnemyAnimationController animationController;
    private EnemyHealth health;
    private Path path;
    private int currentWaypoint;
    private bool facingRight;
    private bool isChasing;
    private bool isGrounded;
    private float lastJumpTime;
    private Vector2 smoothedDirection;

    public bool FacingRight => facingRight;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animationController = GetComponent<EnemyAnimationController>();
        health = GetComponent<EnemyHealth>();

        rb.gravityScale = 2f;
        rb.linearDamping = 1f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (!target)
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj) target = playerObj.transform;
        }

        var col = GetComponent<Collider2D>();
        col.sharedMaterial = new PhysicsMaterial2D { friction = 0f };

        health.OnDamageTaken += HandleDamageTaken;

        InvokeRepeating(nameof(UpdatePath), 0f, updatePathInterval);
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDamageTaken -= HandleDamageTaken;
    }

    private void HandleDamageTaken()
    {
        chaseDistance += chaseDistanceIncrease;
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
                animationController?.SetWalking(false);
                return;
            }
        }

        isChasing = Vector2.Distance(transform.position, target.position) <= chaseDistance;

        if (!isChasing || path == null || currentWaypoint >= path.vectorPath.Count)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animationController?.SetWalking(false);
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

        animationController?.SetWalking(Mathf.Abs(horizontalMove) > 0.1f);

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

        GetComponent<EnemyAttack>()?.CheckAttack();
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
        isGrounded = Physics2D.OverlapCircle(
            new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY),
            checkGroundRadius,
            groundLayer
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }
}