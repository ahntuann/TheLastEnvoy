using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroTextPlayer : MonoBehaviour
{
    [Header("Video nền")]
    public VideoPlayer introVideo;

    [Header("UI Text hiển thị")]
    public TextMeshProUGUI introText;

    [Header("Cấu hình thời gian")]
    public float fadeDuration = 1.5f;
    public float delayBetweenLines = 2f;

    [Header("Âm thanh")]
    public AudioSource backgroundAudio;  // nhạc nền
    public AudioClip introMusic;         // âm thanh nền
    public AudioClip rainSound;          // hiệu ứng mưa
    public AudioClip drumSound;          // trống canh xa

    private AudioSource rainAudioSource;
    private AudioSource drumAudioSource;

    void Start()
    {
        if (introVideo != null)
            introVideo.Play();

        // Nhạc nền
        if (backgroundAudio != null && introMusic != null)
        {
            backgroundAudio.clip = introMusic;
            backgroundAudio.volume = 0.6f;
            backgroundAudio.loop = true;
            backgroundAudio.Play();
        }

        // Tạo AudioSource riêng cho rain và drum
        rainAudioSource = gameObject.AddComponent<AudioSource>();
        rainAudioSource.clip = rainSound;
        rainAudioSource.loop = true;
        rainAudioSource.volume = 0.3f;

        drumAudioSource = gameObject.AddComponent<AudioSource>();
        drumAudioSource.clip = drumSound;
        drumAudioSource.loop = true;
        drumAudioSource.volume = 1f;

        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        introText.text = "";
        introText.alpha = 0f;

        // Dòng 1
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(ShowLine("Năm Cảnh Thịnh thứ tư — 1792.", 1f));

        drumAudioSource.Play();
        rainAudioSource.Play();

        yield return new WaitForSeconds(delayBetweenLines);

        // Dòng 2
        yield return StartCoroutine(ShowLine("Vua Quang Trung vừa băng hà.\nCả kinh thành Phú Xuân chìm trong tang tóc và nghi kỵ.", 1f));
        yield return new WaitForSeconds(delayBetweenLines);

        // Dòng 3
        yield return StartCoroutine(ShowLine("Quyền lực rơi vào tay Thái sư Bùi Đắc Tuyên —\nmột cơn bão chính biến đang dâng lên trong bóng tối.", 1f));
        yield return new WaitForSeconds(delayBetweenLines);

        // Dòng 4
        yield return StartCoroutine(ShowLine("Giữa đêm mưa xối xả, một người bí ẩn\nđang lẩn trốn trong những ngóc ngách của kinh thành.", 1f));
        yield return new WaitForSeconds(delayBetweenLines);

        // Dòng 5 – chữ to hơn, nhạc trầm dần
        if (backgroundAudio != null)
            StartCoroutine(FadeOutMusic(backgroundAudio, 4f));

        introText.fontSize += 10;
        yield return StartCoroutine(ShowLine("Nơi hắn muốn đến là <b>Mật thất</b> nơi có người đang đợi hắn...\nVà đêm nay, vận mệnh của đất nước liệu có đổi thay…", 2f));

        // Kết thúc
        yield return new WaitForSeconds(3f);

        // Tắt hiệu ứng âm thanh
        rainAudioSource.Stop();
        drumAudioSource.Stop();

        SceneManager.LoadScene("MatThat");
    }

    IEnumerator ShowLine(string text, float holdTime)
    {
        introText.text = text;
        yield return StartCoroutine(FadeText(0, 1));
        yield return new WaitForSeconds(holdTime + fadeDuration);
        yield return StartCoroutine(FadeText(1, 0));
    }

    IEnumerator FadeText(float from, float to)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            introText.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        introText.alpha = to;
    }

    IEnumerator FadeOutMusic(AudioSource audio, float duration)
    {
        float startVolume = audio.volume;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            audio.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
        audio.Stop();
    }
}
