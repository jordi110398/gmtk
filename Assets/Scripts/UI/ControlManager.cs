using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public MainMenu menuPrincipal;
    
    public void Exit()
    {
        if (menuPrincipal != null)
        {
            menuPrincipal.ClosedControlls();
        }
    }
}
