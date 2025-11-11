using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class QuizOption
{
    public string optionText;
    public bool isCorrect;
}

[System.Serializable]
public class QuizQuestion
{
    public string questionText;
    public List<QuizOption> options;
}

public class QuizController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject quizPanel;
    public MonoBehaviour questionTextUI;
    public List<Button> optionButtons;

    [Header("Popup UI")]
    public GameObject badEndPopupFirst;   // Popup khi sai câu 1
    public Image popupImageFirst;         // Ảnh cho popup đầu tiên
    public Sprite badEndSpriteFirst;      // Hình ảnh hiển thị khi sai câu 1

    public GameObject badEndPopupLater;   // Popup khi sai ở các câu sau
    public Image popupImageLater;         // Ảnh cho popup thua sau này
    public Sprite badEndSpriteLater;      // Hình ảnh hiển thị khi sai quá 3 lần


    [Header("Questions Setup")]
    public List<QuizQuestion> quizQuestions;

    [Header("Settings")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color defaultColor = Color.white;
    public string badEndScene = "Bad_end_Quiz";

    [Header("Lives Display")]
    public List<GameObject> lifeIcons;

    // 🧩 ADDED: Callback thông báo quiz hoàn thành
    public System.Action<bool> OnQuizCompleted; // true = thắng, false = thua

    private int currentQuestionIndex = 0;
    private int wrongCount = 0;
    private bool isAnimatingLife = false;

    void Start()
    {
        Debug.Log("<color=cyan>🎮 QuizController START()</color>");

        if (quizPanel != null) quizPanel.SetActive(false);

        // 🔒 Ban đầu: Ẩn toàn bộ icon mạng
        if (lifeIcons != null && lifeIcons.Count > 0)
        {
            foreach (var icon in lifeIcons)
            {
                if (icon != null)
                    icon.SetActive(false);
            }
        }

        Debug.Log($"<color=yellow>🧠 Số câu hỏi được tải: {quizQuestions.Count}</color>");
        Debug.Log($"<color=yellow>❤️ Số mạng (đang ẩn): {lifeIcons.Count}</color>");
        if (badEndPopupFirst != null) badEndPopupFirst.SetActive(false);
        if (badEndPopupLater != null) badEndPopupLater.SetActive(false);

    }


    public void ShowQuestion(int index)
    {
        if (quizQuestions.Count == 0)
        {
            Debug.LogWarning("<color=red>⚠ Không có câu hỏi nào trong danh sách!</color>");
            return;
        }

        if (quizPanel != null) quizPanel.SetActive(true);
        QuizQuestion q = quizQuestions[index];
        Debug.Log($"<color=green>📖 Hiển thị câu hỏi #{index + 1}: {q.questionText}</color>");

        SetText(questionTextUI, q.questionText);

        for (int i = 0; i < optionButtons.Count; i++)
        {
            if (i < q.options.Count)
            {
                optionButtons[i].gameObject.SetActive(true);
                SetOptionButton(optionButtons[i], q.options[i]);
            }
            else
            {
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void SetOptionButton(Button btn, QuizOption option)
    {
        Text txt = btn.GetComponentInChildren<Text>();
        TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null) txt.text = option.optionText;
        else if (tmp != null) tmp.text = option.optionText;

        var colors = btn.colors;
        colors.normalColor = defaultColor;
        btn.colors = colors;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnOptionSelected(option, btn));
    }

    void ShowBadEndPopupFirst()
    {
        if (badEndPopupFirst != null && popupImageFirst != null && badEndSpriteFirst != null)
        {
            popupImageFirst.sprite = badEndSpriteFirst;
            badEndPopupFirst.SetActive(true);
        }
    }

    void ShowBadEndPopupLater()
    {
        if (badEndPopupLater != null && popupImageLater != null && badEndSpriteLater != null)
        {
            popupImageLater.sprite = badEndSpriteLater;
            badEndPopupLater.SetActive(true);
        }
    }



    void OnOptionSelected(QuizOption selectedOption, Button btn)
    {
        Debug.Log($"<color=white>➡ Bạn chọn: {selectedOption.optionText}</color>");

        var colors = btn.colors;
        colors.normalColor = selectedOption.isCorrect ? correctColor : wrongColor;
        btn.colors = colors;

        if (selectedOption.isCorrect)
        {
            Debug.Log("<color=green>✅ Câu trả lời đúng!</color>");
        }
        else
        {
            Debug.Log("<color=red>❌ Câu trả lời sai!</color>");
        }

        // --- Nếu là câu đầu tiên ---  
        if (currentQuestionIndex == 0 && !selectedOption.isCorrect)
        {
            Debug.Log("<color=magenta>💀 Sai ngay câu đầu tiên → Bad Ending Popup (FIRST)!</color>");
            ShowBadEndPopupFirst();
            return;
        }



        // --- Nếu là câu 2 trở đi ---
        if (!selectedOption.isCorrect)
        {
            wrongCount++;
            Debug.Log($"<color=orange>⚠ Sai {wrongCount} lần!</color>");

            // ✅ Gọi hàm làm mất mạng tương ứng
            if (wrongCount <= lifeIcons.Count)
                StartCoroutine(AnimateLifeLoss(wrongCount - 1));

            // Nếu sai quá 3 lần
            if (wrongCount >= 3)
            {
                Debug.Log("<color=red>☠ Sai quá 3 lần → Bad Ending Popup (LATER)!</color>");
                ShowBadEndPopupLater();
                OnQuizCompleted?.Invoke(false);

                // Chờ 1-2 giây để người chơi thấy popup trước khi chuyển scene
                StartCoroutine(GoToBadEndSceneAfterDelay(2f));
                return;
            }
        }

        NextQuestion();
    }

    void NextQuestion()
    {
        currentQuestionIndex++;
        Debug.Log($"<color=cyan>⏭ Chuyển sang câu #{currentQuestionIndex + 1}</color>");

        // ✨ Khi bước sang câu thứ 2 → hiện các mạng
        if (currentQuestionIndex == 1 && lifeIcons != null)
        {
            Debug.Log("<color=yellow>💡 Bắt đầu hiển thị mạng (lifeIcons)!</color>");
            foreach (var icon in lifeIcons)
            {
                if (icon != null)
                    icon.SetActive(true);
            }
        }

        if (currentQuestionIndex >= quizQuestions.Count)
        {
            Debug.Log("<color=lime>🏁 Quiz kết thúc – Người chơi vượt qua tất cả câu hỏi!</color>");
            if (quizPanel != null) quizPanel.SetActive(false);

            // 🧩 ADDED: báo thắng cuộc
            OnQuizCompleted?.Invoke(true);

            return;
        }

        ShowQuestion(currentQuestionIndex);
    }


    IEnumerator AnimateLifeLoss(int index)
    {
        Debug.Log($"[DEBUG] AnimateLifeLoss() gọi với index = {index}");

        if (isAnimatingLife)
        {
            Debug.Log("[DEBUG] ❌ Bỏ qua vì isAnimatingLife = true");
            yield break;
        }
        if (lifeIcons == null)
        {
            Debug.Log("[DEBUG] ❌ lifeIcons = null");
            yield break;
        }
        if (index < 0 || index >= lifeIcons.Count)
        {
            Debug.Log($"[DEBUG] ❌ Index không hợp lệ: {index}");
            yield break;
        }
        if (lifeIcons[index] == null)
        {
            Debug.Log($"[DEBUG] ❌ lifeIcons[{index}] = null");
            yield break;
        }

        isAnimatingLife = true;
        GameObject icon = lifeIcons[index];

        Image img = icon.GetComponent<Image>();
        if (img == null)
        {
            Debug.Log($"[DEBUG] ❌ Không tìm thấy Image trên {icon.name}");
            yield break;
        }

        Debug.Log($"<color=red>💔 Mất một mạng (Icon #{index + 1})</color>");

        // ✨ Nhấp nháy 2 lần trước khi tắt icon
        for (int i = 0; i < 2; i++)
        {
            Debug.Log($"[DEBUG] Blink {i + 1}/2 cho {icon.name}");
            img.enabled = false;
            yield return new WaitForSeconds(0.2f);
            img.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log($"[DEBUG] 🔚 Tắt icon {icon.name}");
        icon.SetActive(false);
        isAnimatingLife = false;
    }


    void SetText(MonoBehaviour textUI, string msg)
    {
        if (textUI == null) return;
        if (textUI is Text txt) txt.text = msg;
        else if (textUI is TextMeshProUGUI tmp) tmp.text = msg;
    }

    IEnumerator GoToBadEndSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(badEndScene);
    }

}
