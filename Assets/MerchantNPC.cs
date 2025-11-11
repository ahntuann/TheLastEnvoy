using System;
using UnityEngine;

public class MerchantShopTrigger : MonoBehaviour
{
    [Header("UI Panel")]
    [SerializeField] private GameObject shopPanel; // Gán ShopPanel trong Inspector
    [Header("Player Detection")]
    [SerializeField] private string playerTag = "Player";

    private bool isPlayerNear = false;
    private bool isShopOpen = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            ToggleShop();
        }

        // Cho phép đóng shop bằng phím Esc
        if (isShopOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleShop();
        }
    }

    void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        shopPanel.SetActive(isShopOpen);

        // Khi mở shop, dừng chuyển động game
        Time.timeScale = isShopOpen ? 0f : 1f;

        // Hiển thị hoặc ẩn chuột
        Cursor.visible = isShopOpen;
        Cursor.lockState = isShopOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNear = true;
            Debug.Log("Nhấn E để mở shop!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNear = false;
            if (isShopOpen)
                ToggleShop();
        }
    }

    internal void CloseShopFromButton()
    {
        throw new NotImplementedException();
    }
}
