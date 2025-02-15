using UnityEngine;
using DG.Tweening;

public class CandleManager : MonoBehaviour
{
    [Header("Candle Parts (Make sure pivots are set to Bottom)")]
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

    private Transform candleContainer;
    private GameObject bottomSegment;
    private GameObject middleSegment;
    private GameObject topSegment;
    private float bottomHeight;
    private float middleFullHeight;

    void Awake()
    {
        if (!bottomPrefab || !middlePrefab || !topPrefab || !maskObject)
        {
            Debug.LogError("Assign all candle parts and the maskObject in the Inspector.");
            return;
        }

        SpriteRenderer srBottom = bottomPrefab.GetComponent<SpriteRenderer>();
        SpriteRenderer srMiddle = middlePrefab.GetComponent<SpriteRenderer>();

        if (srBottom != null)
            bottomHeight = srBottom.bounds.size.y;
        if (srMiddle != null)
            middleFullHeight = srMiddle.bounds.size.y;

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
        topSegment.transform.localPosition = new Vector3(0, bottomHeight + middleFullHeight, 0);

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
        if (currentDuration > 0)
        {
            currentDuration = Mathf.Max(0, currentDuration - Time.deltaTime * decreaseRate);
            UpdateCandleVisual();
        }
        else
        {
            Debug.Log("Candle burned out. Game Over.");
            // Insert game over logic here.
        }
    }

    void UpdateCandleVisual()
    {
        // Calculate the fraction of the middle segment that should be visible.
        float fraction = Mathf.Clamp01(currentDuration / maxDuration);
        
        // Apply both X and Y multipliers.
        maskObject.transform.localScale = new Vector3(maskScaleMultiplierX, fraction * maskScaleMultiplierY, 1);
        
        float visibleMiddleHeight = middleFullHeight * fraction;
        topSegment.transform.localPosition = new Vector3(0, bottomHeight + visibleMiddleHeight, 0);
    }

    // Call this method to add wax to the candle.
    public void AddWax()
    {
        currentDuration = Mathf.Min(maxDuration, currentDuration + waxAddAmount);
        UpdateCandleVisual();
    }

    // New method: Call this to decrease the candle's duration when an enemy deals damage.
    public void DecreaseDuration(float amount)
    {
        currentDuration = Mathf.Max(0, currentDuration - amount);
        UpdateCandleVisual();
    }
}
