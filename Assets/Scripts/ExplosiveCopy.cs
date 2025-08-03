using UnityEngine;

public class ExplosiveCopy : MonoBehaviour
{
    public float explosionRadius = 3f;
    public float explosionForce = 500f;
    public float triggerDistance = 1.5f;
    public LayerMask targetLayerMask = 1; // Capes que poden activar l'explosió


    public GameObject explosionParticlesPrefab; // Assigna el prefab des de l'inspector

    private bool hasExploded = false;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Canviar color per indicar que és explosiu (opcional)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
        }
    }
    
    void Update()
    {
        if (!hasExploded)
        {
            CheckForTargets();
        }
    }
    
    void CheckForTargets()
    {
        // Buscar objectius dins del radi de trigger
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, triggerDistance, targetLayerMask);
        
        foreach (Collider2D target in targets)
        {
            // Si detecta el jugador o altres objectius
            if (target.CompareTag("Player") || target.CompareTag("Enemy"))
            {
                Explode();
                break;
            }
        }
    }
    
    void Explode()
    {
        if (hasExploded) return;
        
        hasExploded = true;
        
        Debug.Log("Còpia explosiva ha explotat!");
        // Reproduir so d'explosió si està assignat (utilitza l'AudioManager global)
        if (AudioManager.Instance != null && AudioManager.Instance.explosionSound != null && AudioManager.Instance.sfxSource != null)
        {
            AudioManager.Instance.sfxSource.PlayOneShot(AudioManager.Instance.explosionSound);
        }
        
        // Trobar tots els objectes dins del radi d'explosió
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D obj in objectsInRange)
        {
            Rigidbody2D objRb = obj.GetComponent<Rigidbody2D>();
            if (objRb != null && objRb.gameObject != gameObject)
            {
                // Calcular direcció i distància
                Vector2 direction = (obj.transform.position - transform.position).normalized;
                float distance = Vector2.Distance(transform.position, obj.transform.position);
                // Aplicar força inversament proporcional a la distància
                float force = explosionForce / (distance + 1f);
                objRb.AddForce(direction * force);
                Debug.Log($"Objecte {obj.name} afectat per l'explosió!");
            }
        }

        // Instanciar partícules d'explosió si s'ha assignat el prefab
        if (explosionParticlesPrefab != null)
        {
            Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
        }

        // Efecte visual simple (canviar color)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
        }

        // Destruir la còpia després de l'explosió
        Destroy(gameObject, 0.8f);
    }
    
    void OnDrawGizmosSelected()
    {
        // Dibuixar radi de trigger
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
        
        // Dibuixar radi d'explosió
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
