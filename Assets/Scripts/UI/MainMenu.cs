using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject armarioCerrado;
    public GameObject puertaIzquierdaAbierta;
    public GameObject puertaDerechaAbierta;
    public GameObject panelOpciones;
    public GameObject panelControles;
    public GameObject botonesMenu;
    public CameraZoom cameraZoom;




    public void StartJuego()
    {
        botonesMenu.SetActive(false);
        armarioCerrado.SetActive(false);
        puertaIzquierdaAbierta.SetActive(true);

        cameraZoom.EmpezarZoom();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void AbrirOpciones()
    {
        //Opciones
        botonesMenu.SetActive(false);
        armarioCerrado.SetActive(false);
        puertaDerechaAbierta.SetActive(true);
        panelOpciones.SetActive(true);
    }

    public void ClosedOptions()
    {
        panelOpciones.SetActive(false);
        puertaDerechaAbierta.SetActive(false);
        armarioCerrado.SetActive(true);
        botonesMenu.SetActive(true);

    }

    void Load()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void MostrarControles()
    {
        botonesMenu.SetActive(false);
        armarioCerrado.SetActive(false);
        puertaDerechaAbierta.SetActive(true);
        panelControles.SetActive(true);
    }

    public void ClosedControlls()
    {
        panelControles.SetActive(false);
        puertaDerechaAbierta.SetActive(false);
        armarioCerrado.SetActive(true);
        botonesMenu.SetActive(true);
    }
}
