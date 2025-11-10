using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // để load scene
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
    public GameObject quizPanel;          // Panel chứa quiz
    public MonoBehaviour questionTextUI;  // Có thể là Text hoặc TMP
    public List<Button> optionButtons;    // Buttons cho các lựa chọn

    [Header("Questions Setup")]
    public List<QuizQuestion> quizQuestions;

    [Header("Settings")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color defaultColor = Color.white;

    private int currentQuestionIndex = 0;

    void Start()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
    }

    public void ShowQuestion(int index)
    {
        if (quizQuestions.Count == 0) return;
        if (quizPanel != null) quizPanel.SetActive(true);

        QuizQuestion q = quizQuestions[index];

        // Set question text
        SetText(questionTextUI, q.questionText);

        // Set option buttons
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

    public void SetOptionButton(Button btn, QuizOption option)
    {
        // Tự động lấy Text hoặc TMP Text
        Text txt = btn.GetComponentInChildren<Text>();
        TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();

        if (txt != null) txt.text = option.optionText;
        else if (tmp != null) tmp.text = option.optionText;

        // Reset màu button
        var colors = btn.colors;
        colors.normalColor = defaultColor;
        btn.colors = colors;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnOptionSelected(option, btn));
    }

    public void OnOptionSelected(QuizOption selectedOption, Button btn)
    {
        var colors = btn.colors;
        colors.normalColor = selectedOption.isCorrect ? correctColor : wrongColor;
        btn.colors = colors;

        Debug.Log("Bạn chọn: " + selectedOption.optionText + " → " + (selectedOption.isCorrect ? "Đúng" : "Sai"));

        // --- NEW: nếu câu đầu tiên và chọn sai → Bad Ending ---
        if (currentQuestionIndex == 0 && !selectedOption.isCorrect)
        {
            Debug.Log("Sai câu đầu tiên → Bad Ending!");
            SceneManager.LoadScene("Bad_ending"); // load scene Bad_ending
            return;
        }

        // Nếu không phải câu đầu tiên hoặc chọn đúng → tiếp quiz
        NextQuestion();
    }

    public void NextQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex >= quizQuestions.Count)
        {
            Debug.Log("Quiz kết thúc!");
            if (quizPanel != null) quizPanel.SetActive(false);
            return;
        }
        ShowQuestion(currentQuestionIndex);
    }

    public void SetText(MonoBehaviour textUI, string msg)
    {
        if (textUI == null) return;

        if (textUI is Text txt) txt.text = msg;
        else if (textUI is TextMeshProUGUI tmp) tmp.text = msg;
    }
}
