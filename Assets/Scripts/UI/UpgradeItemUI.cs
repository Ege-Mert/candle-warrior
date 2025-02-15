using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UpgradeItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI levelText;
    public Button upgradeButton;

    private RuntimeUpgrade runtimeUpgrade;
    private System.Action<RuntimeUpgrade> onUpgradePurchased;

    /// <summary>
    /// Initializes the UI element with the given runtime upgrade data and player currency.
    /// </summary>
    public void Initialize(RuntimeUpgrade ru, int playerCurrency, System.Action<RuntimeUpgrade> purchaseCallback)
    {
        runtimeUpgrade = ru;
        onUpgradePurchased = purchaseCallback;
        upgradeNameText.text = runtimeUpgrade.upgradeData.upgradeName;
        UpdateUI(playerCurrency);
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnClickUpgrade);
    }

    /// <summary>
    /// Updates the UI texts and button interactability based on the current wax.
    /// </summary>
    public void UpdateUI(int playerCurrency)
    {
        if (runtimeUpgrade == null) return;
        if (runtimeUpgrade.CanUpgrade())
        {
            int cost = runtimeUpgrade.GetCurrentCost();
            costText.text = "Cost: " + cost;
            levelText.text = "Level: " + runtimeUpgrade.currentLevel + "/" + runtimeUpgrade.upgradeData.maxLevel;
            upgradeButton.interactable = playerCurrency >= cost;
        }
        else
        {
            costText.text = "Maxed";
            levelText.text = "Level: " + runtimeUpgrade.currentLevel + "/" + runtimeUpgrade.upgradeData.maxLevel;
            upgradeButton.interactable = false;
        }
    }
    

    void OnClickUpgrade()
    {
        onUpgradePurchased?.Invoke(runtimeUpgrade);
    }

    /// <summary>
    /// Animates the cost text to provide feedback when an upgrade is purchased.
    /// </summary>
    public void AnimateUpgrade()
    {
        costText.transform.DOScale(1.2f, 0.2f).OnComplete(() =>
        {
            costText.transform.DOScale(1f, 0.2f);
        });
    }
}
