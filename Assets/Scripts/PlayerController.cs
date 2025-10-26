using UnityEngine;
using UnityEngine.UI;

public class Player01Controller : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    [Header("Health Settings")]
    [SerializeField] private int maxHp = 100;
    private int currentHp;

    [Header("UI HP Bar")]
    [SerializeField] private Image HP;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private LayerMask enemyLayer;

    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isAttacking = false;
    private bool isTakingDamage = false;
    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHp = maxHp;

        if (HP != null)
            HP.fillAmount = 1f;
    }

    void Update()
    {
        if (isDead) return;

        HandleAttack();
        HandleMovement();
        HandleJump();
        UpdateAnimation();
    }

    private void HandleMovement()
    {
        if (isAttacking || isTakingDamage) return;

        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isAttacking && !isTakingDamage)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && !isTakingDamage)
        {
            isAttacking = true;
            animator.SetTrigger("Attack");
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isJumping = !isGrounded;

        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
    }

    public void TakeHit(int damage, Vector2 attackerPosition)
    {
        if (isDead || isTakingDamage) return;

        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        Debug.Log($"Player took {damage} damage! HP: {currentHp}/{maxHp}");

        if (HP != null)
            HP.fillAmount = (float)currentHp / maxHp;

        isTakingDamage = true;
        float knockDir = Mathf.Sign(transform.position.x - attackerPosition.x);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(knockDir * 4f, 2.5f), ForceMode2D.Impulse);

        Invoke(nameof(EndHurt), 0.4f);

        if (currentHp <= 0)
            Die();
    }

    private void EndHurt()
    {
        isTakingDamage = false;
    }

    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Debug.Log("Player Died!");
    }


    public void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (Collider2D target in hits)
        {
         
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
                continue; 
            }

           
            Boss boss = target.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(attackDamage);
                continue;
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
