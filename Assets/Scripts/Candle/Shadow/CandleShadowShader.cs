using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class CandleShadowShader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CandleManager candleManager;
    [SerializeField] private Material candleShadowMaterial;

    [Header("Shadow Settings")]
    [SerializeField] private Color shadowColor = Color.black;
    [SerializeField, Range(0f, 1f)] private float baseOpacity = 0.8f;
    [SerializeField, Range(0f, 1f)] private float fadeStart = 0.3f;
    [SerializeField, Range(0f, 1f)] private float fadeEnd = 0.8f;
    
    [Header("Flicker Settings")]
    [SerializeField, Range(1f, 10f)] private float flickerSpeed = 5f;
    [SerializeField, Range(0f, 0.5f)] private float flickerIntensity = 0.1f;
    [SerializeField] private bool increasedFlickerOnLowDuration = true;
    [SerializeField] private float lowDurationThreshold = 0.3f;

    private Material instancedMaterial;
    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int OpacityId = Shader.PropertyToID("_Opacity");
    private static readonly int FadeStartId = Shader.PropertyToID("_FadeStart");
    private static readonly int FadeEndId = Shader.PropertyToID("_FadeEnd");
    private static readonly int FlickerSpeedId = Shader.PropertyToID("_FlickerSpeed");
    private static readonly int FlickerIntensityId = Shader.PropertyToID("_FlickerIntensity");

    private void Awake()
    {
        instancedMaterial = new Material(candleShadowMaterial);
        GetComponent<SpriteRenderer>().material = instancedMaterial;
    }

    private void Start()
    {
        UpdateShaderProperties();

        if (candleManager != null)
        {
            candleManager.onDurationChanged.AddListener(UpdateFlickerBasedOnDuration);
        }
        else
        {
            Debug.LogWarning("CandleManager not assigned to CandleShadowShader!");
        }
    }

    private void UpdateShaderProperties()
    {
        if (instancedMaterial != null)
        {
            instancedMaterial.SetColor(ColorId, shadowColor);
            instancedMaterial.SetFloat(OpacityId, baseOpacity);
            instancedMaterial.SetFloat(FadeStartId, fadeStart);
            instancedMaterial.SetFloat(FadeEndId, fadeEnd);
            instancedMaterial.SetFloat(FlickerSpeedId, flickerSpeed);
            instancedMaterial.SetFloat(FlickerIntensityId, flickerIntensity);
        }
    }

    private void UpdateFlickerBasedOnDuration(float durationPercentage)
    {
        if (!increasedFlickerOnLowDuration) return;

        if (durationPercentage <= lowDurationThreshold)
        {
            float normalizedPercentage = durationPercentage / lowDurationThreshold;
            float newIntensity = Mathf.Lerp(flickerIntensity, 0.5f, 1f - normalizedPercentage);
            SetFlickerIntensity(newIntensity);
        }
        else
        {
            SetFlickerIntensity(flickerIntensity);
        }
    }

    public void SetFlickerIntensity(float intensity)
    {
        if (instancedMaterial != null)
        {
            instancedMaterial.SetFloat(FlickerIntensityId, intensity);
        }
    }

    private void OnValidate()
    {
        UpdateShaderProperties();
    }

    private void OnDestroy()
    {
        if (instancedMaterial != null)
        {
            Destroy(instancedMaterial);
        }

        if (candleManager != null)
        {
            candleManager.onDurationChanged.RemoveListener(UpdateFlickerBasedOnDuration);
        }
    }
}