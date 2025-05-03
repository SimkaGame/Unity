using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateAnimation(float horizontalMove, bool isGrounded, bool facingRight)
    {
        animator.SetFloat("HorizontalMove", Mathf.Abs(horizontalMove));
        animator.SetBool("Jumping", !isGrounded);
    }

    public void SetShooting(bool isShooting) => animator.SetBool("IsShooting", isShooting);
}