using UnityEngine;
using DG.Tweening;

public class CandleManager : MonoBehaviour
{
    [Header("Candle Parts (Ensure pivots are set to Bottom)")]
    public GameObject bottomPrefab;
    public GameObject middlePrefab;
    public GameObject topPrefab;
    [Tooltip("A GameObject with a SpriteMask component for the middle segment")]
    public GameObject maskObject;

    [Header("Candle Settings")]
    [Tooltip("Maximum duration when the candle is full (seconds)")]
    public float maxDuration = 120f;
    [Tooltip("Rate at which the candle burns (seconds per second)")]
    public float decreaseRate = 1f;
    [Tooltip("Amount of duration restored when wax is added")]
    public float waxAddAmount = 24f;

    [Header("Current State")]
    public float currentDuration = 120f;

    [Header("Mask Scale Multipliers")]
    [Tooltip("Multiplier to adjust the mask's height (Y axis) relative to the middle segment.")]
    public float maskScaleMultiplierY = 1f;
    [Tooltip("Multiplier to adjust the mask's width (X axis) relative to the middle segment.")]
    public float maskScaleMultiplierX = 1f;

    [Header("Safe Zone Settings")]
    [Tooltip("Reference to the Safe Zone GameObject (should be a circular sprite)")]
    public Transform safeZoneTransform;
    [Tooltip("Minimum safe zone scale (when candle is burned out)")]
    public float safeZoneMinScale = 0.5f;
    [Tooltip("Maximum safe zone scale (when candle is full)")]
    public float safeZoneMaxScale = 2f;

    [Header("Candle Top Adjustment")]
    [Tooltip("Vertical offset to apply to the top segment (set this to the difference between the top segment's pivot and its bottom)")]
    public float topSegmentOffset = 0f;

    // Internal variables for positioning
    private Transform candleContainer;
    private GameObject bottomSegment;
    private GameObject middleSegment;
    private GameObject topSegment;
    private float bottomHeight;
    private float middleFullHeight;
    private float topHeight;

    // Flag to indicate when enemy damage tween is active.
    private bool isTakingDamage = false;

    void Awake()
    {
        if (!bottomPrefab || !middlePrefab || !topPrefab || !maskObject)
        {
            Debug.LogError("Assign all candle parts and the maskObject in the Inspector.");
            return;
        }
        if (safeZoneTransform == null)
        {
            Debug.LogError("Assign the Safe Zone Transform in the Inspector.");
            return;
        }

        SpriteRenderer srBottom = bottomPrefab.GetComponent<SpriteRenderer>();
        SpriteRenderer srMiddle = middlePrefab.GetComponent<SpriteRenderer>();
        SpriteRenderer srTop = topPrefab.GetComponent<SpriteRenderer>();

        if (srBottom != null)
            bottomHeight = srBottom.bounds.size.y;
        if (srMiddle != null)
            middleFullHeight = srMiddle.bounds.size.y;
        if (srTop != null)
            topHeight = srTop.bounds.size.y;

        currentDuration = maxDuration;
    }

    void Start()
    {
        candleContainer = new GameObject("CandleContainer").transform;
        candleContainer.SetParent(transform);
        candleContainer.localPosition = Vector3.zero;

        bottomSegment = Instantiate(bottomPrefab, candleContainer);
        bottomSegment.transform.localPosition = Vector3.zero;

        middleSegment = Instantiate(middlePrefab, candleContainer);
        middleSegment.transform.localPosition = new Vector3(0, bottomHeight, 0);

        topSegment = Instantiate(topPrefab, candleContainer);
        topSegment.transform.localPosition = new Vector3(0, bottomHeight + middleFullHeight + topSegmentOffset, 0);

        if (maskObject.transform.parent != candleContainer)
            maskObject.transform.SetParent(candleContainer);
        maskObject.transform.localPosition = middleSegment.transform.localPosition;

        SpriteRenderer middleSR = middleSegment.GetComponent<SpriteRenderer>();
        if (middleSR != null)
            middleSR.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        UpdateCandleVisual();
    }

    void Update()
    {
        if (!isTakingDamage && currentDuration > 0)
        {
            currentDuration = Mathf.Max(0, currentDuration - Time.deltaTime * decreaseRate);
            UpdateCandleVisual();
        }
        else if (currentDuration <= 0)
        {
            Debug.Log("Candle burned out. Game Over.");
        }
    }

    // Make UpdateCandleVisual public so it can be called externally.
    public void UpdateCandleVisual()
    {
        float fraction = Mathf.Clamp01(currentDuration / maxDuration);
        maskObject.transform.localScale = new Vector3(maskScaleMultiplierX, fraction * maskScaleMultiplierY, 1);
        float visibleMiddleHeight = middleFullHeight * fraction;
        topSegment.transform.localPosition = new Vector3(0, bottomHeight + visibleMiddleHeight + topSegmentOffset, 0);
        float safeZoneScale = Mathf.Lerp(safeZoneMinScale, safeZoneMaxScale, fraction);
        safeZoneTransform.localScale = new Vector3(safeZoneScale, safeZoneScale, 1);
    }

    public void RestoreDuration(float amount)
    {
        currentDuration = Mathf.Min(maxDuration, currentDuration + amount);
        UpdateCandleVisual();
    }

    public void DecreaseDuration(float amount)
    {
        isTakingDamage = true;
        float newDuration = Mathf.Max(0, currentDuration - amount);
        DOTween.To(() => currentDuration, x => { currentDuration = x; UpdateCandleVisual(); }, newDuration, 0.2f)
               .SetEase(Ease.OutQuad)
               .OnComplete(() => isTakingDamage = false);
    }

    public void AddWax()
    {
        currentDuration = Mathf.Min(maxDuration, currentDuration + waxAddAmount);
        UpdateCandleVisual();
    }
}
