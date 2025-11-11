using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class QuanDaiThan : MonoBehaviour
{
    public static event System.Action OnDuaThu; // 📜 Sự kiện thông báo Main rằng NPC đang đưa thư

    [Header("Điểm tuần tra")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float waitTime = 2f;

    [Header("Hội thoại")]
    public TextMeshProUGUI dialogText;
    public Canvas dialogCanvas;

    [TextArea(2, 4)]
    public string[] dialogLines = {
        "Xin chào, ta là Quan Đại Thần!",
        "Ngươi đã đến được đây, thật can đảm!",
        "Chúng ta có chuyện cần bàn đấy..."
    };

    [TextArea(2, 4)]
    public string[] dialogLines2 = {
        "Được rồi",
        "Hãy chuẩn bị cho thử thách kế tiếp!"
    };

    [TextArea(2, 4)]
    public string[] dialogLines3 = {
        "Hãy cầm lấy cái mật thư này!",
        "ĐI ĐI..."
    };

    [Header("UI Gợi ý")]
    [Tooltip("Text hiển thị gợi ý như 'Ấn E để chat'")]
    public GameObject hintChatUI;

    private Transform targetPoint;
    private Animator anim;
    private bool isWaiting = false;
    private bool stopPatrol = false;
    private float fixedY;

    private int currentDialogIndex = 0;
    private bool isTalking = false;
    private bool isWaitingForPlayer = false;
    private bool hasContinuedAfterPlayer = false;

    void Start()
    {
        targetPoint = pointB;
        anim = GetComponent<Animator>();
        fixedY = transform.position.y;

        if (anim != null)
            anim.SetBool("isWalking", true);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;

        if (dialogCanvas != null)
            dialogCanvas.enabled = false;

        if (hintChatUI != null)
            hintChatUI.SetActive(false);
    }

    void OnEnable()
    {
        PlayerDialogController.OnPlayerDialogSubmitted += HandlePlayerDialogSubmitted;
    }

    void OnDisable()
    {
        PlayerDialogController.OnPlayerDialogSubmitted -= HandlePlayerDialogSubmitted;
    }

    void Update()
    {
        if (!stopPatrol && !isWaiting)
            MoveBetweenPoints();
    }

    void MoveBetweenPoints()
    {
        Vector2 newPos = new Vector2(
            Mathf.MoveTowards(transform.position.x, targetPoint.position.x, speed * Time.deltaTime),
            fixedY
        );
        transform.position = newPos;

        Vector3 localScale = transform.localScale;
        localScale.x = (targetPoint == pointB) ? Mathf.Abs(localScale.x) : -Mathf.Abs(localScale.x);
        transform.localScale = localScale;

        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.05f)
            StartCoroutine(WaitAndSwitch());
    }

    IEnumerator WaitAndSwitch()
    {
        isWaiting = true;
        if (anim != null) anim.SetBool("isWalking", false);
        yield return new WaitForSeconds(waitTime);

        targetPoint = (targetPoint == pointA) ? pointB : pointA;

        if (anim != null) anim.SetBool("isWalking", true);
        isWaiting = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Main"))
            StopAndTalk();
    }

    void StopAndTalk()
    {
        if (stopPatrol) return;
        stopPatrol = true;
        isTalking = true;
        hasContinuedAfterPlayer = false;

        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetTrigger("isIdle");
        }

        if (dialogCanvas != null)
            dialogCanvas.enabled = true;

        currentDialogIndex = 0;
        StartCoroutine(ShowDialogCoroutine());
        Debug.Log("🛑 QuanDaiThan chạm Main → dừng và nói chuyện");
    }

    IEnumerator ShowDialogCoroutine()
    {
        // NPC nói đoạn 1
        while (currentDialogIndex < dialogLines.Length)
        {
            yield return StartCoroutine(TypeSentence(dialogLines[currentDialogIndex]));
            yield return new WaitForSeconds(2f);
            currentDialogIndex++;
        }

        // Dừng lại chờ player
        isTalking = false;
        isWaitingForPlayer = true;

        if (dialogCanvas != null)
            dialogCanvas.enabled = false;

        // 👇 Hiển thị gợi ý "Ấn E để chat"
        if (hintChatUI != null)
            hintChatUI.SetActive(true);

        Debug.Log("💬 Hiển thị gợi ý: Ấn E để chat");
    }

    // --- Khi Player nói xong ---
    private void HandlePlayerDialogSubmitted(string playerText)
    {
        Debug.Log("NPC nhận được sự kiện player nói: " + playerText);

        if (isWaitingForPlayer && !hasContinuedAfterPlayer)
        {
            isWaitingForPlayer = false;
            hasContinuedAfterPlayer = true;

            // Ẩn gợi ý chat sau khi player bắt đầu nói
            if (hintChatUI != null)
                hintChatUI.SetActive(false);

            StartCoroutine(ContinueAfterPlayer());
        }
    }

    IEnumerator ContinueAfterPlayer()
    {
        yield return new WaitForSeconds(2f);

        if (dialogCanvas != null)
            dialogCanvas.enabled = true;

        currentDialogIndex = 0;

        // NPC nói đoạn 2
        while (currentDialogIndex < dialogLines2.Length)
        {
            yield return StartCoroutine(TypeSentence(dialogLines2[currentDialogIndex]));
            yield return new WaitForSeconds(2f);
            currentDialogIndex++;
        }

        // Dua thu animation
        yield return StartCoroutine(DuaThuSequence());

        // NPC nói đoạn 3
        currentDialogIndex = 0;
        while (currentDialogIndex < dialogLines3.Length)
        {
            yield return StartCoroutine(TypeSentence(dialogLines3[currentDialogIndex]));
            yield return new WaitForSeconds(2f);
            currentDialogIndex++;
        }

        // Kết thúc hội thoại
        isTalking = false;
        stopPatrol = false;
        if (dialogCanvas != null) dialogCanvas.enabled = false;
        if (anim != null) anim.SetBool("isWalking", true);

        Debug.Log("✅ QuanDaiThan kết thúc hội thoại, tiếp tục tuần tra");
    }

    IEnumerator DuaThuSequence()
    {
        if (anim != null)
        {
            anim.SetTrigger("dua_thu");
            Debug.Log("🎬 NPC kích hoạt Dua_thu animation");
        }

        yield return new WaitForSeconds(1.2f);
        OnDuaThu?.Invoke(); // Gửi event để Main nhận thư
        Debug.Log("📨 Gửi event Dua_thu tới Main");
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.1f);
            Gizmos.DrawSphere(pointB.position, 0.1f);
        }
    }
#endif
}
