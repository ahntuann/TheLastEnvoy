using UnityEngine;

public class ExplosionZone : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab; 
    [SerializeField] private float explosionInterval = 3f; 
    [SerializeField] private float explosionOffset = 0.2f; 
    private float timer;

    void Start()
    {
        timer = explosionInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            TriggerExplosion();
            timer = explosionInterval; 
        }
    }

    private void TriggerExplosion()
    {
        if (explosionPrefab != null)
        {
          
            Vector3 explosionPos = transform.position + new Vector3(0, explosionOffset, 0);
            Instantiate(explosionPrefab, explosionPos, Quaternion.identity);
        }
    }
}
