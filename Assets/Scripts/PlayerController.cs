using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("References")]
    public SpriteRenderer spriteRenderer; // Assign via Inspector
    public Rigidbody2D rb;              // Assign via Inspector

    private PlayerInputActions inputActions;
    private Vector2 movementInput;
    private bool isDashing = false;
    private bool canDash = true;

    private void Awake()
    {
        // Ensure components are assigned.
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize the input actions asset.
        inputActions = new PlayerInputActions();

        // Set up movement callbacks.
        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => movementInput = Vector2.zero;

        // Set up dash callback.
        inputActions.Player.Dash.performed += ctx => AttemptDash();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        // Flip sprite for left/right movement based on horizontal input.
        if (movementInput.x < 0)
            spriteRenderer.flipX = true;
        else if (movementInput.x > 0)
            spriteRenderer.flipX = false;
    }

    private void FixedUpdate()
    {
        // If dashing, skip normal movement.
        if (isDashing)
            return;

        Vector2 newPosition = rb.position + movementInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void AttemptDash()
    {
        if (canDash && movementInput != Vector2.zero)
        {
            isDashing = true;
            canDash = false;

            Vector2 dashDirection = movementInput.normalized;
            // Use DOTween to animate the dash movement.
            rb.DOMove(rb.position + dashDirection * dashSpeed, dashDuration).OnComplete(() =>
            {
                isDashing = false;
                // Set a cooldown for the dash using DOTween's delayed call.
                DOVirtual.DelayedCall(dashCooldown, () => { canDash = true; });
            });

            // Optional: Visual dash effect (e.g., flashing color)
            spriteRenderer.DOColor(Color.gray, dashDuration).OnComplete(() =>
            {
                spriteRenderer.DOColor(Color.white, 0.1f);
            });
        }
    }
}
