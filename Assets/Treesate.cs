using UnityEngine;

public class Sou : MonoBehaviour
{
    [Header("Object References")]
    public GameObject treeHidden;
    public GameObject treeNormal;
    public GameObject hiddenNPC;

    [Header("Input Settings")]
    public KeyCode keyV = KeyCode.V;
    public KeyCode keyS = KeyCode.S;
    public int mouseButton = 1; // 0 = Left, 1 = Right

    private bool revealed = false;

    void Update()
    {
        // Kiểm tra tổ hợp phím: V + S + Chuột phải
        if (!revealed && Input.GetKey(keyV) && Input.GetKey(keyS) && Input.GetMouseButtonDown(mouseButton))
        {
            Reveal();
        }
    }

    void Reveal()
    {
        revealed = true;

        // 1. Đổi cây
        if (treeHidden != null) treeHidden.SetActive(false);
        if (treeNormal != null) treeNormal.SetActive(true);

        // 2. Hiện NPC
        if (hiddenNPC != null) hiddenNPC.SetActive(true);

        Debug.Log("🌳 NPC đã lộ diện!");
    }
}
