using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private int damage = 1;
    [SerializeField] private Vector2 attackBoxSize = new Vector2(4f, 4f);
    [SerializeField] private Vector2 attackBoxOffset = new Vector2(1f, 0f);
    [SerializeField] private LayerMask playerLayer;

    private EnemyMovement movement;
    private EnemyAnimationController animationController;
    private float lastAttackTime;

    private void Start()
    {
        movement = GetComponent<EnemyMovement>();
        animationController = GetComponent<EnemyAnimationController>();
    }

    public void CheckAttack()
    {
        var attackPos = (Vector2)transform.position + 
            (movement.FacingRight ? attackBoxOffset : new Vector2(-attackBoxOffset.x, attackBoxOffset.y));
        var hit = Physics2D.OverlapBox(attackPos, attackBoxSize, 0f, playerLayer);

        if (hit && hit.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
        {
            var playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth && playerHealth.CurrentHealth > 0)
            {
                playerHealth.TakeDamage(damage, false);
                lastAttackTime = Time.time;
                animationController?.TriggerAttack();
            }
        }
    }

    private void OnDrawGizmos()
    {
        bool facingRight = movement != null ? movement.FacingRight : true;
        var attackPos = (Vector2)transform.position + 
            (facingRight ? attackBoxOffset : new Vector2(-attackBoxOffset.x, attackBoxOffset.y));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPos, attackBoxSize);
    }
}