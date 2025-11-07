using UnityEngine;
using UnityEngine.UI;

public class Enemy2 : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 3f;
    [SerializeField] private float detectRange = 2.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int damagePerHit = 5;
    [SerializeField] private float maxHp = 20f;
    [SerializeField] private Image hpBar;

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float knockbackForce = 6f;
    [SerializeField] private float stunDuration = 0.4f;
    [SerializeField] private float dieAnimDuration = 1.2f;

    private float currentHp;
    private float nextAttackTime = 0f;
    private bool movingRight = false;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isStunned = false;

    private Vector3 startPos;
    private Animator anim;
    private Transform player;
    private Rigidbody2D rb;
    private Collider2D col;

    protected virtual void Start()
    {
        currentHp = maxHp;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        startPos = transform.position;

        if (hpBar != null)
            hpBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isDead || isStunned) return;

        DetectPlayer();
        if (isAttacking) return;

        Move();
    }

    void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);

        if (hit != null)
        {
            Player01Controller detectedPlayer = hit.GetComponent<Player01Controller>();

            //// Nếu player đang ẩn trong bụi thì enemy bỏ qua
            //if (detectedPlayer != null && detectedPlayer.IsHidden())
            //{
            //    isAttacking = false;
            //    player = null;
            //    return;
            //}

            player = hit.transform;
            FacePlayer();

            if (Time.time >= nextAttackTime && !isAttacking)
            {
                isAttacking = true;
                anim.SetTrigger("Attack");
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            isAttacking = false;
            movingRight = transform.localScale.x > 0;
        }
    }

    void FacePlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;
        if (player.position.x > transform.position.x && scale.x < 0)
            scale.x *= -1;
        else if (player.position.x < transform.position.x && scale.x > 0)
            scale.x *= -1;

        transform.localScale = scale;
    }

    void Move()
    {
        float leftBound = startPos.x - distance;
        float rightBound = startPos.x + distance;

        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            if (transform.position.x >= rightBound)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            if (transform.position.x <= leftBound)
            {
                movingRight = true;
                Flip();
            }
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void DealDamage()
    {
        if (isDead) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hit != null)
        {
            Player01Controller player = hit.GetComponent<Player01Controller>();
            //if (player != null && !player.IsHidden())
            {
                player.TakeHit(damagePerHit, transform.position);
            }
        }

        Invoke(nameof(ResetAttack), 0.3f);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        Debug.Log($"Enemy took {damage} damage! HP: {currentHp}/{maxHp}");

        // Hiển thị thanh máu khi bị đánh
        if (hpBar != null)
        {
            hpBar.gameObject.SetActive(true);
            hpBar.fillAmount = currentHp / maxHp;
        }

        // Hiệu ứng knockback
        if (rb != null)
        {
            isStunned = true;

            // Lấy hướng knockback ngược hướng player
            float knockDir = (player != null && player.position.x < transform.position.x) ? 1 : -1;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(knockDir * knockbackForce, 3f), ForceMode2D.Impulse);

            Invoke(nameof(EndStun), stunDuration);
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void EndStun()
    {
        isStunned = false;
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        isAttacking = false;
        isStunned = false;

        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (col != null) col.enabled = false;

        anim.ResetTrigger("Attack");
        anim.SetTrigger("EnemyDie");
        Debug.Log("Enemy Died!");

        Destroy(gameObject, dieAnimDuration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
