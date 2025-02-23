using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float HorizontalMove = 0f;
    
    private bool FacingRight = false;
    private bool FacingLeft = false;

    [Space]
    public bool isGrounded = false;
    public float checkGroundOffsetY = -1.8f;
    public float checkGroundRadius = 0.3f;
    public LayerMask groundLayer;

    public float speed = 1f;
    public float jumpForce = 2f;

    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        HorizontalMove = Input.GetAxisRaw("Horizontal") * speed;

        animator.SetFloat("HorizontalMove", Mathf.Abs(HorizontalMove));

        if (HorizontalMove < 0 && FacingRight)
        {
            Flip();
        }
        else if (HorizontalMove > 0 && !FacingRight)
        {
            Flip();
        }

        animator.SetBool("Jumping", !isGrounded);
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

        rb.velocity = new Vector2(HorizontalMove * 10f, rb.velocity.y);

        CheckGround();
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y + checkGroundOffsetY), checkGroundRadius, groundLayer);
    }
}