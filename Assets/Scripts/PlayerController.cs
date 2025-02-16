using System.Collections;
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

    [Header("Animation")] 
    public Animator animator;
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float afterImageLifetime = 0.5f;
    [SerializeField] private float afterImageSpawnRate = 0.05f;

    private PlayerInputActions inputActions;
    private Vector2 movementInput;
    private bool isDashing = false;
    private bool canDash = true;

    public AudioSource audioSource;
    public AudioClip audioClip;

    private void Awake()
    {
        // Singleton assignment.
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

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

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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
        
        bool isMoving = movementInput != Vector2.zero;
        animator.SetBool("isMoving", isMoving);

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
            PlaySound();
            StartCoroutine(CreateAfterImages());
        }
    }

    // ---------------- Upgrade Methods ----------------

    /// <summary>
    /// Upgrades movement speed.
    /// New moveSpeed = baseMoveSpeed + effect.
    /// </summary>
    public void UpgradeMovementSpeed(int level, float effect)
    {
        moveSpeed = baseMoveSpeed + effect;
        Debug.Log("Movement Speed upgraded to: " + moveSpeed);
    }

    /// <summary>
    /// Upgrades pickup range.
    /// New pickupRange = basePickupRange + effect.
    /// </summary>
    public void UpgradePickupRange(int level, float effect)
    {
        pickupRange = basePickupRange + effect;
        Debug.Log("Pickup Range upgraded to: " + pickupRange);
    }

    /// <summary>
    /// Upgrades attack speed.
    /// New attackCooldown = baseAttackCooldown + effect (effect should be negative).
    /// </summary>
    public void UpgradeAttackSpeed(int level, float effect)
    {
        attackCooldown = Mathf.Max(0.1f, baseAttackCooldown + effect);
        Debug.Log("Attack Cooldown upgraded to: " + attackCooldown);
    }

    /// <summary>
    /// Upgrades attack damage.
    /// New attackDamage = baseAttackDamage + effect.
    /// </summary>
    public void UpgradeDamage(int level, float effect)
    {
        attackDamage = baseAttackDamage + effect;
        Debug.Log("Attack Damage upgraded to: " + attackDamage);
    }

    /// <summary>
    /// Upgrades dash speed (or dash length).
    /// New dashSpeed = baseDashSpeed + effect.
    /// </summary>
    public void UpgradeDashSpeed(int level, float effect)
    {
        dashSpeed = baseDashSpeed + effect;
        Debug.Log("Dash Speed upgraded to: " + dashSpeed);
    }

    /// <summary>
    /// Upgrades dash cooldown.
    /// New dashCooldown = baseDashCooldown + effect (effect should be negative).
    /// </summary>
    public void UpgradeDashCooldown(int level, float effect)
    {
        dashCooldown = Mathf.Max(0.1f, baseDashCooldown + effect);
        Debug.Log("Dash Cooldown upgraded to: " + dashCooldown);
    }

    /// <summary>
    /// (CandleLength upgrade is removed from the market.)
    /// </summary>
    public void UpgradeCandleLength(int level, float effect)
    {
        // Not used in Market upgrades.
        Debug.Log("Candle Length upgrade is handled separately.");
    }
    // Animation Methots
    private IEnumerator CreateAfterImages()
    {
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            CreateAfterImage();
            yield return new WaitForSeconds(afterImageSpawnRate);
            elapsedTime += afterImageSpawnRate;
        }
    }

    private void CreateAfterImage()
    {
        GameObject afterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);
        SpriteRenderer afterImageSR = afterImage.GetComponent<SpriteRenderer>();

        afterImageSR.sprite = spriteRenderer.sprite;  // Mevcut sprite'ı kopyala
        afterImageSR.flipX = spriteRenderer.flipX;    // Flip durumunu koru
        afterImageSR.color = new Color(1f, 1f, 1f, 0.5f); // Yarı saydam başlat

        // Gölgeyi zamanla yok et
        afterImageSR.DOFade(0f, afterImageLifetime).OnComplete(() => Destroy(afterImage));
    }

    void PlaySound()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
