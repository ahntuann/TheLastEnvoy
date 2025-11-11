using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndingAController : MonoBehaviour
{
    [Header("UI References")]
    public Image backgroundImage;             // Ảnh nền
    public CanvasGroup fadeOverlay;           // Lớp fade đen
    public TextMeshProUGUI storyText;         // Text hiển thị câu chuyện
    public Button skipButton;                 // Nút bỏ qua (nếu muốn)

    [Header("Story Settings")]
    [TextArea(3, 8)]
    public string[] storyLines;               // Các đoạn văn
    public float textDisplayTime = 4f;        // Mỗi đoạn hiển thị bao lâu
    public float fadeTime = 1.5f;             // Thời gian fade

    void Start()
    {
        if (fadeOverlay != null)
            fadeOverlay.alpha = 1;

        skipButton.onClick.AddListener(OnSkip);
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        // Fade in scene
        yield return StartCoroutine(Fade(1, 0, fadeTime));

        for (int i = 0; i < storyLines.Length; i++)
        {
            storyText.text = storyLines[i];
            yield return new WaitForSeconds(textDisplayTime);

            // Fade nhẹ giữa các dòng
            if (i < storyLines.Length - 1)
            {
                yield return StartCoroutine(Fade(0, 1, 0.5f));
                yield return new WaitForSeconds(0.3f);
                yield return StartCoroutine(Fade(1, 0, 0.5f));
            }
        }

        // Fade out và kết thúc
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(Fade(0, 1, fadeTime));

        SceneManager.LoadScene("MainMenu"); // Kết thúc → trở về Menu
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        if (fadeOverlay == null) yield break;
        float elapsed = 0;
        fadeOverlay.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeOverlay.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        fadeOverlay.alpha = to;
    }

    void OnSkip()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("MainMenu");
    }
}
