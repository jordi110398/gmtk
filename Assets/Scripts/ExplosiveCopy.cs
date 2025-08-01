using UnityEngine;

public class ExplosiveCopy : MonoBehaviour
{
    public float explosionRadius = 3f;
    public float explosionForce = 500f;
    public float triggerDistance = 1.5f;
    public LayerMask targetLayerMask = 1; // Capes que poden activar l'explosió
    
    private bool hasExploded = false;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
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
        
        // Efecte visual simple (canviar color)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
        }
        
        // Destruir la còpia després de l'explosió
        Destroy(gameObject, 0.5f);
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
