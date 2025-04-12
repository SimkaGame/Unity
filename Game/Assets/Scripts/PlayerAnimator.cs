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

    public void UpdateAnimation(float horizontalMove, bool isGrounded, bool facingRight)
    {
        animator.SetFloat("HorizontalMove", Mathf.Abs(horizontalMove));
        animator.SetBool("Jumping", !isGrounded);
    }

    public void SetShooting(bool isShooting)
    {
        animator.SetBool("IsShooting", isShooting);
    }
}