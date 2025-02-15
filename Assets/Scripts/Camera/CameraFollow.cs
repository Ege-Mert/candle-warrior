using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    [Tooltip("The target for the camera to follow (e.g., the Player)")]
    public Transform target;
    [Tooltip("Offset relative to the target's position")]
    public Vector3 offset = new Vector3(0, 0, -10);
    [Tooltip("Smooth follow speed (used for Lerp)")]
    public float followSpeed = 5f;

    [Header("DOTween Options")]
    [Tooltip("Toggle to use DOTween for smooth movement instead of Lerp")]
    public bool useTween = false;
    [Tooltip("Duration of the DOTween movement tween")]
    public float tweenDuration = 0.1f;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 targetPos = target.position + offset;

        if (useTween)
        {
            // Use DOTween to smoothly move the camera
            // We call DOMove each frame; DOTween will adjust the tween smoothly.
            transform.DOMove(targetPos, tweenDuration).SetEase(Ease.OutSine).SetUpdate(true);
        }
        else
        {
            // Use Lerp for smooth movement.
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }
    }
}
