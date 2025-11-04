using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 3f;
    [SerializeField] private float detectRange = 5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Vector2 attackDelayRange = new Vector2(1f, 2f);
    [SerializeField] private int damagePerHit = 10;
    [SerializeField] private float attackRange = 2f;

    [Header("Combat Settings")]
    [SerializeField] private float maxHp = 200f;
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float stunDuration = 0.5f;

    private float currentHp;
    private Vector3 startPos;
    private bool movingRight = false;
    private bool isAttacking = false;
    private bool isPreparingAttack = false;
    private bool hasAttackedOnce = false;
    private bool isDead = false;
    private bool isStunned = false;

    private Animator anim;
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        startPos = transform.position;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHp = maxHp;

        if (!movingRight)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -1;
            transform.localScale = scale;
        }
    }

    void Update()
    {
        if (isDead || isStunned) return;

        if (isAttacking || isPreparingAttack) return;

        DetectPlayer();
        Move();
    }

    void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);

        if (hit != null)
        {
            player = hit.transform;

            if (!isAttacking && !isPreparingAttack)
                FacePlayer();

            if (!hasAttackedOnce)
            {
                hasAttackedOnce = true;
                StartCoroutine(ImmediateAttack());
            }
            else if (!isAttacking && !isPreparingAttack)
            {
                StartCoroutine(PrepareAttack());
            }
        }
        else
        {
            isAttacking = false;
            isPreparingAttack = false;
            hasAttackedOnce = false;
            movingRight = transform.localScale.x > 0;
        }
    }

    IEnumerator ImmediateAttack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    IEnumerator PrepareAttack()
    {
        isPreparingAttack = true;

        float waitTime = Random.Range(attackDelayRange.x, attackDelayRange.y);
        yield return new WaitForSeconds(waitTime);

        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
        if (hit != null)
        {
            isAttacking = true;
            anim.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(1f);
        isAttacking = false;
        isPreparingAttack = false;
    }

 
    public void DealDamage()
    {
        if (isDead) return;

        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hit != null)
        {
            Player01Controller player = hit.GetComponent<Player01Controller>();
            if (player != null)
            {
                player.TakeHit(damagePerHit, transform.position);
            }
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

  
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);
        Debug.Log($"Boss took {damage} damage! HP: {currentHp}/{maxHp}");

      
        if (rb != null)
        {
            isStunned = true;
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

    void EndStun()
    {
        isStunned = false;
    }

    void Die()
    {
        isDead = true;
        isAttacking = false;
        anim.SetTrigger("Die");
        Debug.Log("Boss Died!");
        Destroy(gameObject, 1.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
