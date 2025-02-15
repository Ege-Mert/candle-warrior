using UnityEngine;

public enum UpgradeType
{
    MovementSpeed,
    PickupRange,
    AttackSpeed,
    Damage,
    DashLength,
    DashCooldown,
    CandleLength
}

[CreateAssetMenu(fileName = "UpgradeData", menuName = "Upgrades/UpgradeData", order = 1)]
public class UpgradeData : ScriptableObject
{
    [Header("Basic Settings")]
    public string upgradeName;
    public UpgradeType upgradeType;
    [TextArea]
    public string description;

    [Header("Level Settings")]
    [Tooltip("Maximum level this upgrade can reach. Should match the length of CostPerLevel and EffectPerLevel arrays.")]
    public int maxLevel = 10;

    [Header("Cost and Effect Arrays")]
    [Tooltip("Custom cost for each level (0-indexed). Array length should be equal to maxLevel.")]
    public int[] costPerLevel;
    [Tooltip("Custom effect value for each level (0-indexed). For example, movement speed increase or damage bonus.")]
    public float[] effectPerLevel;

    /// <summary>
    /// Returns the cost for the current level (0-indexed).
    /// </summary>
    public int GetCurrentCost(int currentLevel)
    {
        if (currentLevel < 0 || currentLevel >= costPerLevel.Length)
            return 0;
        return costPerLevel[currentLevel];
    }

    /// <summary>
    /// Returns the effect value for the current level (0-indexed).
    /// </summary>
    public float GetCurrentEffect(int currentLevel)
    {
        if (currentLevel < 0 || currentLevel >= effectPerLevel.Length)
            return 0f;
        return effectPerLevel[currentLevel];
    }

    /// <summary>
    /// Returns true if there is room to upgrade further.
    /// </summary>
    public bool CanUpgrade(int currentLevel)
    {
        return currentLevel < maxLevel;
    }
}
