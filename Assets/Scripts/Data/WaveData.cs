using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnInfo
{
    [Tooltip("Enemy prefab to spawn")]
    public GameObject enemyPrefab;
    [Tooltip("How many of this enemy to spawn")]
    public int count;
    [Tooltip("Delay before starting to spawn this enemy type")]
    public float delayBeforeSpawn;
    [Tooltip("Time interval between spawns of this enemy type")]
    public float spawnInterval;
}

[CreateAssetMenu(fileName = "WaveData", menuName = "Waves/WaveData", order = 1)]
public class WaveData : ScriptableObject
{
    [Tooltip("List of enemy spawn configurations for this wave")]
    public List<EnemySpawnInfo> enemySpawns;
    [Tooltip("Optional overall duration for the wave (if not using enemy clear condition)")]
    public float waveDuration;
}
