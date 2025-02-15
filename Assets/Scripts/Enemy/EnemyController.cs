using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats (Adjustable)")]
    public float speed = 2f;
    public float damage = 10f;
    public float health = 100f;
    [Range(0f, 1f)]
    public float knockbackResistance = 0.2f; // 0 = full knockback, 1 = no knockback

    [Header("Wax Drop")]
    public GameObject waxPickupPrefab;  // The wax item to drop on death
    public int waxDropAmount = 5;       // How much wax this enemy drops

    private Transform target;
    private WaveManager waveManager;

    void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
    }

    void Update()
    {
        if (target != null)
        {
            // Move toward the candle target (set by WaveManager).
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ApplyDifficultyMultiplier(float multiplier)
    {
        speed *= multiplier;
        damage *= multiplier;
        health *= multiplier;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
        else
        {
            // Optional: flash effect, animation, or partial knockback
        }
    }

    void Die()
    {
        // Drop wax if assigned
        if (waxPickupPrefab != null)
        {
            GameObject wax = Instantiate(waxPickupPrefab, transform.position, Quaternion.identity);
            WaxPickup waxPickup = wax.GetComponent<WaxPickup>();
            if (waxPickup != null)
            {
                waxPickup.SetValue(waxDropAmount);
            }
        }

        // Remove from wave manager
        if (waveManager != null)
            waveManager.RemoveEnemy(gameObject);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If colliding with the Candle, damage it, then destroy self.
        if (collision.CompareTag("Candle"))
        {
            CandleManager candle = collision.GetComponent<CandleManager>();
            if (candle != null)
            {
                candle.DecreaseDuration(damage);
            }
            if (waveManager != null)
                waveManager.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }
}
