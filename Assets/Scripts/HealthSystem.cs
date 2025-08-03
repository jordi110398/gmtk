using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public HeartSystem playerHearts; // Referència al HeartSystem del Player

    // Vida màxima (igual que al HeartSystem)
    public float maxHealth;

    // Vida actual
    private float playerHealth;

    // Jugador
    public GameObject player;
 
    // Camera
    private Camera mainCamera;

    // Menu Game Over
    public GameObject gameOverMenu;

    private void Update()
    {

    }
    private void Start()
    {
        // Inicialitzar la vida dels jugadors
        playerHealth = maxHealth = playerHearts.maxHealth;
        mainCamera = Camera.main;
    }

    // Funció per infligir mal a un jugador
    public void TakeDamage(string playerTag, float amount)
    {
        // Obtenir els jugadors
        player = GameObject.FindGameObjectWithTag("Player");


        if (playerTag == "Player")
        {
            var playerController = player.GetComponent<PlayerManager>();
            playerHealth -= amount;
            playerHealth = Mathf.Max(playerHealth, 0); // Evitar valors negatius
            playerHearts.TakeDamage(amount); // Actualitzar la barra de vida

            // So de dany Player
            playerController.audioSource.PlayOneShot(AudioManager.Instance.playerHurt);
            // Camera shake
            if (mainCamera != null)
            {
                mainCamera.GetComponent<AdaptiveCamera>().ShakeCamera(0.15f, 0.3f); // Iniciar el shake de la càmera
            }
            // --- Flash de dany ---
            playerController.StartCoroutine(playerController.PlayDamageFlash());
            playerController.StartCoroutine(playerController.PlayDamagePulse());

            Debug.Log($"Player ha rebut {amount} de mal. Vida restant: {playerHealth}");
        }
    }
}
