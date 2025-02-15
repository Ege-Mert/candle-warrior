using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Wave Configuration")]
    [Tooltip("List of waves to spawn in order")]
    public List<WaveData> waves;
    
    [Header("Spawn Settings")]
    [Tooltip("Array of spawn points for enemies")]
    public Transform[] spawnPoints;
    [Tooltip("Target that enemies move toward (e.g., the Candle)")]
    public Transform candleTarget;
    
    [Header("Difficulty Scaling")]
    [Tooltip("Multiplier applied to enemy stats each wave")]
    public float enemyDifficultyMultiplier = 1f;

    private int currentWaveIndex = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        if (waves.Count > 0)
            StartCoroutine(StartNextWave());
        else
            Debug.LogError("No waves assigned in the WaveManager.");
    }

    IEnumerator StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("All waves completed!");
            yield break;
        }

        WaveData currentWave = waves[currentWaveIndex];
        Debug.Log("Starting Wave: " + (currentWaveIndex + 1));

        // Spawn enemies as defined in the current wave.
        foreach (var spawnInfo in currentWave.enemySpawns)
        {
            yield return new WaitForSeconds(spawnInfo.delayBeforeSpawn);

            for (int i = 0; i < spawnInfo.count; i++)
            {
                SpawnEnemy(spawnInfo.enemyPrefab);
                yield return new WaitForSeconds(spawnInfo.spawnInterval);
            }
        }

        yield return new WaitForSeconds(currentWave.waveDuration);

        // Wait until all active enemies are cleared.
        while (activeEnemies.Count > 0)
        {
            yield return null;
        }

        // Wait an additional 1 second after the last enemy is cleared.
        yield return new WaitForSeconds(1f);

        currentWaveIndex++;
        enemyDifficultyMultiplier += 0.1f; // Increase difficulty for next wave.
        Debug.Log("Wave Complete! Preparing next wave...");

        // Show the market UI before starting the next wave.
        MarketUIManager marketUI = FindObjectOfType<MarketUIManager>();
        if (marketUI != null)
        {
            marketUI.ShowMarketUI();  // No parameter now
            // Wait until the market panel is hidden (player pressed Continue).
            yield return new WaitUntil(() => marketUI.marketPanel.activeSelf == false);
        }

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

        EnemyController enemyCtrl = enemy.GetComponent<EnemyController>();
        if (enemyCtrl != null)
        {
            enemyCtrl.SetTarget(candleTarget);
            enemyCtrl.ApplyDifficultyMultiplier(enemyDifficultyMultiplier);
        }
        activeEnemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);
    }
}
