using UnityEngine;
using System.Collections;

public class EnemyGuard : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;

    [Header("Alert Settings")]
    public GameObject alertIcon;
    public float alertDuration = 1f;

    [Header("Target")]
    public Transform player; // Gán thủ công hoặc gán runtime khi chạm

    private Animator animator;
    private Transform targetPoint;
    private float fixedY;
    private bool isChasing = false;
    private bool isAlerting = false;
    private bool playerDetected = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        targetPoint = pointB;
        fixedY = transform.position.y;

        if (alertIcon != null)
            alertIcon.SetActive(false);
    }

    void Update()
    {
        if (isChasing && player != null)
        {
            ChasePlayer();
        }
        else if (!isChasing && !isAlerting)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (animator != null)
            animator.Play("Walk");

        Vector3 currentPos = transform.position;
        currentPos.y = fixedY;

        Vector3 targetPos = new Vector3(targetPoint.position.x, fixedY, currentPos.z);
        float moveDir = targetPos.x - currentPos.x;

        transform.position = Vector2.MoveTowards(currentPos, targetPos, patrolSpeed * Time.deltaTime);
        Flip(moveDir);

        // Đổi hướng khi đến điểm
        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.05f)
            targetPoint = targetPoint == pointA ? pointB : pointA;
    }

    IEnumerator AlertThenChase()
    {
        isAlerting = true;
        if (animator != null)
            animator.Play("idle");

        if (alertIcon != null)
            alertIcon.SetActive(true);

        yield return new WaitForSeconds(alertDuration);

        if (alertIcon != null)
            alertIcon.SetActive(false);

        isAlerting = false;
        StartChase();
    }

    void StartChase()
    {
        isChasing = true;
        if (animator != null)
            animator.Play("Chase");
    }

    void ChasePlayer()
    {
        if (player == null) return;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(player.position.x, fixedY, currentPos.z);
        float moveDir = targetPos.x - currentPos.x;

        transform.position = Vector2.MoveTowards(currentPos, targetPos, chaseSpeed * Time.deltaTime);
        Flip(moveDir);

        // Nếu quá xa thì dừng đuổi
        if (Vector2.Distance(transform.position, player.position) > 6f)
        {
            StopChase();
        }
    }

    void StopChase()
    {
        isChasing = false;
        playerDetected = false;
        if (animator != null)
            animator.Play("Walk");
    }

    void Flip(float moveDirection)
    {
        if (moveDirection == 0) return;
        Vector3 scale = transform.localScale;
        if ((moveDirection > 0 && scale.x < 0) || (moveDirection < 0 && scale.x > 0))
        {
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    // 🔹 Phát hiện "test"
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("test"))
        {
            player = other.transform;
            if (!isChasing && !isAlerting)
                StartCoroutine(AlertThenChase());
        }
    }

    // 🔹 Khi test rời khỏi vùng
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("test"))
        {
            StopChase();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.1f);
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }
    }
#endif
}
