using ArabicSupport;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public LayerMask wallLayer;
    public Animator animator;
    public TMP_Text winningText;

    private bool movingRight = true;
    private float lastAttackTime = -1f;
    private float attackCooldown = 3f; 

    [SerializeField] private int maxHealth = 3;
    private int currentHealth;
    [SerializeField] private List<GameObject> heartIcons; 
    [SerializeField] private float heartDisplayRange = 5f; 
    [SerializeField] private GameObject deathSprite; 

    private bool isDead = false;

    [SerializeField] private Transform attackTransform; 
    [SerializeField] private float attackRadius = 0.5f; 

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
        SetHeartsActive(false);
        if (deathSprite != null)
            deathSprite.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        Patrol();
        DetectPlayer();
        HandleHeartDisplay();
    }

    void Patrol()
    {
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime * (movingRight ? 1 : -1));

        bool hittingWall = Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer);

        bool atEdge = !Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        if (hittingWall)
        {
            Flip();
        }
        else if (atEdge)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void DetectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, 1.2f, playerLayer);
        if (player != null)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                animator.Play("BanditAttack");
                lastAttackTime = Time.time;
            }
        }
    }

    void HandleHeartDisplay()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, heartDisplayRange, playerLayer);
        SetHeartsActive(player != null);
    }

    void SetHeartsActive(bool active)
    {
        foreach (var heart in heartIcons)
        {
            heart.SetActive(active);
        }
    }

    public void TakeDamage(int amount = 1)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHeartsUI();

        StartCoroutine(FlashEffect(0.5f, 0.1f));

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHeartsUI()
    {
        for (int i = 0; i < heartIcons.Count; i++)
        {
            SpriteRenderer sr = heartIcons[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = i < currentHealth ? 1f : 0f;
                sr.color = c;
            }
        }
    }

    private void Die()
    {
        isDead = true;
        SetHeartsActive(false);
        if (deathSprite != null)
            deathSprite.SetActive(true);
        Destroy(gameObject);
        winningText.text = ArabicFixer.Fix("لقد نجحت في الحصول على اول تمثال!!", showTashkeel: true, useHinduNumbers: true);
        winningText.color = Color.green;
        StartCoroutine(WaitAndCloseGame());
    }

    private IEnumerator WaitAndCloseGame()
    {
        yield return new WaitForSeconds(2f);
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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

    // Call this from the attack animation event
    public void CheckAttackHit()
    {
        Collider2D player = Physics2D.OverlapCircle(attackTransform.position, attackRadius, playerLayer);
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(1);
            }
        }
    }

    // Optional: visualize attack range in editor
    private void OnDrawGizmosSelected()
    {
        if (attackTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackTransform.position, attackRadius);
        }
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, 0.1f);
        }
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, 0.1f);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1.2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, heartDisplayRange);
    }
}
