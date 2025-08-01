using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Important! Cal afegir l'espai de noms per a les coroutines

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

    // CONFIGURACIÓ DISPARS
    public GameObject projectilePrefab; // Assignar des de l'inspector
    public float projectileSpeed = 10f;

    // FÍSICA
    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;

    // Variable pública per al punt de spawn (assignar des de l'Inspector)
    public Transform spawnPoint;

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
        horizontalInput = Input.GetAxisRaw("Horizontal");

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
        if (Input.GetKeyDown(KeyCode.Z) && plantedCopies.Count > 0)
        {
            // La primera còpia és la plataforma
            ActivatePlatformCopy();
        }
        else if (Input.GetKeyDown(KeyCode.X) && plantedCopies.Count > 1)
        {
            // La segona còpia és la disparadora
            ActivateShooterCopy();
        }
        else if (Input.GetKeyDown(KeyCode.T) && plantedCopies.Count > 2)
        {
            // La tercera còpia és l'explosiva
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

        GameObject prefabToUse = null;
        string debugMessage = "";
        GameObject newCopy = null;

        switch (currentCopies)
        {
            case 0:
                prefabToUse = platformCopyPrefab;
                debugMessage = "Primera còpia creada (Plataforma)! Activa amb Z";
                break;
            case 1:
                prefabToUse = shooterCopyPrefab;
                debugMessage = "Segona còpia creada (Disparadora)! Activa amb X";
                break;
            case 2:
                prefabToUse = explosiveCopyPrefab;
                debugMessage = "Tercera còpia creada (Explosiva)! Activa amb T";
                break;
        }

        // Si no s'ha assignat un prefab, utilitza l'objecte actual com a fallback
        if (prefabToUse == null)
        {
            prefabToUse = gameObject;
            Debug.LogWarning("No s'ha assignat un prefab per la còpia. S'ha usat el propi jugador com a fallback.");
        }

        // Crear i configurar la nova còpia
        newCopy = Instantiate(prefabToUse, transform.position, transform.rotation);
        
        // Treure l'script del jugador si s'utilitza el propi objecte
        if (prefabToUse == gameObject)
        {
            Destroy(newCopy.GetComponent<PlayerManager>());
        }

        // Assignar els components i les configuracions específiques
        Rigidbody2D copyRb = newCopy.GetComponent<Rigidbody2D>();
        if (copyRb == null)
        {
            copyRb = newCopy.AddComponent<Rigidbody2D>();
        }
        copyRb.bodyType = RigidbodyType2D.Static;

        // Afegir components específics i desactivar-los
        switch (currentCopies)
        {
            case 1: // Shooter
                ShooterCopy shooter = newCopy.GetComponent<ShooterCopy>();
                if (shooter == null)
                {
                    shooter = newCopy.AddComponent<ShooterCopy>();
                }
                shooter.projectilePrefab = projectilePrefab;
                shooter.projectileSpeed = projectileSpeed;
                shooter.enabled = false;
                break;
            case 2: // Explosive
                ExplosiveCopy explosive = newCopy.GetComponent<ExplosiveCopy>();
                if (explosive == null)
                {
                    explosive = newCopy.AddComponent<ExplosiveCopy>();
                }
                explosive.enabled = false;
                break;
        }

        plantedCopies.Add(newCopy);
        currentCopies++;
        Debug.Log(debugMessage);
    }
    
    void ActivatePlatformCopy()
    {
        Debug.Log("Còpia plataforma activada! (Ja està activa per defecte)");
    }
    
    void ActivateShooterCopy()
    {
        if (plantedCopies.Count > 1)
        {
            GameObject shooterCopy = plantedCopies[1];
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
        if (plantedCopies.Count > 2)
        {
            // Inicia la coroutine per a l'efecte d'explosió
            StartCoroutine(ExplodeCopies());
        }
    }

    // Coroutine per gestionar l'efecte d'explosió
    IEnumerator ExplodeCopies()
    {
        Debug.Log("Còpia explosiva activada! Canviant el seu color a vermell abans de l'explosió.");

        // 1. Només canvia el color de l'última còpia (l'explosiva) a vermell
        GameObject explosiveCopy = plantedCopies[2];
        if (explosiveCopy != null)
        {
            SpriteRenderer sr = explosiveCopy.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.red;
            }
        }

        // 2. Espera 2 segons (el temps de l'"explosió")
        yield return new WaitForSeconds(2f);

        Debug.Log("¡BOOM! Destruint totes les còpies.");

        // 3. Destrueix totes les còpies
        foreach (GameObject copy in plantedCopies)
        {
            if (copy != null)
            {
                Destroy(copy);
            }
        }

        // 4. Neteja la llista i reinicia el comptador
        plantedCopies.Clear();
        currentCopies = 0;

        // 5. Reinicia la posició del jugador al punt d'aparició assignat des de l'inspector
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            Debug.Log("Jugador teletransportat al punt d'aparició.");
        }
        else
        {
            Debug.LogWarning("No s'ha assignat cap punt de spawn! El jugador no s'ha mogut.");
        }
    }
}