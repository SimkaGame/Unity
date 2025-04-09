using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float horizontalMove = 0f;
    private bool facingRight = false;

    [Space]
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private float checkGroundOffsetY = -1.8f;
    [SerializeField] private float checkGroundRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    public float speed = 1f;
    public float jumpForce = 2f;

    private PlayerAnimator playerAnimator;

    public float HorizontalMove => horizontalMove;
    public bool IsGrounded => isGrounded;
    public bool FacingRight { get => facingRight; set => facingRight = value; }

    public bool isMovingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<PlayerAnimator>();

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.sharedMaterial = new PhysicsMaterial2D { friction = 0f, bounciness = 0f };
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        horizontalMove = Input.GetAxisRaw("Horizontal") * speed;

        if (horizontalMove > 0)
        {
            isMovingRight = true;
        }
        else if (horizontalMove < 0)
        {
            isMovingRight = false;
        }

        if (horizontalMove < 0 && FacingRight)
        {
            Flip();
        }
        else if (horizontalMove > 0 && !FacingRight)
        {
            Flip();
        }

        playerAnimator.UpdateAnimation(horizontalMove, isGrounded, FacingRight);
    }

    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalMove * 10f, rb.linearVelocity.y);
        CheckGround();
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY), checkGroundRadius, groundLayer);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Collectible"))
        {
            EmeraldPickup emerald = other.GetComponent<EmeraldPickup>();
            if (emerald != null)
            {
                emerald.TriggerPickupAnimation();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY), checkGroundRadius);
    }
}
