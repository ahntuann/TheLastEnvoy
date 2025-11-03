using UnityEngine;

// Namespace "lãnh địa" của bạn
namespace LangGioThan
{
    public class PlayerController : MonoBehaviour
    {
        // --- Các biến để chỉnh trong Inspector (giữ nguyên của bạn) ---
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float jumpForce = 15f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;

        // --- Các biến nội bộ ---
        private Rigidbody2D rb;
        private bool isGrounded;
        // private float moveInput; // Không cần ở đây nữa
        // private bool isFacingRight = true; // Không cần nữa, script 2 dùng cách lật khác
        // private bool wantsToJump = false; // Không cần nữa

        // Dùng Awake() để lấy component, tương tự script 2
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        // Dùng Update() cho tất cả logic, tương tự script 2
        void Update()
        {
            HandleMovement();
            HandleJump();
        }

        // Hàm xử lý di chuyển, mô phỏng theo script 2
        private void HandleMovement()
        {
            // --- 1. Lấy Input Di Chuyển (MƯỢT MÀ) ---
            // Dùng GetAxis() để có độ mượt (gia tốc), thay vì GetAxisRaw()
            float moveInput = Input.GetAxis("Horizontal");

            // --- 2. Áp Dụng Di Chuyển ---
            // Gán vận tốc trực tiếp, giống script 2
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            // --- 3. Lật Mặt (theo cách của script 2) ---
            // Cách lật này đơn giản và hiệu quả
            if (moveInput > 0)
            {
                // Quay mặt sang phải
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (moveInput < 0)
            {
                // Quay mặt sang trái
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        // Hàm xử lý nhảy, mô phỏng theo script 2
        private void HandleJump()
        {
            // --- 1. Kiểm Tra Đất ---
            // Kiểm tra đất ngay trước khi check input nhảy
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

            // --- 2. Xử Lý Nhảy ---
            // Dùng GetButtonDown("Jump") là phím Space (mặc định)
            // Áp dụng lực nhảy ngay lập tức, không cần cờ (flag)
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                // Gán vận tốc dọc bằng lực nhảy
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        // Không cần FixedUpdate() và Flip() nữa
    }
}