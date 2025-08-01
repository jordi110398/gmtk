using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Important! Cal afegir l'espai de noms per a les coroutines

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerManager : MonoBehaviour
{
    // --- VARIABLES CONFIGURABLES DESDE EL INSPECTOR ---

    [Header("MOVIMENT")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("GROUNDCHECK")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask = 1;

    [Header("CÒPIES")]
    public int maxCopies = 3;
    // PREFABS DE CÒPIES (assignar des de l'inspector)
    // S'assignen en ordre: 0=Plataforma, 1=Disparadora, 2=Explosiva
    public GameObject[] copyPrefabs = new GameObject[3];

    [Header("CONFIGURACIÓ DISPARS")]
    public GameObject projectilePrefab; // Assignar des de l'inspector
    public float projectileSpeed = 10f;

    // --- VARIABLES PRIVADES ---

    // FÍSICA
    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;

    // CÒPIES
    private int currentCopies = 0;
    private readonly List<GameObject> plantedCopies = new List<GameObject>();

    // --- MÈTODES DE UNITY ---

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // COMPROVAR GROUNDCHECK
        if (groundCheck == null)
        {
            CreateGroundCheck();
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

    // --- GESTIÓ D'INPUTS I MOVIMENT ---

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
        if (Input.GetKeyDown(KeyCode.Z))
        {
            ActivatePlatformCopy();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            ActivateShooterCopy();
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            ActivateExplosiveCopy();
        }
    }

    void HandleMovement()
    {
        // MOVIMENT HORITZONTAL
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    // --- LÒGICA DE LES CÒPIES ---

    void CreateNextCopy()
    {
        if (currentCopies >= maxCopies)
        {
            Debug.Log("Has arribat al màxim de còpies!");
            return;
        }

        GameObject prefabToUse = copyPrefabs[currentCopies];

        // Si no s'ha assignat un prefab, mostra un avís i no continuis.
        if (prefabToUse == null)
        {
            Debug.LogWarning($"No s'ha assignat el prefab per a la còpia {currentCopies}. Fes-ho a l'inspector.");
            return;
        }

        // Crear i configurar la nova còpia
        GameObject newCopy = Instantiate(prefabToUse, transform.position, transform.rotation);
        
        // Assegurem que la còpia sigui estàtica
        if (newCopy.TryGetComponent<Rigidbody2D>(out Rigidbody2D copyRb))
        {
            copyRb.bodyType = RigidbodyType2D.Static;
        }

        // Configurar components específics de la còpia
        ConfigureCopyComponents(newCopy, currentCopies);

        plantedCopies.Add(newCopy);
        currentCopies++;
        Debug.Log($"Còpia {currentCopies} creada! Tipus: {prefabToUse.name}");
    }

    void ConfigureCopyComponents(GameObject copy, int copyIndex)
    {
        // Assignar components i les configuracions específiques
        switch (copyIndex)
        {
            case 1: // Shooter
                if (copy.TryGetComponent<ShooterCopy>(out var shooter))
                {
                    shooter.projectilePrefab = projectilePrefab;
                    shooter.projectileSpeed = projectileSpeed;
                    shooter.enabled = false; // El mantenim desactivat fins que s'activi amb la tecla X
                }
                break;
            case 2: // Explosive
                if (copy.TryGetComponent<ExplosiveCopy>(out var explosive))
                {
                    explosive.enabled = false; // El mantenim desactivat fins que s'activi amb la tecla T
                }
                break;
        }
    }

    void ActivatePlatformCopy()
    {
        // La primera còpia és la plataforma
        if (plantedCopies.Count > 0)
        {
            Debug.Log("Còpia plataforma activada! (Ja està activa per defecte)");
        }
    }

    void ActivateShooterCopy()
    {
        // La segona còpia és la disparadora
        if (plantedCopies.Count > 1 && plantedCopies[1] != null)
        {
            ShooterCopy shooter = plantedCopies[1].GetComponent<ShooterCopy>();
            if (shooter != null)
            {
                shooter.enabled = true;
                Debug.Log("Còpia disparadora activada!");
            }
        }
    }

    void ActivateExplosiveCopy()
    {
        // La tercera còpia és l'explosiva
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
        if (explosiveCopy != null && explosiveCopy.TryGetComponent<SpriteRenderer>(out var sr))
        {
            sr.color = Color.red;
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
    }


    // --- HELPERS I GIZMOS ---

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);
    }

    void CreateGroundCheck()
    {
        GameObject groundCheckObj = new GameObject("GroundCheck");
        groundCheckObj.transform.SetParent(transform);
        groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
        groundCheck = groundCheckObj.transform;
        Debug.Log("GroundCheck object creat automàticament.");
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}