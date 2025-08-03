using UnityEngine;

public class ShooterCopy : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public float shootInterval = 2f;
    public Transform shootPoint;
    private float lastShootTime;
    private AudioSource audioSource;

    public Vector2 shootDirection = Vector2.right; // Nova variable pública per la direcció de disparo

    void Start()
    {
        // Girar el sprite segons la direcció de disparo
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = (shootDirection.x > 0); // flipX només si dispara cap a la dreta
        }
        // Crear punt de disparo sempre davant del sprite
        float shootX = Mathf.Abs(shootDirection.x) > 0.1f ? Mathf.Sign(shootDirection.x) : -1f;
        if (shootPoint == null)
        {
            GameObject shootPointObj = new GameObject("ShootPoint");
            shootPointObj.transform.SetParent(transform);
            shootPointObj.transform.localPosition = new Vector3(shootX, 0f, 0f); // Davant del sprite
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
            projectileRb.linearVelocity = shootDirection.normalized * projectileSpeed;
        }
        // Destruir el projectil després de 5 segons
        Destroy(projectile, 5f);
        Debug.Log("Còpia disparadora ha disparat!");
    }
}
