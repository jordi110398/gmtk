using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private GameObject gameManager;
    [SerializeField] private float health = 10f;
    [SerializeField] private GameObject ropePrefab;
    [SerializeField] private Transform ropeSpawnPoint;
    [SerializeField] private float attackInterval = 5f;

    private float attackTimer = 0f;

    void Update()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            AttackWithRope();
            attackTimer = 0f;
        }
    }

    void AttackWithRope()
    {
        Instantiate(ropePrefab, ropeSpawnPoint.position, ropeSpawnPoint.rotation);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.GetComponent<HealthSystem>().TakeDamage(1f);
        }
        else if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
            health -= 1f;
            if (health <= 0)
            {
                Destroy(gameObject);
                Debug.Log("Boss defeated!");
            }
            else
            {
                Debug.Log($"Boss health remaining: {health}");
            }
        }
    }
}