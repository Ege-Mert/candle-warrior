using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ArcTargetIndicator : MonoBehaviour
{
    [Header("Arc Settings")]
    public float arcRadius = 1.5f;    // How far the arc extends from the player
    public float arcAngle = 60f;      // The total angle (in degrees) of the arc
    public int arcSegments = 20;      // Number of points in the arc

    private LineRenderer lineRenderer;
    private Transform player;         // We'll assign the player's transform
    private bool hasTarget = false;
    private float currentAngle = 0f;  // We'll store the angle to the target

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        // Initially disable or clear the line
        lineRenderer.positionCount = 0;
        player = transform.parent; // If this object is a child of the Player
    }

    public void UpdateArc(Transform target)
    {
        if (target == null)
        {
            // No target -> hide the arc
            lineRenderer.positionCount = 0;
            hasTarget = false;
            return;
        }

        hasTarget = true;
        // Calculate the angle from the player to the target
        Vector2 dir = target.position - player.position;
        currentAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Draw the arc
        DrawArc();
    }

    void DrawArc()
    {
        // The arc is centered around currentAngle. So we go from
        // (currentAngle - arcAngle/2) to (currentAngle + arcAngle/2).
        float startAngle = currentAngle - (arcAngle / 2f);
        float endAngle   = currentAngle + (arcAngle / 2f);

        lineRenderer.positionCount = arcSegments + 1;

        for (int i = 0; i <= arcSegments; i++)
        {
            float t = i / (float)arcSegments;
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            float rad = angle * Mathf.Deg2Rad;

            // Convert polar coords to Cartesian
            float x = player.position.x + arcRadius * Mathf.Cos(rad);
            float y = player.position.y + arcRadius * Mathf.Sin(rad);

            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
        }
    }
}
