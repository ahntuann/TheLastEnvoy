using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FinalDecisionController : MonoBehaviour
{
    [Header("UI References")]
    public Button btnPhanKhang;
    public Button btnKhuatPhuc;
    public Button btnHySinh;
    public TextMeshProUGUI questionText;

    void Start()
    {
        // Gán sự kiện cho nút
        btnPhanKhang.onClick.AddListener(() => OnChoiceSelected("EndingA"));
        btnKhuatPhuc.onClick.AddListener(() => OnChoiceSelected("EndingB"));
        btnHySinh.onClick.AddListener(() => OnChoiceSelected("EndingC"));
    }

    void OnChoiceSelected(string sceneName)
    {
        // Tắt các nút để ngăn double click
        btnPhanKhang.interactable = false;
        btnKhuatPhuc.interactable = false;
        btnHySinh.interactable = false;

        // Chuyển ngay sang scene tương ứng
        SceneManager.LoadScene(sceneName);
    }
}
