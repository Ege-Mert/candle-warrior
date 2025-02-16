using UnityEngine;

public class SwooshAutoDestroy : MonoBehaviour
{
    [Tooltip("How long before destroying the swoosh (match your clip length)")]
    public float lifetime = 0.3f; // Adjust to match your 3-frame clip duration

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
