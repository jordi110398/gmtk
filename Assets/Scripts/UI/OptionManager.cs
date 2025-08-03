using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public Slider volumenSlider;
    public Slider brilloSlider;
    public CanvasGroup brilloOverlay;
    public MainMenu menuPrincipal;

    void Start()
    {
        if (PlayerPrefs.HasKey("volumen"))
            volumenSlider.value = PlayerPrefs.GetFloat("volumen");
        else
            volumenSlider.value = AudioListener.volume;

        if (PlayerPrefs.HasKey("brillo"))
            brilloSlider.value = PlayerPrefs.GetFloat("brillo");
        else
            brilloSlider.value = brilloOverlay.alpha;

        CambiarVolumen(volumenSlider.value);
        CambiarBrillo(brilloSlider.value);
    }

    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
    }

    public void CambiarBrillo(float valor)
    {
        brilloOverlay.alpha = valor;
    }

    public void GuardarOpcionesYSalir()
    {
        PlayerPrefs.SetFloat("volumen", volumenSlider.value);
        PlayerPrefs.SetFloat("brillo", brilloSlider.value);
        PlayerPrefs.Save();

        if (menuPrincipal != null)
        {
            menuPrincipal.ClosedOptions();
        }
    }
}
