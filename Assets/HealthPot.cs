using UnityEngine;
using UnityEngine.UI;

public class HealthPot : MonoBehaviour
{
    [Header("Potion Settings")]
    public int healAmount = 50;
    public int price = 20;

    [Header("UI References")]
    public Text priceTxt;
    public Text quantityTxt;

    private Button button;
    private Player01Controller player;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnBuyPotion);

        player = FindObjectOfType<Player01Controller>();

        if (priceTxt != null)
            priceTxt.text = price + " coins";

        if (quantityTxt != null)
            quantityTxt.text = "x1";
    }

    void OnBuyPotion()
    {
        if (player == null)
        {
            Debug.LogWarning("Không tìm thấy Player01Controller trong scene!");
            return;
        }

        if (player.SpendCoins(price))
        {
            player.Heal(healAmount);
            Debug.Log($"Đã mua Health Potion! +{healAmount} HP");
        }
        else
        {
            Debug.Log("Không đủ coin để mua potion!");
        }
    }
}