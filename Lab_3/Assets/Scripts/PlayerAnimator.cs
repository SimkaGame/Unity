using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerJump()
    {
        animator.SetTrigger("JumpRun");
    }

    public void UpdateAnimation(float horizontalMove, bool isGrounded, bool facingRight)
    {
        animator.SetFloat("HorizontalMove", Mathf.Abs(horizontalMove));
        animator.SetBool("Jumping", !isGrounded);
    }
}
