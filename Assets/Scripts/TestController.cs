using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class TestController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float moveDistance = 1f;

    [Header("Input Chat Settings")]
    public TMP_InputField chatInputField;

    private Rigidbody2D rb;
    private Animator animator;

    private bool isMoving = false;
    private Vector2 moveDir;
    private Vector2 targetPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.WakeUp();

        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        animator.ResetTrigger("nhan_thu"); // 🔄 Reset tránh trigger sớm

        Debug.Log($"[Rigidbody2D check] type={rb.bodyType}, constraints={rb.constraints}");
    }

    void OnEnable()
    {
        QuanDaiThan.OnDuaThu += HandleNPCDuaThu;
    }

    void OnDisable()
    {
        QuanDaiThan.OnDuaThu -= HandleNPCDuaThu;
    }

    void Update()
    {
        // Nếu đang hiển thị hội thoại thì không cho di chuyển
        if (PlayerDialogController.IsTyping)
        {
            if (isMoving)
            {
                StopMove();
                animator.SetFloat("Speed", 0f);
            }
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");

        // Khi không di chuyển, bắt đầu di chuyển nếu nhấn phím
        if (!isMoving)
        {
            if (horizontal > 0.1f)
                StartMove(Vector2.right);
            else if (horizontal < -0.1f)
                StartMove(Vector2.left);
        }
        else
        {
            // Khi đang di chuyển, nếu đổi hướng thì quay đầu ngay
            if (horizontal > 0.1f)
                FaceDirection(Vector2.right);
            else if (horizontal < -0.1f)
                FaceDirection(Vector2.left);
        }

        animator.SetFloat("Speed", isMoving ? 1f : 0f);
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            float before = Vector2.Distance(rb.position, targetPos);
            rb.MovePosition(Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime));
            float after = Vector2.Distance(rb.position, targetPos);

            if (Mathf.Abs(before - after) < 0.0001f)
                Debug.LogWarning($"⚠ Không di chuyển được! pos={rb.position}, target={targetPos}");

            if (after < 0.01f)
                StopMove();
        }
    }

    void StartMove(Vector2 dir)
    {
        isMoving = true;
        moveDir = dir;
        FaceDirection(dir); // ✅ Quay mặt đúng hướng

        targetPos = rb.position + dir * moveDistance;
        Debug.Log($"➡️ Di chuyển hướng {dir}, từ {rb.position} đến {targetPos}");
    }

    void StopMove()
    {
        isMoving = false;
        rb.linearVelocity = Vector2.zero;
        Debug.Log($"🛑 Dừng tại {rb.position}");
    }

    // 🧭 Hàm xử lý quay hướng (dùng localScale hoặc flipX)
    void FaceDirection(Vector2 dir)
    {
        Vector3 scale = transform.localScale;
        float baseScaleX = Mathf.Abs(scale.x);
        scale.x = dir.x > 0 ? baseScaleX : -baseScaleX;
        transform.localScale = scale;

        Debug.Log($"↔️ Quay hướng {(dir.x > 0 ? "phải" : "trái")}, scale.x = {scale.x}");
    }

    // 📨 Khi NPC đưa thư → Main nhận thư
    private void HandleNPCDuaThu()
    {
        if (animator == null)
        {
            Debug.LogWarning("⚠ Animator của Main bị null, không thể chạy nhan_thu!");
            return;
        }

        StopMove();
        animator.SetFloat("Speed", 0f);
        StartCoroutine(PlayNhanThuAnim());
    }

    private IEnumerator PlayNhanThuAnim()
    {
        yield return new WaitForSeconds(0.3f);
        animator.ResetTrigger("nhan_thu");
        animator.SetTrigger("nhan_thu");
        Debug.Log("📩 Main nhận thư → chạy animation nhan_thu");

        // ⏳ Chờ animation xong (ví dụ dài 2s)
        yield return new WaitForSeconds(2f);

        // ✅ Cho phép Main di chuyển lại sau khi nhận thư
        isMoving = false;
        Debug.Log("✅ Main có thể di chuyển ra cửa");
    }

}
