using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 3f;
    [SerializeField] private float detectRange = 2f;
    [SerializeField] private LayerMask playerLayer;

    private Vector3 startPos;
    private bool movingRight = false;
    private bool isAttacking = false;
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
        DetectPlayer();

        if (isAttacking) return;

        Move();
    }

    void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);

        if (hit != null)
        {
            player = hit.transform;
            FacePlayer(); 

            if (!isAttacking)
            {
                isAttacking = true;
                anim.SetTrigger("Attack");
            }
        }
        else
        {
           
            if (isAttacking)
            {
                isAttacking = false;

     
                movingRight = transform.localScale.x > 0;
            }
        }
    }

    void FacePlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;


        if (player.position.x > transform.position.x && scale.x < 0)
        {
            scale.x *= -1;
        }
      
        else if (player.position.x < transform.position.x && scale.x > 0)
        {
            scale.x *= -1;
        }

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
