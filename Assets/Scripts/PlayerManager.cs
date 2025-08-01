using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    // MOVIMENT
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    // GROUNDCHECK
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask = 1;

    // CÒPIES
    public int maxCopies = 3;
    private int currentCopies = 0;
    private List<GameObject> plantedCopies = new List<GameObject>();
    
    // PREFABS DE CÒPIES (assignar des de l'inspector)
    public GameObject platformCopyPrefab;   // Prefab per la còpia plataforma
    public GameObject shooterCopyPrefab;    // Prefab per la còpia disparadora  
    public GameObject explosiveCopyPrefab;  // Prefab per la còpia explosiva
    
    // TIPUS DE CÒPIES (per ordre de creació)
    private GameObject platformCopy = null;    // Primera còpia (Z)
    private GameObject shooterCopy = null;     // Segona còpia (X)
    private GameObject explosiveCopy = null;   // Tercera còpia (T)
    
    // CONFIGURACIÓ DISPARS
    public GameObject projectilePrefab; // Assignar des de l'inspector
    public float projectileSpeed = 10f;

    // FISICA
    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // COMPROVAR GROUNDCHECK
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }
    }

    void Update()
    {
        HandleInput();
        CheckGrounded();
    }
    
    void FixedUpdate()
    {
        HandleMovement();
    }
    
    void HandleInput()
    {
        // INPUT HORITZONTAL
        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A))
            horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.D))
            horizontalInput = 1f;
        
        // SALT
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
        
        // CREAR CÒPIES SEQÜENCIALMENT AMB C
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreateNextCopy();
        }
        
        // ACTIVAR CÒPIES
        if (Input.GetKeyDown(KeyCode.Z) && platformCopy != null)
        {
            ActivatePlatformCopy();
        }
        else if (Input.GetKeyDown(KeyCode.X) && shooterCopy != null)
        {
            ActivateShooterCopy();
        }
        else if (Input.GetKeyDown(KeyCode.T) && explosiveCopy != null)
        {
            ActivateExplosiveCopy();
        }
    }
    
    void HandleMovement()
    {
        // MOVIMENT HORITZONTAL
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }
    
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
    
    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void CreateNextCopy()
    {
        if (currentCopies >= maxCopies)
        {
            Debug.Log("Has arribat al màxim de còpies!");
            return;
        }
        
        Vector3 copyPosition = transform.position;
        
        if (platformCopy == null)
        {
            // Crear primera còpia: PLATAFORMA
            GameObject prefabToUse = platformCopyPrefab != null ? platformCopyPrefab : gameObject;
            platformCopy = Instantiate(prefabToUse, copyPosition, transform.rotation);
            
            // Si és un prefab personalitzat, afegir els components necessaris
            if (platformCopyPrefab != null)
            {
                if (platformCopy.GetComponent<Rigidbody2D>() == null)
                    platformCopy.AddComponent<Rigidbody2D>();
            }
            else
            {
                Destroy(platformCopy.GetComponent<PlayerManager>());
            }
            
            Rigidbody2D copyRb = platformCopy.GetComponent<Rigidbody2D>();
            if (copyRb != null)
            {
                copyRb.bodyType = RigidbodyType2D.Static;
            }
            
            platformCopy.tag = "PlatformCopy";
            plantedCopies.Add(platformCopy);
            currentCopies++;
            
            Debug.Log("Primera còpia creada (Plataforma)! Activa amb Z");
        }
        else if (shooterCopy == null)
        {
            // Crear segona còpia: DISPARADORA
            GameObject prefabToUse = shooterCopyPrefab != null ? shooterCopyPrefab : gameObject;
            shooterCopy = Instantiate(prefabToUse, copyPosition, transform.rotation);
            
            // Si és un prefab personalitzat, afegir els components necessaris
            if (shooterCopyPrefab != null)
            {
                if (shooterCopy.GetComponent<Rigidbody2D>() == null)
                    shooterCopy.AddComponent<Rigidbody2D>();
            }
            else
            {
                Destroy(shooterCopy.GetComponent<PlayerManager>());
            }
            
            if (shooterCopy.GetComponent<ShooterCopy>() == null)
                shooterCopy.AddComponent<ShooterCopy>();
            
            Rigidbody2D copyRb = shooterCopy.GetComponent<Rigidbody2D>();
            if (copyRb != null)
            {
                copyRb.bodyType = RigidbodyType2D.Static;
            }
            
            ShooterCopy shooter = shooterCopy.GetComponent<ShooterCopy>();
            shooter.projectilePrefab = projectilePrefab;
            shooter.projectileSpeed = projectileSpeed;
            
            // Desactivar el component inicialment
            shooter.enabled = false;
            
            shooterCopy.tag = "ShooterCopy";
            plantedCopies.Add(shooterCopy);
            currentCopies++;
            
            Debug.Log("Segona còpia creada (Disparadora)! Activa amb X");
        }
        else if (explosiveCopy == null)
        {
            // Crear tercera còpia: EXPLOSIVA
            GameObject prefabToUse = explosiveCopyPrefab != null ? explosiveCopyPrefab : gameObject;
            explosiveCopy = Instantiate(prefabToUse, copyPosition, transform.rotation);
            
            // Si és un prefab personalitzat, afegir els components necessaris
            if (explosiveCopyPrefab != null)
            {
                if (explosiveCopy.GetComponent<Rigidbody2D>() == null)
                    explosiveCopy.AddComponent<Rigidbody2D>();
            }
            else
            {
                Destroy(explosiveCopy.GetComponent<PlayerManager>());
            }
            
            if (explosiveCopy.GetComponent<ExplosiveCopy>() == null)
                explosiveCopy.AddComponent<ExplosiveCopy>();
            
            Rigidbody2D copyRb = explosiveCopy.GetComponent<Rigidbody2D>();
            if (copyRb != null)
            {
                copyRb.bodyType = RigidbodyType2D.Static;
            }
            
            // Desactivar el component inicialment
            explosiveCopy.GetComponent<ExplosiveCopy>().enabled = false;
            
            explosiveCopy.tag = "ExplosiveCopy";
            plantedCopies.Add(explosiveCopy);
            currentCopies++;
            
            Debug.Log("Tercera còpia creada (Explosiva)! Activa amb T");
        }
    }
    
    void ActivatePlatformCopy()
    {
        Debug.Log("Còpia plataforma activada! (Ja està activa per defecte)");
    }
    
    void ActivateShooterCopy()
    {
        if (shooterCopy != null)
        {
            ShooterCopy shooter = shooterCopy.GetComponent<ShooterCopy>();
            if (shooter != null)
            {
                shooter.enabled = true;
                Debug.Log("Còpia disparadora activada!");
            }
        }
    }
    
    void ActivateExplosiveCopy()
    {
        if (explosiveCopy != null)
        {
            ExplosiveCopy explosive = explosiveCopy.GetComponent<ExplosiveCopy>();
            if (explosive != null)
            {
                explosive.enabled = true;
                Debug.Log("Còpia explosiva activada!");
            }
        }
    }
}
