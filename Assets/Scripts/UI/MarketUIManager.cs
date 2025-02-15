using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class MarketUIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The parent panel of the market UI")]
    public GameObject marketPanel;
    [Tooltip("TMP text element showing current wax currency")]
    public TextMeshProUGUI waxText;
    [Tooltip("Button to continue to the next wave")]
    public Button continueButton;
    [Tooltip("Parent transform for dynamically instantiated upgrade items")]
    public Transform upgradeListParent;
    [Tooltip("Prefab for a single upgrade item (should have UpgradeItemUI attached)")]
    public GameObject upgradeItemPrefab;

    [Header("Upgrade Data")]
    [Tooltip("List of all available upgrades (ScriptableObjects)")]
    public List<UpgradeData> upgradeDatas;

    // Instead of a local wax variable, we use PlayerCurrencyManager (assumed to be a singleton).
    private List<RuntimeUpgrade> runtimeUpgrades = new List<RuntimeUpgrade>();
    private List<UpgradeItemUI> upgradeItems = new List<UpgradeItemUI>();

    void Start()
    {
        marketPanel.SetActive(false);
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(ResumeGame);
        InitializeUpgrades();
    }

    /// <summary>
    /// Initializes new RuntimeUpgrade instances for each UpgradeData asset.
    /// Call this on game start/restart so upgrades reset.
    /// </summary>
    public void InitializeUpgrades()
{
    runtimeUpgrades.Clear();
    // Remove any existing UI items except the resume button.
    foreach (Transform child in upgradeListParent)
    {
        if (child.gameObject != continueButton.gameObject)
            Destroy(child.gameObject);
    }
    upgradeItems.Clear();

    foreach (UpgradeData data in upgradeDatas)
    {
        RuntimeUpgrade ru = new RuntimeUpgrade(data);
        runtimeUpgrades.Add(ru);

        GameObject go = Instantiate(upgradeItemPrefab, upgradeListParent);
        UpgradeItemUI itemUI = go.GetComponent<UpgradeItemUI>();
        if (itemUI != null)
        {
            int initialWax = (PlayerCurrencyManager.Instance != null) ? PlayerCurrencyManager.Instance.currentWax : 0;
            itemUI.Initialize(ru, initialWax, OnUpgradePurchased);
            upgradeItems.Add(itemUI);
        }
    }
}

    public void ShowMarketUI()
    {
        if (PlayerCurrencyManager.Instance != null)
        {
            UpdateWaxText();
            foreach (var item in upgradeItems)
            {
                item.UpdateUI(PlayerCurrencyManager.Instance.currentWax);
            }
        }
        marketPanel.SetActive(true);
        Time.timeScale = 0f; // Pause game.
    }

    public void ResumeGame()
    {
        marketPanel.SetActive(false);
        Time.timeScale = 1f; // Resume game.
    }

    void UpdateWaxText()
    {
        if (waxText != null && PlayerCurrencyManager.Instance != null)
            waxText.text = "Wax: " + PlayerCurrencyManager.Instance.currentWax;
    }

    // Callback from UpgradeItemUI when an upgrade is purchased.
    void OnUpgradePurchased(RuntimeUpgrade ru)
    {
        int cost = ru.GetCurrentCost();
        if (PlayerCurrencyManager.Instance != null &&
            PlayerCurrencyManager.Instance.currentWax >= cost &&
            ru.CanUpgrade())
        {
            PlayerCurrencyManager.Instance.SubtractWax(cost);
            ru.Upgrade();
            UpdateWaxText();
        }
        else
        {
            Debug.Log("Not enough wax to upgrade " + ru.upgradeData.upgradeName);
        }

        // Refresh all UI items.
        foreach (var item in upgradeItems)
        {
            item.UpdateUI(PlayerCurrencyManager.Instance.currentWax);
        }

        // Optionally, apply the upgrade effect to your game systems here.
        ApplyUpgrade(ru);
    }

   void ApplyUpgrade(RuntimeUpgrade ru)
    {
        switch (ru.upgradeData.upgradeType)
        {
            case UpgradeType.MovementSpeed:
                PlayerController.Instance.UpgradeMovementSpeed(ru.currentLevel, ru.upgradeData.GetCurrentEffect(ru.currentLevel));
                break;
            case UpgradeType.PickupRange:
                PlayerController.Instance.UpgradePickupRange(ru.currentLevel, ru.upgradeData.GetCurrentEffect(ru.currentLevel));
                break;
            case UpgradeType.AttackSpeed:
                PlayerController.Instance.UpgradeAttackSpeed(ru.currentLevel, ru.upgradeData.GetCurrentEffect(ru.currentLevel));
                break;
            case UpgradeType.Damage:
                PlayerController.Instance.UpgradeDamage(ru.currentLevel, ru.upgradeData.GetCurrentEffect(ru.currentLevel));
                break;
            case UpgradeType.DashLength:
                // Here, we use UpgradeDashSpeed as a proxy for dash length if desired.
                PlayerController.Instance.UpgradeDashSpeed(ru.currentLevel, ru.upgradeData.GetCurrentEffect(ru.currentLevel));
                break;
            case UpgradeType.DashCooldown:
                PlayerController.Instance.UpgradeDashCooldown(ru.currentLevel, ru.upgradeData.GetCurrentEffect(ru.currentLevel));
                break;
            case UpgradeType.CandleLength:
                // You can either apply this to the player (if it affects some player property) or call a method on CandleManager.
                PlayerController.Instance.UpgradeCandleLength(ru.currentLevel, ru.upgradeData.GetCurrentEffect(ru.currentLevel));
                break;
        }
    }
}
