using UnityEngine;

public class CandleProximity : MonoBehaviour
{
    [Tooltip("Reference to the CandleRestorer component.")]
    public CandleRestorer candleRestorer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            candleRestorer.ShowRestoreButton();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            candleRestorer.HideRestoreButton();
        }
    }
}
