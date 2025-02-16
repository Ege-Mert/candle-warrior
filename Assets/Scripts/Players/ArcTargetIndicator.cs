using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(LineRenderer))]
public class ArcTargetIndicator : MonoBehaviour
{
    [Header("Arc Settings")]
    public float arcRadius = 1.5f;
    public float arcAngle = 60f;
    public int arcSegments = 20;

    private LineRenderer lineRenderer;
    private Transform player;
    private bool hasTarget = false;
    private float currentAngle = 0f;
    private Tween arcTween;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        player = transform.parent;
    }

    public void UpdateArc(Transform target)
    {
        if (target == null)
        {
            lineRenderer.positionCount = 0;
            hasTarget = false;
            return;
        }

        hasTarget = true;
        Vector2 dir = target.position - player.position;
        currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        DrawArc();
    }

    void DrawArc()
    {
        float startAngle = currentAngle - (arcAngle / 2f);
        float endAngle = currentAngle + (arcAngle / 2f);
        lineRenderer.positionCount = arcSegments + 1;

        for (int i = 0; i <= arcSegments; i++)
        {
            float t = i / (float)arcSegments;
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            float rad = angle * Mathf.Deg2Rad;
            float x = player.position.x + arcRadius * Mathf.Cos(rad);
            float y = player.position.y + arcRadius * Mathf.Sin(rad);
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    void LateUpdate()
    {
        if (hasTarget)
            DrawArc();
    }

    public void SwayArc(float swayAmount, float duration)
    {
        if (arcTween != null && arcTween.IsActive())
            arcTween.Kill();

        float originalAngle = arcAngle;
        arcTween = DOTween.Sequence()
            .Append(DOTween.To(() => arcAngle, x => arcAngle = x, originalAngle + swayAmount, duration * 0.5f)
                .SetEase(Ease.OutQuad))
            .Append(DOTween.To(() => arcAngle, x => arcAngle = x, originalAngle, duration * 0.5f)
                .SetEase(Ease.OutQuad));
    }
}
