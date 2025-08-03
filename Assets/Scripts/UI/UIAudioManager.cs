using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;
    public AudioSource source;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayClick(AudioClip clip)
    {
        if (clip != null && source != null && source.enabled)
        {
            source.PlayOneShot(clip);
        }
    }
}
