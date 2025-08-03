using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    [Header("Level Settings")]
    public string nextSceneName = "Level 2"; // Nom de la següent escena
    public int nextSceneIndex = 2;   // Índex de la següent escena (alternativa)
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip levelCompleteSound;
    
    private bool hasTriggered = false; // Per evitar múltiples activacions
    
    void Start()
    {
        // Assegurar-se que el collider està configurat com a trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("NextLevel necessita un Collider2D per funcionar!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Comprovar si és el jugador i no s'ha activat ja
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            
            Debug.Log("Jugador ha arribat al final del nivell!");
            
            // Reproduir so si està assignat
            if (audioSource != null && levelCompleteSound != null)
            {
                audioSource.PlayOneShot(levelCompleteSound);
                // Esperar que acabi el so abans de canviar d'escena
                Invoke(nameof(LoadNextLevel), levelCompleteSound.length);
            }
            else
            {
                // Canviar d'escena immediatament
                LoadNextLevel();
            }
        }
    }
    
    void LoadNextLevel()
    {
        // Prioritzar el nom de l'escena sobre l'índex
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else if (nextSceneIndex >= 0)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Si no s'ha especificat cap escena, carregar la següent en ordre
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }
}
