using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour
{
    [Header("UI Gợi ý (Hint Text)")]
    [Tooltip("Kéo vào đây đối tượng Text (ví dụ HintText trong Canvas)")]
    public GameObject hintUI;

    [Header("Cấu hình Scene")]
    [Tooltip("Tên Scene sẽ load khi nhấn O")]
    public string nextSceneName = "Bad_end";

    private bool playerInRange = false;

    void Start()
    {
        // Ẩn hint khi bắt đầu game
        if (hintUI != null)
            hintUI.SetActive(false);
        else
            Debug.LogWarning("⚠️ Chưa gán HintUI vào DoorTrigger!");
    }

    void Update()
    {
        // Khi player đang ở gần cửa và nhấn O => mở cửa
        if (playerInRange && Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("🚪 Mở cửa → chuyển Scene...");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Main"))
        {
            playerInRange = true;

            if (hintUI != null)
            {
                hintUI.SetActive(true);
                Debug.Log("🪄 Gợi ý hiển thị: Ấn O để mở cửa");
            }
            else
            {
                Debug.LogWarning("⚠️ HintUI chưa được gán trong Inspector!");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Main"))
        {
            playerInRange = false;
            if (hintUI != null)
                hintUI.SetActive(false);
        }
    }
}
