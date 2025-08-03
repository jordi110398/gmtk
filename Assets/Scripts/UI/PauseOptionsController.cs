using UnityEngine;
using UnityEngine.UI;

public class PauseOptionsController : MonoBehaviour
{
    [Header("UI")]
    public Slider volumenSlider;
    public Slider brilloSlider;
    public CanvasGroup brilloOverlay; 

    [Header("Referencia")]
    public PauseMenuController pauseMenuController; 

    private const string KEY_VOLUME = "pause_volume";
    private const string KEY_BRIGHT = "pause_brightness";

    void Start()
    {
        // Cargar valores guardados o usar valores actuales
        if (PlayerPrefs.HasKey(KEY_VOLUME))
            volumenSlider.value = PlayerPrefs.GetFloat(KEY_VOLUME);
        else
            volumenSlider.value = AudioListener.volume;

        if (PlayerPrefs.HasKey(KEY_BRIGHT))
            brilloSlider.value = PlayerPrefs.GetFloat(KEY_BRIGHT);
        else
            brilloSlider.value = brilloOverlay != null ? brilloOverlay.alpha : 0f;

        AplicarVolumen(volumenSlider.value);
        AplicarBrillo(brilloSlider.value);

        volumenSlider.onValueChanged.AddListener(AplicarVolumen);
        brilloSlider.onValueChanged.AddListener(AplicarBrillo);
    }

    public void AplicarVolumen(float v)
    {
        AudioListener.volume = v;
    }

    public void AplicarBrillo(float b)
    {
        if (brilloOverlay != null)
            brilloOverlay.alpha = b;
    }

    public void GuardarYCerrar()
    {
        PlayerPrefs.SetFloat(KEY_VOLUME, volumenSlider.value);
        PlayerPrefs.SetFloat(KEY_BRIGHT, brilloSlider.value);
        PlayerPrefs.Save();

        
        if (pauseMenuController != null)
            pauseMenuController.ResumeSubmenu();
    }
}
