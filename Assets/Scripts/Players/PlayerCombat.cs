using System;
using UnityEngine;
using DG.Tweening;

public class PlayerCombat : MonoBehaviour
{
    [Header("Ranges")]
    public float targetingRange = 5f; // Visual targeting range (for arc indicator)
    public float attackRange = 2f;    // Actual range for dealing damage

    [Header("Attack Settings")]
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public LayerMask enemyLayer;
    public float baseKnockbackDistance = 0.5f;

    [Header("Swoosh Settings")]
    [Tooltip("Prefab of the swoosh (3-frame non-looping animation)")]
    public GameObject swooshPrefab;
    [Tooltip("Extra offset added to arcIndicator.arcRadius so the swoosh spawns further out")]
    public float swooshExtraOffset = 0.5f;
    [Tooltip("Additional rotation (in degrees) applied to the swoosh over its lifetime")]
    public float swooshAdditionalRotation = 90f; // Adjust in Inspector as needed

    [Header("References")]
    [Tooltip("Reference to the ArcTargetIndicator component")]
    public ArcTargetIndicator arcIndicator;

    private float attackTimer = 0f;
    private Transform currentTarget;
    public AudioSource audioSource;
    public AudioClip audioClip;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        // 1. Find the nearest enemy within targetingRange.
        currentTarget = FindNearestEnemy(targetingRange);

        // 2. Update the arc indicator so it points toward the enemy.
        if (arcIndicator != null)
            arcIndicator.UpdateArc(currentTarget);

        // 3. If an enemy is within attackRange and the cooldown is ready, attack.
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        if (hits.Length == 0)
            return null;

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
        // 1. Spawn the swoosh at a position computed from the arc indicator.
        if (swooshPrefab != null && arcIndicator != null)
        {
            // Calculate the direction from the player to the enemy.
            Vector3 direction = (enemy.position - transform.position).normalized;
            // Spawn position: from the player plus (arcRadius + extra offset) along the direction.
            Vector3 spawnPos = transform.position + direction * (arcIndicator.arcRadius + swooshExtraOffset);
            GameObject swoosh = Instantiate(swooshPrefab, spawnPos, Quaternion.identity);

            // Rotate the swoosh to face the enemy.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            swoosh.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Adjust the animation speed based on attackCooldown (faster attacks = faster animation).
            Animator swooshAnim = swoosh.GetComponent<Animator>();
            if (swooshAnim != null)
            {
                float animSpeed = 1f / attackCooldown; // e.g., if attackCooldown=0.5, speed=2.0
                swooshAnim.speed = animSpeed;
            }

            // Apply additional rotation tween so the swoosh rotates extra degrees over its lifetime.
            // We use the lifetime from SwooshAutoDestroy (or a default value).
            SwooshAutoDestroy autoDestroy = swoosh.GetComponent<SwooshAutoDestroy>();
            float lifetime = (autoDestroy != null) ? autoDestroy.lifetime : 0.3f;
            swoosh.transform.DORotate(swoosh.transform.eulerAngles + new Vector3(0f, 0f, swooshAdditionalRotation),
                                      lifetime, RotateMode.FastBeyond360)
                  .SetEase(Ease.OutQuad);
        }

        // 2. Deal damage to the enemy.
        EnemyController enemyCtrl = enemy.GetComponent<EnemyController>();
        if (enemyCtrl != null)
        {
            enemyCtrl.TakeDamage(attackDamage);

            // Optional knockback effect.
            Vector2 direction = (enemy.position - transform.position).normalized;
            float actualKnockback = baseKnockbackDistance * (1f - enemyCtrl.knockbackResistance);
            enemy.DOMove(enemy.position + (Vector3)(direction * actualKnockback), 0.2f)
                 .SetEase(Ease.OutQuad);
        }
        //müzik
        audioSource.PlayOneShot(audioClip);
    }

    void OnDrawGizmosSelected()
    {
        // Yellow sphere for targetingRange.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetingRange);

        // Red sphere for attackRange.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
