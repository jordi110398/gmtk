using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseRoot;         
    public GameObject optionsPanel;      
    public GameObject controlsPanel;     

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed. isPaused=" + isPaused);
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        pauseRoot.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void Resume()
    {
        isPaused = false;
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(false);
        pauseRoot.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void OpenControls()
    {
        controlsPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void ResumeSubmenu()
    {
        optionsPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
