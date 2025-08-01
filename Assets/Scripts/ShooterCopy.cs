using UnityEngine;

public class ShooterCopy : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float shootInterval = 2f;
    public Transform shootPoint;
    private float lastShootTime;
    
    void Start()
    {
        // Crear punt de disparo si no existeix
        if (shootPoint == null)
        {
            GameObject shootPointObj = new GameObject("ShootPoint");
            shootPointObj.transform.SetParent(transform);
            shootPointObj.transform.localPosition = new Vector3(1f, 0f, 0f); // Dispara cap a la dreta
            shootPoint = shootPointObj.transform;
        }
        
        lastShootTime = Time.time;
    }
    
    void Update()
    {
        // Disparar automàticament cada shootInterval segons
        if (Time.time - lastShootTime >= shootInterval && projectilePrefab != null)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }
    
    void Shoot()
    {
        // Crear projectil
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        
        // Afegir velocitat al projectil
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb != null)
        {
            projectileRb.linearVelocity = Vector2.right * projectileSpeed;
        }
        
        // Destruir el projectil després de 5 segons
        Destroy(projectile, 5f);
        
        Debug.Log("Còpia disparadora ha disparat!");
    }
}
