using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    public Vector3 originalPos;
    private bool moveBack = false;
    private SpriteRenderer spriteRenderer;
    private float maxDownDistance = 0.09f;
    private int objectsOnPlate = 0;

    [Header("Events")]
    public UnityEvent OnActivated;
    public UnityEvent OnDeactivated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Copy"))
        {
            objectsOnPlate++;
            moveBack = false;
            // Només activa si és el primer objecte
            if (objectsOnPlate == 1)
            {
                AudioManager.Instance.sfxSource.PlayOneShot(AudioManager.Instance.pressurePlateSound);
                OnActivated.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Copy"))
        {
            Debug.Log("Objecte sortint de la placa de pressió: " + other.name);
            objectsOnPlate = Mathf.Max(0, objectsOnPlate - 1);
            if (objectsOnPlate == 0)
            {
                moveBack = true;
                AudioManager.Instance.sfxSource.PlayOneShot(AudioManager.Instance.pressurePlateSound, 0.5f);
                OnDeactivated.Invoke();
            }
        }
    }

    void Update()
    {
        if (objectsOnPlate > 0)
        {
            if (transform.position.y > originalPos.y - maxDownDistance)
            {
                transform.Translate(0, -0.01f, 0);
                //spriteRenderer.color = Color.blue;
            }
        }
        else if (moveBack)
        {
            if (transform.position.y < originalPos.y)
            {
                transform.Translate(0f, 0.01f, 0f);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, originalPos.y, transform.position.z);
                moveBack = false;
            }
        }
    }
}
