using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 3f;
    [SerializeField] private float detectRange = 5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Vector2 attackDelayRange = new Vector2(1f, 2f); 

    private Vector3 startPos;
    private bool movingRight = false;
    private bool isAttacking = false;
    private bool isPreparingAttack = false;
    private bool hasAttackedOnce = false;
    private Animator anim;
    private Transform player;

    void Start()
    {
        startPos = transform.position;
        anim = GetComponent<Animator>();

        if (!movingRight)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * -1;
            transform.localScale = scale;
        }
    }

    void Update()
    {
        
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

    
    System.Collections.IEnumerator ImmediateAttack()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");

        
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }

    
    System.Collections.IEnumerator PrepareAttack()
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
