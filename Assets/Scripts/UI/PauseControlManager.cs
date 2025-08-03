using UnityEngine;

public class PauseControlManager : MonoBehaviour
{
    public PauseMenuController pauseMenuController;

    public void Volver()
    {
        if (pauseMenuController != null)
            pauseMenuController.ResumeSubmenu();
    }
}
