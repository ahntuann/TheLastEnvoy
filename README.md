README – Combat System Final Integration (Player, Enemy, Boss & HPBar)
Tổng quan

Dự án này hoàn thiện hệ thống chiến đấu 2D cơ bản gồm:

Player có thể di chuyển, nhảy, tấn công.
Kẻ địch (Enemy) tuần tra, phát hiện và tấn công Player.
Boss có AI thông minh hơn (tấn công có độ trễ, knockback và stun khi trúng đòn).
Hệ thống HPBar UI hiển thị máu người chơi, trừ trực tiếp khi nhận sát thương.
Có logic phản đòn, knockback, delay sau tấn công giúp gameplay mượt và cân bằng.

Cấu trúc thư mục & Script
Assets/
├── Scripts/
│   ├── Player01Controller.cs     // Logic di chuyển, tấn công, nhận damage của Player
│   ├── Enemy.cs                  // Enemy thường: tuần tra, tấn công, nhận damage, knockback
│   ├── Boss.cs                   // Boss AI nâng cao, nhận damage, stun, knockback
│   ├── HealthBar.cs (tùy chọn)   // Nếu bạn có script riêng để cập nhật HP UI
│   └── (Các script phụ khác nếu có)
├── UI/
│   └── HPBar (Canvas UI gồm Border + Fill)

Player01Controller.cs
Di chuyển trái/phải, nhảy.
Tấn công cận chiến.
Khi bị quái đánh → máu giảm trực tiếp trên thanh HP UI.
Khi tấn công trúng → trừ máu Enemy hoặc Boss.
Hỗ trợ hiệu ứng knockback khi bị đánh.

Lưu ý:
Gán HP là Image (Fill) trong Canvas HPBar.
Gán enemyLayer bao gồm cả Enemy và Boss.
attackRange, attackDamage có thể chỉnh trong Inspector.

Enemy.cs
Tuần tra trong vùng cho phép.
Phát hiện Player trong phạm vi detectRange.
Tấn công khi Player ở gần.
Có delay giữa các đòn đánh.
Bị đẩy lùi nhẹ khi trúng đòn.
Chết khi HP ≤ 0.

Lưu ý:
hpBar có thể ẩn đi vì Enemy không cần thanh máu hiển thị.
Boss.cs
AI phức tạp hơn Enemy.
Phát hiện Player ở xa hơn (detectRange lớn).
Có độ trễ tấn công ngẫu nhiên (attackDelayRange).
Có knockback và stun khi bị trúng đòn.
Gây sát thương lớn hơn (damagePerHit).

Lưu ý:
TakeDamage() hoạt động khi Player gọi boss.TakeDamage(attackDamage).
Đảm bảo Boss thuộc cùng Layer với Enemy để Player có thể gây damage.

HPBar (UI)
Cấu trúc gợi ý:
Canvas (Screen Space - Overlay)
└── HPBar
    ├── Border (Image)
    └── Fill (Image) ← gán vào biến "HP" trong Player01Controller


Gợi ý:
Anchor: Top Left (cố định góc màn hình).
HP.fillAmount = currentHp / maxHp; cập nhật trực tiếp trong script Player.
Tùy chỉnh trong Unity
Player:

Add component: Rigidbody2D, Animator, Player01Controller.
Gán:

HP = Fill Image.
groundCheck = một Empty Object dưới chân.
enemyLayer = Layer có Enemy & Boss.
Enemy & Boss:
Add component: Rigidbody2D, Animator, script tương ứng.

playerLayer = Layer có Player.

Đảm bảo có Collider2D để nhận va chạm.
