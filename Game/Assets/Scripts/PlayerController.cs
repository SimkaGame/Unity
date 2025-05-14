using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float checkGroundOffsetY = -1.8f;
    [SerializeField] private float checkGroundRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private PlayerAnimator playerAnimator;
    private float horizontalMove;
    private float originalSpeed;
    private bool facingRight;
    private bool isGrounded;
    private bool isTeleporting;

    public float HorizontalMove => horizontalMove;
    public bool IsGrounded => isGrounded;
    public bool FacingRight { get => facingRight; set => facingRight = value; }
    public bool IsTeleporting { get => isTeleporting; set => isTeleporting = value; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<PlayerAnimator>();
        originalSpeed = speed;
        isTeleporting = false;
    }

    private void Update()
    {
        if (!isTeleporting)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            horizontalMove = Input.GetAxisRaw("Horizontal") * speed;
        }
        else
        {
            horizontalMove = 0f;
        }

        if ((horizontalMove < 0 && FacingRight) || (horizontalMove > 0 && !FacingRight)) Flip();

        playerAnimator.UpdateAnimation(horizontalMove, isGrounded, FacingRight);
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

    public void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void SlowDown(float slowFactor) => speed = originalSpeed * slowFactor;
    public void RestoreSpeed() => speed = originalSpeed;
}