using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraZoom : MonoBehaviour
{
    [Header("Zoom settings")]
    public Camera cam; // arrastra la Main Camera aquí
    public float targetZoom = 3f; // tamaño final (menor que el inicial para acercar)
    public float zoomSpeed = 2f; // qué tan rápido interpola
    public float threshold = 0.05f; // cuándo considera el zoom completado

    [Header("Scene")]
    public string sceneToLoad = "SampleScene";
    public float delayAfterZoom = 0.3f; // espera antes de cargar

    private bool isZooming = false;

    void Awake()
    {
        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        if (!isZooming) return;

        // Lerp del tamaño de la cámara
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

        if (Mathf.Abs(cam.orthographicSize - targetZoom) < threshold)
        {
            isZooming = false;
            cam.orthographicSize = targetZoom; 
            Invoke(nameof(LoadScene), delayAfterZoom);
        }
    }

    // Llamar esto desde MainMenu.StartJuego()
    public void EmpezarZoom()
    {
        isZooming = true;
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
