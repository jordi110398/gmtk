using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Important! Cal afegir l'espai de noms per a les coroutines

public class PlayerManager : MonoBehaviour
{
    // MOVIMENT
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    // VIDA DEL JUGADOR
    public int vidaMax = 6;
    public int vidaActual;

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

    // Variable para el punto de spawn. Ahora es privada y se asigna con un método
    private Transform spawnPoint;

    private Animator animator;

    // AUDIO
    public AudioSource audioSource;

    // FEEDBACK VISUAL
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 originalScale;
    public float flashDuration = 0.1f; // Durada del flaix de dany
    private bool isFacingRight = true; // Per controlar la direcció del jugador
    private float damageCoolDown = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        vidaActual = vidaMax;
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        originalScale = transform.localScale;
        // Guarda la escala original del jugador

        // Guarda la posición inicial del jugador como el primer punto de spawn
        // Puedes cambiar esto para que el spawnPoint sea un GameObject inicial en la escena
        spawnPoint = transform;

        // COMPROVAR GROUNDCHECK
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        HandleInput();
        CheckGrounded();

        animator.SetBool("OnAir", !isGrounded);
        damageCoolDown -= Time.deltaTime;
        damageCoolDown = Mathf.Max(0f, damageCoolDown);

        // Comprovació de la vida del jugador
        if (vidaActual <= 0)
        {
            ResetPlayerPosition();
        }
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

        // Exemple per a provar la reducció de vida (pots esborrar-ho)
        if (Input.GetKeyDown(KeyCode.V))
        {
            vidaActual--;
            Debug.Log("Vida actual: " + vidaActual);
        }
    }

    void HandleMovement()
    {
        // MOVIMENT HORITZONTAL
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // GIRAR EL SPRITE SEGONS LA DIRECCIÓ
        if (horizontalInput > 0)
        {
            isFacingRight = true;
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z); // Mirar a la dreta
        }
        else if (horizontalInput < 0)
        {
            isFacingRight = false;
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z); // Mirar a l'esquerra
        }
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
        Vector3 spawnOffset = Vector3.zero;
        Quaternion copyRotation = Quaternion.identity; // Per defecte, sense rotació

        switch (currentCopies)
        {
            case 0:
                prefabToUse = platformCopyPrefab;
                debugMessage = "Primera còpia creada (Plataforma)! Activa amb Z";
                spawnOffset = new Vector3(isFacingRight ? 1.2f : -1.2f, 0f, 0f); // Davant del jugador
                break;
            case 1:
                prefabToUse = shooterCopyPrefab;
                debugMessage = "Segona còpia creada (Disparadora)! Activa amb X";
                spawnOffset = new Vector3(isFacingRight ? 1.5f : -1.5f, 0f, 0f); // Més endavant
                // NO modificar copyRotation!
                break;
            case 2:
                prefabToUse = explosiveCopyPrefab;
                debugMessage = "Tercera còpia creada (Explosiva)! Activa amb T";
                spawnOffset = new Vector3(isFacingRight ? 1.2f : -1.2f, -0.15f, 0f); // Davant i una mica abaix
                break;
        }

        // Si no s'ha assignat un prefab, utilitza l'objecte actual com a fallback
        if (prefabToUse == null)
        {
            prefabToUse = gameObject;
            Debug.LogWarning("No s'ha assignat un prefab per la còpia. S'ha usat el propi jugador com a fallback.");
        }

        // Crear i configurar la nova còpia
        newCopy = Instantiate(prefabToUse, transform.position + spawnOffset, copyRotation);

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
                // Mirar cap a la direcció del jugador
                SpriteRenderer shooterSprite = newCopy.GetComponent<SpriteRenderer>();
                if (shooterSprite != null)
                {
                    shooterSprite.flipX = isFacingRight;
                }
                // Si el prefab té un punt de disparo, girar-lo
                if (shooter.shootPoint != null)
                {
                    Vector3 originalLocalPos = shooter.shootPoint.localPosition;
                    shooter.shootPoint.localPosition = new Vector3(isFacingRight ? Mathf.Abs(originalLocalPos.x) : -Mathf.Abs(originalLocalPos.x), originalLocalPos.y, originalLocalPos.z);
                }
                // Guardar la direcció de dispar
                shooter.shootDirection = isFacingRight ? Vector2.right : Vector2.left;
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
            StartCoroutine(ExplodeCopies());
        }
    }

    // NOU MÈTODE: Estableix el punt de spawn actual
    public void SetCurrentCheckpoint(Transform newCheckpoint)
    {
        spawnPoint = newCheckpoint;
        Debug.Log("Checkpoint activat! Nou punt de respawn establert.");
    }

    private void ResetPlayerPosition()
    {
        if (spawnPoint != null)
        {
            // Restableix la posició i la vida
            transform.position = spawnPoint.position;
            vidaActual = vidaMax;
            Debug.Log("Jugador teletransportat al punt d'aparició i vida restablerta.");
        }
        else
        {
            // En cas que no hi hagi punt de spawn assignat, va a l'origen
            transform.position = Vector3.zero;
            vidaActual = vidaMax;
            Debug.LogWarning("No s'ha assignat cap punt de spawn! El jugador es teletransporta a l'origen (0,0).");
        }
    }

    // Coroutine per gestionar l'efecte d'explosió
    IEnumerator ExplodeCopies()
    {
        Debug.Log("Còpia explosiva activada! Canviant el seu color a vermell abans de l'explosió.");

        GameObject explosiveCopy = plantedCopies[2];
        if (explosiveCopy != null)
        {
            SpriteRenderer sr = explosiveCopy.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.red;
            }
        }

        yield return new WaitForSeconds(2f);

        Debug.Log("¡BOOM! Destruint totes les còpies.");

        foreach (GameObject copy in plantedCopies)
        {
            if (copy != null)
            {
                Destroy(copy);
            }
        }

        plantedCopies.Clear();
        currentCopies = 0;

        // Reinicia el jugador després de l'explosió
        ResetPlayerPosition();
    }

    public void TakeDamage(float amount){
        if (damageCoolDown <= 0f){
            vidaActual -= (int)amount;
            vidaActual = Mathf.Max(vidaActual, 0);
            StartCoroutine(PlayDamageFlash());
            StartCoroutine(PlayDamagePulse());

            if (vidaActual <= 0)
            {
                ResetPlayerPosition();
            }
            else
            {
                Debug.Log($"Jugador ha rebut {amount} de mal. Vida restant: {vidaActual}");
            }
            damageCoolDown = 1f;
        }
    }

    // FEEDBACK VISUAL
    public IEnumerator PlayDamageFlash()
    {
        Debug.Log("Flaix de mal activat!"); // Comprovació de si entra a la funció
        spriteRenderer.color = Color.red; // Color vermell
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    public IEnumerator PlayDamagePulse()
    {
        float pulseScale = 1.2f;
        float pulseDuration = 0.1f;
        float xSign = isFacingRight ? 1f : -1f;

        // Escala cap amunt mantenint la direcció
        transform.localScale = new Vector3(originalScale.x * pulseScale * xSign, originalScale.y * pulseScale, originalScale.z);
        yield return new WaitForSeconds(pulseDuration);

        // Torna a la mida original i direcció correcta
        transform.localScale = new Vector3(originalScale.x * xSign, originalScale.y, originalScale.z);
    }
}