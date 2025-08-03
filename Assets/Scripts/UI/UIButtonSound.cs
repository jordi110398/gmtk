using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    public AudioClip clickSound;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            UIAudioManager.Instance.PlayClick(clickSound);
        });
    }
}
