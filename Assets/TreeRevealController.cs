using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TreeRevealController : MonoBehaviour
{
    [Header("Object References")]
    public GameObject treeHidden;
    public GameObject treeNormal;
    public GameObject hiddenNPC;

    [Header("UI References")]
    public GameObject guideTextUI1;
    public GameObject guideTextUI2;
    public GameObject dialogUI;

    [Header("Input Settings")]
    public KeyCode keyV = KeyCode.V;
    public KeyCode keyS = KeyCode.S;
    public int rightClickButton = 1;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;                 // fade Guide/NPC
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Trigger Settings")]
    public string mainTag = "Main";

    [Header("Quiz Settings")]
    public QuizController quizController; // Tham chiếu QuizController

    [Header("Dialog Settings")]
    [TextArea(2, 5)]
    public List<string> npcDialogLines = new List<string>();
    public float dialogFadeDuration = 0.3f;         // fade in/out từng câu
    public float dialogHoldDuration = 1f;           // giữ câu thoại

    private bool stepV = false;
    private bool stepS = false;
    private bool revealed = false;
    private bool playerInRange = false;

    private int currentDialogIndex = 0;
    private bool dialogPlaying = false;

    private Text t1, t2, dialogText;
    private TMPro.TextMeshProUGUI tmp1, tmp2, dialogTMP;



    void Start()
    {
        // Tree & NPC setup
        if (treeHidden != null) treeHidden.SetActive(true);
        if (treeNormal != null) treeNormal.SetActive(false);
        if (hiddenNPC != null) hiddenNPC.SetActive(false);

        // Guide UI setup
        if (guideTextUI1 != null)
        {
            t1 = guideTextUI1.GetComponent<Text>();
            tmp1 = guideTextUI1.GetComponent<TMPro.TextMeshProUGUI>();
            SetAlpha(guideTextUI1, 0f);
        }
        if (guideTextUI2 != null)
        {
            t2 = guideTextUI2.GetComponent<Text>();
            tmp2 = guideTextUI2.GetComponent<TMPro.TextMeshProUGUI>();
            SetAlpha(guideTextUI2, 0f);
        }

        // Dialog UI setup
        if (dialogUI != null)
        {
            dialogText = dialogUI.GetComponent<Text>();
            dialogTMP = dialogUI.GetComponent<TMPro.TextMeshProUGUI>();
            SetAlpha(dialogUI, 0f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(mainTag) && !revealed)
        {
            playerInRange = true;
            StartCoroutine(ShowGuidesSequence());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return; // kiểm tra collider
        if (other.CompareTag("Main"))
        {
            if (guideTextUI1 != null)
                SetAlpha(guideTextUI1.gameObject, 0f);

            if (guideTextUI2 != null)
                SetAlpha(guideTextUI2.gameObject, 0f);
        }
    }


    void Update()
    {
        if (!playerInRange) return;

        // --- Bước nhấn V / S / Click Right ---
        if (!stepV && Input.GetKeyDown(keyV))
        {
            stepV = true;
            if (guideTextUI2 != null) SetText(guideTextUI2, "Tốt! Giờ nhấn [S]");
        }
        if (stepV && !stepS && Input.GetKeyDown(keyS))
        {
            stepS = true;
            if (guideTextUI2 != null) SetText(guideTextUI2, "Gần xong! Giờ Click chuột phải");
        }
        if (stepV && stepS && Input.GetMouseButtonDown(rightClickButton) && !revealed)
        {
            StartCoroutine(RevealTree());
        }

        // --- Next dialog bằng click trái ---
        if (revealed && dialogUI != null && Input.GetMouseButtonDown(0) && !dialogPlaying)
        {
            if (currentDialogIndex < npcDialogLines.Count)
                StartCoroutine(ShowDialogLine(npcDialogLines[currentDialogIndex]));
        }
    }

    // --- Guide sequence ---
    IEnumerator ShowGuidesSequence()
    {
        // Guide 1
        if (guideTextUI1 != null)
        {
            SetText(guideTextUI1, "Từ đã hình như gốc cây có gì đó mờ ám... Hãy cẩn thận.");
            yield return StartCoroutine(FadeInText(guideTextUI1));
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(FadeOutText(guideTextUI1));
        }

        // Guide 2
        if (guideTextUI2 != null)
        {
            SetText(guideTextUI2, "Hãy nhấn V-S-Click Right để kiểm tra");
            yield return StartCoroutine(FadeInText(guideTextUI2));
        }
    }

    // --- Reveal tree & NPC ---
    IEnumerator RevealTree()
    {
        revealed = true;

        // Update Guide 2 text
        if (guideTextUI2 != null)
            SetText(guideTextUI2, "Kẻ ẩn nấp đã lộ diện!");

        // Tree
        if (treeHidden != null) treeHidden.SetActive(false);
        if (treeNormal != null) treeNormal.SetActive(true);

        // NPC fade in
        if (hiddenNPC != null)
        {
            hiddenNPC.SetActive(true);
            yield return StartCoroutine(FadeInSprite(hiddenNPC));
        }

        // Dialog UI sẽ xuất hiện khi nhấn click trái
    }

    // --- Show dialog line with fade in/out ---
    IEnumerator ShowDialogLine(string line)
    {
        dialogPlaying = true;

        SetText(dialogUI, line);

        // Fade in
        float t = 0f;
        while (t < dialogFadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / dialogFadeDuration);
            SetAlpha(dialogUI, alpha);
            yield return null;
        }
        SetAlpha(dialogUI, 1f);

        // Hold
        yield return new WaitForSeconds(dialogHoldDuration);

        // Fade out
        t = 0f;
        while (t < dialogFadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / dialogFadeDuration);
            SetAlpha(dialogUI, alpha);
            yield return null;
        }
        SetAlpha(dialogUI, 0f);

        if (line.IndexOf("ok", System.StringComparison.OrdinalIgnoreCase) >= 0 && quizController != null)
        {
            quizController.ShowQuestion(0);
        }


        // Next
        currentDialogIndex++;
        dialogPlaying = false;
    }

    // --- Helpers ---
    void ResetSteps() { stepV = false; stepS = false; }

    void SetAlpha(GameObject go, float a)
    {
        if (go == null) return; // kiểm tra null

        Text txt = go.GetComponent<Text>();
        if (txt != null)
        {
            Color c = txt.color;
            c.a = a;
            txt.color = c;
            return;
        }

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            Color c = tmp.color;
            c.a = a;
            tmp.color = c;
        }
    }


    void SetText(GameObject go, string msg)
    {
        Text txt = go?.GetComponent<Text>();
        TMPro.TextMeshProUGUI tmp = go?.GetComponent<TMPro.TextMeshProUGUI>();
        if (txt != null) txt.text = msg;
        else if (tmp != null) tmp.text = msg;
    }

    IEnumerator FadeInText(GameObject go)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = fadeCurve.Evaluate(t / fadeDuration);
            SetAlpha(go, alpha);
            yield return null;
        }
        SetAlpha(go, 1f);
    }

    IEnumerator FadeOutText(GameObject go)
    {
        float t = 0f;
        float startAlpha = 1f;
        Text txt = go?.GetComponent<Text>();
        TMPro.TextMeshProUGUI tmp = go?.GetComponent<TMPro.TextMeshProUGUI>();
        if (txt != null) startAlpha = txt.color.a;
        else if (tmp != null) startAlpha = tmp.color.a;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, fadeCurve.Evaluate(t / fadeDuration));
            SetAlpha(go, alpha);
            yield return null;
        }
        SetAlpha(go, 0f);
    }

    IEnumerator FadeInSprite(GameObject obj)
    {
        SpriteRenderer[] renderers = obj.GetComponentsInChildren<SpriteRenderer>(true);
        if (renderers.Length == 0) yield break;

        Color[] originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].color;
            renderers[i].color = new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, 0);
        }

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float alpha = fadeCurve.Evaluate(t / fadeDuration);
            for (int i = 0; i < renderers.Length; i++)
            {
                Color c = originalColors[i];
                renderers[i].color = new Color(c.r, c.g, c.b, alpha);
            }
            yield return null;
        }

        for (int i = 0; i < renderers.Length; i++)
        {
            Color c = originalColors[i];
            renderers[i].color = new Color(c.r, c.g, c.b, 1);
        }
    }
}
