using UnityEngine;

[System.Serializable]
public class RuntimeUpgrade
{
    public UpgradeData upgradeData;  // Reference to the static data
    public int currentLevel;         // Runtime level (resets each game)

    public RuntimeUpgrade(UpgradeData data)
    {
        upgradeData = data;
        currentLevel = 0; // Start at level 0 on a new game.
    }

    public int GetCurrentCost()
    {
        if (!CanUpgrade())
            return 0;
        return upgradeData.GetCurrentCost(currentLevel);
    }

    public bool CanUpgrade()
    {
        return upgradeData.CanUpgrade(currentLevel);
    }

    public float GetCurrentEffect()
    {
        return upgradeData.GetCurrentEffect(currentLevel);
    }

    public bool Upgrade()
    {
        if (!CanUpgrade())
            return false;
        currentLevel++;
        return true;
    }
}
