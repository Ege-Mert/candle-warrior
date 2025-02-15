using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats (Adjustable)")]
    public float speed = 2f;
    public float damage = 10f;
    public float health = 100f;
    
    // Optional: Other stats like knockback, attack cooldown, etc.

    private Transform target;
    private WaveManager waveManager;

    void Start()
    {
        // Find the WaveManager in the scene.
        waveManager = FindObjectOfType<WaveManager>();
    }

    void Update()
    {
        if (target != null)
        {
            // Move toward the candle.
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    // Called by the WaveManager when spawning the enemy.
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Scales enemy stats for difficulty (called from WaveManager).
    public void ApplyDifficultyMultiplier(float multiplier)
    {
        speed *= multiplier;
        damage *= multiplier;
        health *= multiplier;
    }

    // Call this when the enemy takes damage from the player.
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (waveManager != null)
            waveManager.RemoveEnemy(gameObject);
        // Optionally, add death animations or effects here.
        Destroy(gameObject);
    }

    // Detect collision with the candle.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Candle"))
        {
            // Try to get the CandleManager component from the collided object.
            CandleManager candle = collision.GetComponent<CandleManager>();
            if (candle != null)
            {
                // Apply damage to the candle (reduce its duration/health).
                candle.DecreaseDuration(damage);
            }

            // Remove and destroy the enemy.
            if (waveManager != null)
                waveManager.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }
}
