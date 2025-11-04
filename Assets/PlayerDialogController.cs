using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class PlayerDialogController : MonoBehaviour
{
    [Header("Dialog UI")]
    public Canvas dialogCanvas;
    public TextMeshProUGUI dialogText;
    public TMP_InputField inputField;

    // Biến toàn cục để script khác (như TestController) biết player đang chat
    public static bool IsTyping { get; private set; } = false;

    // Sự kiện báo khi người chơi gửi xong tin nhắn
    public static event Action<string> OnPlayerDialogSubmitted;

    void Start()
    {
        Debug.Log("PlayerDialogController Start");

        if (dialogCanvas != null)
        {
            dialogCanvas.enabled = false;
            Debug.Log("DialogCanvas disabled at start");
        }

        if (inputField != null)
        {
            inputField.gameObject.SetActive(false);
            inputField.onEndEdit.AddListener(OnInputSubmit);
            Debug.Log("InputField hidden and listener registered");
        }
    }

    void Update()
    {
        // Khi đang nhập, chờ Enter để gửi
        if (IsTyping && inputField != null && inputField.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                string text = inputField.text;
                if (!string.IsNullOrWhiteSpace(text))
                {
                    ShowPlayerText(text);
                }
            }
        }

        // Bấm E để mở chat (chỉ khi chưa gõ)
        if (Input.GetKeyDown(KeyCode.E) && !IsTyping)
        {
            OpenInputDialog();
        }
    }

    void OpenInputDialog()
    {
        IsTyping = true;

        if (dialogCanvas != null)
        {
            dialogCanvas.enabled = true;
            Debug.Log("DialogCanvas enabled");
        }

        if (inputField != null)
        {
            inputField.text = "";
            inputField.gameObject.SetActive(true);
            inputField.ActivateInputField();
            Debug.Log("InputField activated");
        }

        if (dialogText != null)
        {
            dialogText.text = "";
            Debug.Log("DialogText cleared");
        }
    }

    void OnInputSubmit(string text)
    {
        if (!IsTyping) return;

        if (string.IsNullOrWhiteSpace(text))
        {
            Debug.Log("Input empty, ignored");
            return;
        }

        ShowPlayerText(text);
    }

    void ShowPlayerText(string text)
    {
        if (inputField != null)
        {
            inputField.gameObject.SetActive(false);
        }

        if (dialogText != null)
        {
            dialogText.text = "Main: " +text;
            Debug.Log($"DialogText updated: {text}");
        }

        IsTyping = false;
        Debug.Log("Typing ended");

        // Gửi event cho NPC hoặc hệ thống khác
        OnPlayerDialogSubmitted?.Invoke(text);

        StartCoroutine(DisableDialogCanvasAfterDelay(2f));

        string normalizedText = text.Trim().ToLower();

        if (normalizedText == "Không" || normalizedText == "Bye bye" ||
            normalizedText == "được" || normalizedText == "tạm biệt")
        {
            Debug.Log("Keyword detected, loading Bad_ending...");
            SceneManager.LoadScene("Bad_ending");
        }
    }

    private IEnumerator DisableDialogCanvasAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (dialogCanvas != null)
        {
            dialogCanvas.enabled = false;
            Debug.Log("DialogCanvas disabled after delay");
        }
    }
}
