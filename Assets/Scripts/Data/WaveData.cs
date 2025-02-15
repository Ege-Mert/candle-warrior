using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    [Tooltip("Enemy prefab to spawn")]
    public GameObject enemyPrefab;
    [Tooltip("How many of this enemy type to spawn")]
    public int count;
    [Tooltip("Delay before starting to spawn this enemy type (seconds)")]
    public float delayBeforeSpawn;
    [Tooltip("Time interval between individual spawns of this enemy type (seconds)")]
    public float spawnInterval;
}

[CreateAssetMenu(fileName = "WaveData", menuName = "Waves/WaveData", order = 1)]
public class WaveData : ScriptableObject
{
    [Tooltip("List of enemy spawn configurations for this wave")]
    public List<EnemySpawnInfo> enemySpawns;
    
    [Tooltip("Optional overall duration to wait after spawning all enemies (seconds)")]
    public float waveDuration;
}
