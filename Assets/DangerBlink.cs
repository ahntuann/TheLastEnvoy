using UnityEngine;

public class DangerBlink : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private float blinkSpeed = 5f;   
    [SerializeField] private float minAlpha = 0.2f;   
    [SerializeField] private float maxAlpha = 0.8f;   

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed)));
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }
}
