using UnityEngine;
using DG.Tweening;

public class WaxPickup : MonoBehaviour
{
    public int waxValue = 5;
    public float pickupRange = 3f;      // Range within which the wax is attracted to the player
    public float moveDuration = 0.5f;   // Duration of the DOTween move animation

    private Transform playerTransform;
    private bool isCollected = false;

    void Start()
    {
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }

    public void SetValue(int amount)
    {
        waxValue = amount;
    }

    void Update()
    {
        if (isCollected || playerTransform == null)
            return;

        // Check if the player is within pickup range
        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist <= pickupRange)
        {
            isCollected = true;
            // Animate the wax moving to the player
            transform.DOMove(playerTransform.position, moveDuration).SetEase(Ease.InQuad)
                     .OnComplete(Collect);
        }
    }

    void Collect()
    {
        // Immediately update player's wax via PlayerCurrencyManager
        if (PlayerCurrencyManager.Instance != null)
        {
            PlayerCurrencyManager.Instance.AddWax(waxValue);
        }
        Destroy(gameObject);
    }
}
