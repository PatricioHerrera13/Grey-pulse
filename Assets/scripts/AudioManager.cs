using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("SFX")]
    public AudioClip cutBig;
    public AudioClip cutSmall;
    public AudioClip bounce;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource =
            GetComponent<AudioSource>();
    }

    public void PlayCutBig()
    {
        audioSource.PlayOneShot(cutBig);
    }

    public void PlayCutSmall()
    {
        audioSource.PlayOneShot(cutSmall);
    }

    public void PlayBounce()
    {
        audioSource.PlayOneShot(bounce);
    }
}