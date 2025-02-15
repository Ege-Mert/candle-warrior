using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CandleRestorer : MonoBehaviour
{
    [Tooltip("Reference to the CandleManager component.")]
    public CandleManager candleManager;

    [Tooltip("UI Button that appears on the candle to restore duration.")]
    public Button restoreButton;

    [Tooltip("Cost per second of duration restoration (e.g., 2 wax per 1 second restored).")]
    public float restoreCostPerSecond = 2f;

    private void Start()
    {
        if (restoreButton != null)
        {
            restoreButton.gameObject.SetActive(false);
            restoreButton.onClick.RemoveAllListeners();
            restoreButton.onClick.AddListener(RestoreCandle);
        }
    }

    public void ShowRestoreButton()
    {
        if (restoreButton != null)
            restoreButton.gameObject.SetActive(true);
    }

    public void HideRestoreButton()
    {
        if (restoreButton != null)
            restoreButton.gameObject.SetActive(false);
    }

    public void RestoreCandle()
    {
        if (candleManager == null || PlayerCurrencyManager.Instance == null)
            return;

        float missingDuration = candleManager.maxDuration - candleManager.currentDuration;
        if (missingDuration <= 0)
        {
            Debug.Log("Candle is already full.");
            return;
        }

        float fullCost = missingDuration * restoreCostPerSecond;
        int playerWax = PlayerCurrencyManager.Instance.currentWax;
        float durationToRestore = 0f;

        if (playerWax >= fullCost)
        {
            durationToRestore = missingDuration;
            PlayerCurrencyManager.Instance.SubtractWax((int)fullCost);
        }
        else
        {
            durationToRestore = playerWax / restoreCostPerSecond;
            PlayerCurrencyManager.Instance.SubtractWax(playerWax);
        }

        float newDuration = candleManager.currentDuration + durationToRestore;
        DOTween.To(() => candleManager.currentDuration, x => { candleManager.currentDuration = x; candleManager.UpdateCandleVisual(); }, newDuration, 0.3f)
               .SetEase(Ease.OutQuad);
        Debug.Log("Restored candle duration by: " + durationToRestore + " seconds.");
    }
}
