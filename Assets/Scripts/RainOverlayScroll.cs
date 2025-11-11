using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RainOverlayScroll : MonoBehaviour
{
    public float speedY = -0.6f; // hướng mưa rơi xuống
    public float speedX = 0f;    // có thể chỉnh nghiêng mưa nhẹ
    private Material mat;
    private Vector2 offset;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        offset.x += speedX * Time.deltaTime;
        offset.y += speedY * Time.deltaTime;
        mat.mainTextureOffset = offset;
    }
}
