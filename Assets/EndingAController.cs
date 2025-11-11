using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndingAController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI storyText;       // Text hiển thị câu chuyện
    public Button skipButton;               // Nút bỏ qua
    public CanvasGroup textCanvasGroup;     // Dùng để fade in/out text (gắn vào object chứa storyText)

    [Header("Story Settings")]
    [TextArea(3, 8)]
    public string[] storyLines;             // Các đoạn văn
    public float textDisplayTime = 4f;      // Thời gian mỗi dòng hiển thị
    public float fadeDuration = 1.2f;       // Thời gian hiệu ứng fade

    [Header("Audio Settings")]
    public AudioSource audioSource;         // Gắn AudioSource từ Canvas hoặc object riêng
    public AudioClip backgroundMusic;       // Nhạc nền (gán trong Unity)
    [Range(0f, 1f)] public float musicVolume = 0.6f;

    [Header("Scene Settings")]
    public string nextScene = "MainMenu";   // Scene quay về

    void Start()
    {
        if (storyText != null)
            storyText.text = "";

        if (skipButton != null)
            skipButton.onClick.AddListener(OnSkip);

        if (textCanvasGroup != null)
            textCanvasGroup.alpha = 0;

        // Phát nhạc nền
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.volume = musicVolume;
            audioSource.loop = true;
            audioSource.Play();
        }

        StartCoroutine(PlayStory());
    }

    IEnumerator PlayStory()
    {
        for (int i = 0; i < storyLines.Length; i++)
        {
            storyText.text = storyLines[i];
            yield return StartCoroutine(FadeText(0, 1, fadeDuration));   // Fade in
            yield return new WaitForSeconds(textDisplayTime);
            yield return StartCoroutine(FadeText(1, 0, fadeDuration));   // Fade out
        }

        // Kết thúc, fade out nhạc và chuyển scene
        if (audioSource != null)
            StartCoroutine(FadeOutMusic(1f));

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextScene);
    }

    IEnumerator FadeText(float from, float to, float duration)
    {
        if (textCanvasGroup == null) yield break;

        float elapsed = 0f;
        textCanvasGroup.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            textCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        textCanvasGroup.alpha = to;
    }

    IEnumerator FadeOutMusic(float duration)
    {
        if (audioSource == null) yield break;

        float startVol = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }

        audioSource.Stop();
    }

    void OnSkip()
    {
        StopAllCoroutines();
        if (audioSource != null) audioSource.Stop();
        SceneManager.LoadScene(nextScene);
    }
}