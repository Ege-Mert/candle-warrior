using UnityEngine;
using DG.Tweening; // For knockback animations

public class PlayerCombat : MonoBehaviour
{
    [Header("Ranges")]
    public float targetingRange = 5f; // Range for finding an enemy to target visually
    public float attackRange = 2f;    // Actual range to deal damage

    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public LayerMask enemyLayer;
    [SerializeField]private float baseKnockbackDistance = 0.5f;


    // References
    public Transform currentTarget;  
    public ArcTargetIndicator arcIndicator; // If you have an arc script for visuals

    private float attackTimer = 0f;

    void Update()
    {
        attackTimer += Time.deltaTime;

        // 1. Find nearest enemy in the larger "targetingRange" (for visuals).
        currentTarget = FindNearestEnemy(targetingRange);

        // 2. Update any arc/visual indicator to point toward that target.
        if (arcIndicator != null)
            arcIndicator.UpdateArc(currentTarget);

        // 3. Attack if the enemy is within the smaller "attackRange" and cooldown is ready.
        if (currentTarget != null)
        {
            float dist = Vector2.Distance(transform.position, currentTarget.position);
            if (dist <= attackRange && attackTimer >= attackCooldown)
            {
                Attack(currentTarget);
                attackTimer = 0f;
            }
        }
    }

    Transform FindNearestEnemy(float range)
    {
        // OverlapCircleAll finds all enemies in 'range' distance.
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        if (hits.Length == 0) return null;

        Transform nearest = null;
        float shortestDist = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                nearest = hit.transform;
            }
        }
        return nearest;
    }

    void Attack(Transform enemy)
    {
        // Deal damage
        EnemyController enemyCtrl = enemy.GetComponent<EnemyController>();
        if (enemyCtrl != null)
        {
            enemyCtrl.TakeDamage(attackDamage);

            // Optional knockback effect (uses enemy's knockbackResistance)
            Vector2 direction = (enemy.position - transform.position).normalized;

            // The less knockbackResistance, the more knockback
            float actualKnockback = baseKnockbackDistance * (1f - enemyCtrl.knockbackResistance);

            enemy.DOMove(enemy.position + (Vector3)(direction * actualKnockback), 0.2f)
                 .SetEase(Ease.OutQuad);
        }
    }

    // Debug visuals in the Scene view
    void OnDrawGizmosSelected()
    {
        // Yellow = targeting range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetingRange);

        // Red = attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
