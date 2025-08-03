using UnityEngine;

public class Enemic : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    public Transform puntoA;
    public Transform puntoB;
    public float vidaEnemic = 1f; // Vida del enemigo
    public float velocidad = 2f;
    public Collider2D coliderCuerpo;
    public Collider2D coliderCabeza;

    private Vector3 destinoActual;
    private SpriteRenderer spriteRenderer;

    public GameObject playerManager;

    void Start()
    {
        destinoActual = puntoB.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Patrulla entre los dos puntos solo en el eje X
        Vector3 targetPosition = new Vector3(destinoActual.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, velocidad * Time.deltaTime);

        // Cambia de destino si llega a uno de los puntos (solo comprobando el eje X)
        if (Mathf.Abs(transform.position.x - destinoActual.x) < 0.05f)
        {
            destinoActual = destinoActual == puntoA.position ? puntoB.position : puntoA.position;
            // Gira el sprite
            spriteRenderer.flipX = destinoActual == puntoA.position;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si el jugador entra en la cabeza, el enemigo muere
        if (other.gameObject.CompareTag("Player") && other.bounds.Intersects(coliderCabeza.bounds))
        {
            Debug.Log("Cap trepitjat!");
            vidaEnemic -= 1f; // Reducir vida del enemigo
            if (vidaEnemic <= 0)
            {
                Debug.Log("Enemic mort!");
                Destroy(gameObject);
            }
        }
        // Si el jugador entra en el cuerpo, puedes gestionar daño aquí si quieres
        else if (other.gameObject.CompareTag("Player"))
        {
            gameManager.GetComponent<HealthSystem>().TakeDamage(0.5f);
        }
        else if (other.gameObject.CompareTag("Projectile"))
        {
            Destroy(gameObject);
        }
        // Si choca con cualquier otro collider (que no sea el jugador ni un proyectil), gira
        else if (!other.isTrigger && !other.gameObject.CompareTag("Player"))
        {
            destinoActual = destinoActual == puntoA.position ? puntoB.position : puntoA.position;
            spriteRenderer.flipX = destinoActual == puntoA.position;
        }
    }
}