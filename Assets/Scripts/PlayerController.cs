using UnityEngine;
using UnityEngine.UI;

public class Player01Controller : MonoBehaviour
{
    [Header("Stealth Settings")]
    public bool isHiddenInGrass = false;
    private bool isHidden = false;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    [Header("Health Settings")]
    [SerializeField] private int maxHp = 100;
    private int currentHp;

    [Header("UI Settings")]
    [SerializeField] private Image HP;
    [SerializeField] private Text coinText; // Thêm UI hiển thị coin
    [SerializeField] private GameOverManager gameOverManager;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Economy Settings")]
    [SerializeField] private int coins = 0; // số coin hiện tại của người chơi
    [SerializeField] private int potionHeal = 50; // lượng hồi máu của potion

    private Animator animator;
    private Rigidbody2D rb;
    private bool isGrounded;
    public bool isAttacking = false;
    private bool isTakingDamage = false;
    private bool isDead = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHp = maxHp;
        if (HP != null)
            HP.fillAmount = 1f;
        UpdateCoinUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Grass"))
        {
            isHidden = true;
            Debug.Log("Player đang ẩn trong bụi cỏ!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Grass"))
        {
            isHidden = false;
            Debug.Log("Player đã ra khỏi bụi cỏ!");
        }
    }

    public bool IsHidden()
    {
        return isHidden;
    }

    void Update()
    {
        if (isDead) return;

        HandleAttack();
        HandleMovement();
        HandleJump();
        UpdateAnimation();

        // Test mua potion tạm thời (ấn phím H)
        if (Input.GetKeyDown(KeyCode.H))
        {
            BuyHealthPotion();
        }
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
        if (isDead) return;

        isDead = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false;
        Debug.Log("Player Died!");

        if (gameOverManager != null)
            gameOverManager.ShowGameOver();
        else
            Debug.LogWarning("GameOverManager chưa được gán trong Inspector!");
    }

    private void DisableController()
    {
        this.enabled = false;
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
            Enemy2 enemy2 = target.GetComponent<Enemy2>();
            if (enemy2 != null)
            {
                enemy2.TakeDamage(attackDamage);
                continue;
            }
        }
    }

    // 🪙=================== COIN & POTION SYSTEM ===================🪙
    // Coin system
    public void AddCoin(int amount)
    {
        coins += amount;
        UpdateCoinUI();
        Debug.Log("Player nhận được " + amount + " coin. Tổng: " + coins);
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            UpdateCoinUI();
            return true;
        }
        else
        {
            Debug.Log("Không đủ coin!");
            return false;
        }
    }

    public void BuyHealthPotion()
    {
        int price = 20;
        if (SpendCoins(price))
        {
            currentHp = Mathf.Min(currentHp + potionHeal, maxHp);
            if (HP != null)
                HP.fillAmount = (float)currentHp / maxHp;
            Debug.Log("Đã mua potion! Hồi 50 HP.");
        }
        else
        {
            Debug.Log("Không đủ coin để mua potion!");
        }
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = "Coins: " + coins;
    }
    // ===============================================================

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHp += amount;
        currentHp = Mathf.Min(currentHp, maxHp);
        if (HP != null)
            HP.fillAmount = (float)currentHp / maxHp;
        Debug.Log($"Player hồi {amount} máu! HP hiện tại: {currentHp}/{maxHp}");
    }
}