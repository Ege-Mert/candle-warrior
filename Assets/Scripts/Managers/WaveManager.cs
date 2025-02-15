using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Configuration")]
    [Tooltip("List of waves to spawn in order")]
    public List<WaveData> waves;
    
    [Header("Spawn Settings")]
    [Tooltip("Spawn points where enemies can appear")]
    public Transform[] spawnPoints;
    [Tooltip("Target for the enemies (e.g., the candle)")]
    public Transform candleTarget;
    
    [Header("Difficulty Scaling")]
    [Tooltip("Multiplier applied to enemy stats each wave (if applicable)")]
    public float enemyDifficultyMultiplier = 1f;

    // Keep track of the current wave and active enemies.
    private int currentWaveIndex = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("All waves completed!");
            // You could start a boss wave here or trigger a win.
            yield break;
        }

        WaveData currentWave = waves[currentWaveIndex];
        Debug.Log("Starting Wave: " + (currentWaveIndex + 1));

        // For each enemy spawn configuration in this wave…
        foreach (var spawnInfo in currentWave.enemySpawns)
        {
            yield return new WaitForSeconds(spawnInfo.delayBeforeSpawn);

            for (int i = 0; i < spawnInfo.count; i++)
            {
                SpawnEnemy(spawnInfo.enemyPrefab);
                yield return new WaitForSeconds(spawnInfo.spawnInterval);
            }
        }

        // Optionally wait for a duration or until all enemies are cleared.
        yield return new WaitForSeconds(currentWave.waveDuration);

        // Optionally wait until active enemies are cleared:
        while (activeEnemies.Count > 0)
        {
            yield return null;
        }

        currentWaveIndex++;
        // Increase difficulty if desired.
        enemyDifficultyMultiplier += 0.1f;
        // (Here you could open your market/upgrade UI.)
        Debug.Log("Wave Complete! Preparing next wave...");
        StartCoroutine(StartNextWave());
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        // Example: Apply scaling or difficulty adjustments.
        EnemyController enemyCtrl = enemy.GetComponent<EnemyController>();
        if (enemyCtrl != null)
        {
            enemyCtrl.SetTarget(candleTarget);
            enemyCtrl.ApplyDifficultyMultiplier(enemyDifficultyMultiplier);
        }

        activeEnemies.Add(enemy);
    }

    // Call this from your enemy scripts when they are destroyed or removed.
    public void RemoveEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }
}
