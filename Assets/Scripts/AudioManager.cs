using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    public AudioSource musicAudioSource;
    public AudioClip musicClip;

    [Header("Sound Effects")]
    public AudioSource sfxAudioSource;  
    public AudioClip slashClip;         

    void Start()
    {
     
        musicAudioSource.clip = musicClip;
        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            PlaySlash();
        }
    }

    public void PlaySlash()
    {
        if (slashClip != null && sfxAudioSource != null)
        {
            sfxAudioSource.PlayOneShot(slashClip);
        }
    }
}
