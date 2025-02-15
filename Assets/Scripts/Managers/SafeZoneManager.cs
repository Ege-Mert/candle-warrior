using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SafeZoneManager : MonoBehaviour
{
    public Transform playerTransform;
    public Transform safeZoneTransform;
    public Image overlayImage;
    public float outOfZoneThreshold = 3f;

    private float outOfZoneTimer = 0f;
    private bool isGameOver = false;

    void Update()
    {
        if (isGameOver) return;

        ProcessSafeZone();
        UpdateOverlayTween();
        if (outOfZoneTimer >= outOfZoneThreshold) TriggerGameOver();
    }

    void ProcessSafeZone()
    {
        float safeZoneRadius = GetSafeZoneRadius();
        bool isInside = Vector2.Distance(playerTransform.position, safeZoneTransform.position) <= safeZoneRadius;
        if (!isInside)
            outOfZoneTimer += Time.deltaTime;
        else
            outOfZoneTimer = Mathf.Max(0f, outOfZoneTimer - Time.deltaTime * 2);
    }

    float GetSafeZoneRadius()
    {
        return safeZoneTransform.lossyScale.x * 0.5f;
    }

    void UpdateOverlayTween()
    {
        float targetAlpha = Mathf.Clamp01(outOfZoneTimer / outOfZoneThreshold);
        overlayImage.DOFade(targetAlpha, 0.1f);
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over: Player out of safe zone too long");
        // Insert game over handling here
    }
}
