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
            priceTxt.text = price + " gold";

        if (quantityTxt != null)
            quantityTxt.text = "x1";
    }

    void OnBuyPotion()
    {
        if (player == null)
        {
            Debug.LogWarning("Không tìm thấy PlayerStats trong scene!");
            return;
        }

        if (player.coin >= price)
        {
            player.SpendCoins(price);
            player.Heal(healAmount);
            Debug.Log($"Đã mua Health Potion! +{healAmount} HP");
        }
        else
        {
            Debug.Log("Không đủ tiền để mua potion!");
        }
    }
}
