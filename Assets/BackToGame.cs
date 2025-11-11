using UnityEngine;

public class BackToGame : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;

    public void OnBackToGame()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            Time.timeScale = 1f;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Debug.LogWarning("ShopPanel chưa được gán trong Inspector!");
        }
    }
}
