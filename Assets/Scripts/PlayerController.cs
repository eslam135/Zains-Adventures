using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;

    private Vector2 moveInput;
    private bool _isMoving = false;
    private Rigidbody2D rb;
    private Animator anim;
    private TocuhingChecker touchingChecker;
    private bool isFacingRight = true;
    private bool isFacingLeft = false;

    [SerializeField] private int maxJumpCount = 2;
    private int jumpCount = 0;

    public bool isMoving
    {
        get { return _isMoving; }
        private set
        {
            _isMoving = value;
            anim.SetBool("isMoving", value);
        }
    }

    public bool canMove = true;
    private bool wasGrounded = false;

    // Health system
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    [SerializeField] private List<GameObject> heartIcons; 

    [SerializeField] private GameObject respawnPoint; 
    [SerializeField] private Transform attackTransform; 
    [SerializeField] private float attackRadius = 0.5f; 
    [SerializeField] private LayerMask enemyLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();    
        touchingChecker = GetComponent<TocuhingChecker>();
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    void Update()
    {
        bool isGrounded = touchingChecker.IsGrounded;

        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
        }

        wasGrounded = isGrounded;
    }

    private void FixedUpdate()
    {
        if (canMove)
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        else
            rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(!canMove) return;
        moveInput = context.ReadValue<Vector2>();
        isMoving = moveInput != Vector2.zero;
        SetMovingDirection(moveInput);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!canMove) return; 
        if (context.performed && (touchingChecker.IsGrounded || jumpCount < maxJumpCount))
        {
            anim.Play("Jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!canMove) return; 
        if (context.performed)
        {
            anim.Play("Attack");
        }
    }

    private void SetMovingDirection(Vector2 moveInput)
    {
        if(moveInput.x > 0 && !isFacingRight)
        {
            transform.localScale *= new Vector2(-1, 1);
            isFacingRight = true;
            isFacingLeft = false;
        }
        else if(moveInput.x < 0 && !isFacingLeft) 
        {
            transform.localScale *= new Vector2(-1, 1);
            isFacingLeft = true;
            isFacingRight = false;
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
        if (!enabled)
            rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void RespawnAt(GameObject spawnPoint)
    {
        if (spawnPoint == null) return;

        transform.position = spawnPoint.transform.position;
        rb.velocity = Vector2.zero;
        jumpCount = 0;
        wasGrounded = false;
        SetMovementEnabled(true);

        currentHealth = maxHealth;
        UpdateHeartsUI();

        StartCoroutine(FlashEffect(1.5f, 0.1f));
    }

    private IEnumerator FlashEffect(float duration, float flashInterval)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < duration)
        {
            visible = !visible;
            sr.enabled = visible;
            yield return new WaitForSeconds(flashInterval);
            elapsed += flashInterval;
        }

        sr.enabled = true;  
    }

    public void TakeDamage(int amount = 1)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHeartsUI();
        StartCoroutine(FlashEffect(0.5f, 0.1f));

        if (currentHealth <= 0)
        {
            RespawnAt(respawnPoint);
        }
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < heartIcons.Count; i++)
        {
            heartIcons[i].SetActive(i < currentHealth);
        }
    }

    public void SetRespawnPoint(GameObject point)
    {
        respawnPoint = point;
    }

    public void CheckAttackHit()
    {
        Collider2D enemy = Physics2D.OverlapCircle(attackTransform.position, attackRadius, enemyLayer);
        if (enemy != null)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(1);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackTransform.position, attackRadius);
        }
    }
}
