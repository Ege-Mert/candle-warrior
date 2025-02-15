using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Base Movement Settings")]
    public float baseMoveSpeed = 5f;
    public float baseDashSpeed = 10f;
    public float baseDashCooldown = 1f;
    public float dashDuration = 0.2f;

    [Header("Base Combat Settings")]
    public float baseAttackDamage = 10f;
    public float baseAttackCooldown = 1f;

    [Header("Base Pickup Settings")]
    public float basePickupRange = 2f;

    [Header("Current Settings (Computed)")]
    public float moveSpeed;
    public float dashSpeed;
    public float dashCooldown;
    public float attackDamage;
    public float attackCooldown;
    public float pickupRange;

    [Header("References")]
    public SpriteRenderer spriteRenderer; // Assign via Inspector
    public Rigidbody2D rb;                // Assign via Inspector

    private PlayerInputActions inputActions;
    private Vector2 movementInput;
    private bool isDashing = false;
    private bool canDash = true;

    private void Awake()
    {
        // Singleton assignment.
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Initialize current values to base values.
        moveSpeed = baseMoveSpeed;
        dashSpeed = baseDashSpeed;
        dashCooldown = baseDashCooldown;
        attackDamage = baseAttackDamage;
        attackCooldown = baseAttackCooldown;
        pickupRange = basePickupRange;

        inputActions = new PlayerInputActions();
        inputActions.Player.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => movementInput = Vector2.zero;
        inputActions.Player.Dash.performed += ctx => AttemptDash();
    }

    private void OnEnable() { inputActions.Enable(); }
    private void OnDisable() { inputActions.Disable(); }

    private void Update()
    {
        // Flip sprite based on horizontal input.
        if (movementInput.x < 0)
            spriteRenderer.flipX = true;
        else if (movementInput.x > 0)
            spriteRenderer.flipX = false;
    }

    private void FixedUpdate()
    {
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
            rb.DOMove(rb.position + dashDirection * dashSpeed, dashDuration).OnComplete(() =>
            {
                isDashing = false;
                DOVirtual.DelayedCall(dashCooldown, () => { canDash = true; });
            });

            // Visual dash effect.
            spriteRenderer.DOColor(Color.gray, dashDuration).OnComplete(() =>
            {
                spriteRenderer.DOColor(Color.white, 0.1f);
            });
        }
    }

    // ---------------- Upgrade Methods ----------------

    /// <summary>
    /// Upgrades movement speed.
    /// Effect is additive: new moveSpeed = baseMoveSpeed + effect.
    /// </summary>
    public void UpgradeMovementSpeed(int level, float effect)
    {
        moveSpeed = baseMoveSpeed + effect;
        Debug.Log("Movement Speed upgraded to: " + moveSpeed);
    }

    /// <summary>
    /// Upgrades pickup range.
    /// </summary>
    public void UpgradePickupRange(int level, float effect)
    {
        pickupRange = basePickupRange + effect;
        Debug.Log("Pickup Range upgraded to: " + pickupRange);
    }

    /// <summary>
    /// Upgrades attack speed.
    /// For attack cooldown, a lower cooldown means faster attacks.
    /// Here, effect is negative: new attackCooldown = baseAttackCooldown + effect.
    /// </summary>
    public void UpgradeAttackSpeed(int level, float effect)
    {
        attackCooldown = Mathf.Max(0.1f, baseAttackCooldown + effect);
        Debug.Log("Attack Cooldown upgraded to: " + attackCooldown);
    }

    /// <summary>
    /// Upgrades attack damage.
    /// </summary>
    public void UpgradeDamage(int level, float effect)
    {
        attackDamage = baseAttackDamage + effect;
        Debug.Log("Attack Damage upgraded to: " + attackDamage);
    }

    /// <summary>
    /// Upgrades dash speed (or dash length).
    /// </summary>
    public void UpgradeDashSpeed(int level, float effect)
    {
        dashSpeed = baseDashSpeed + effect;
        Debug.Log("Dash Speed upgraded to: " + dashSpeed);
    }

    /// <summary>
    /// Upgrades dash cooldown.
    /// Effect is negative: new dashCooldown = baseDashCooldown + effect.
    /// </summary>
    public void UpgradeDashCooldown(int level, float effect)
    {
        dashCooldown = Mathf.Max(0.1f, baseDashCooldown + effect);
        Debug.Log("Dash Cooldown upgraded to: " + dashCooldown);
    }

    /// <summary>
    /// Upgrades candle length.
    /// This upgrade might be applied to CandleManager instead, but here's a placeholder.
    /// </summary>
    public void UpgradeCandleLength(int level, float effect)
    {
        // You might want to call CandleManager.Instance.UpgradeCandleLength(level, effect);
        Debug.Log("Candle Length upgrade applied with effect: " + effect);
    }
}
