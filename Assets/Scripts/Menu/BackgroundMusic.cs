using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    void Start()
    {
        GetComponent<AudioSource>().Play();
    }
}
