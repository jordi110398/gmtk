using UnityEngine;

public class MovingDoor : MonoBehaviour
{
public float moveDistance = 2f;      // Distància que es mou la porta
public float moveSpeed = 2f;         // Velocitat de moviment
private Vector3 closedPosition;
private Vector3 openPosition;
private bool isOpening = false;

public bool startOpen = true; // Pots posar-ho a false per començar tancada
public bool openDown = false; // Si true, la porta s'obre cap a baix

// SO
public AudioSource audioSource;

    void Start()
    {
        closedPosition = transform.position;
        // Decideix la direcció d'obertura
        Vector3 moveDir = openDown ? Vector3.down : Vector3.up;
        openPosition = closedPosition + moveDir * moveDistance;
        audioSource = GetComponent<AudioSource>();

        if (startOpen)
        {
            transform.position = openPosition;
            isOpening = true;
        }
        else
        {
            transform.position = closedPosition;
            isOpening = false;
        }
    }

    void Update()
    {
        bool isMoving = false;

        if (isOpening)
        {
            if (transform.position != openPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, openPosition, moveSpeed * Time.deltaTime);
                isMoving = true;
            }
        }
        else
        {
            if (transform.position != closedPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, closedPosition, moveSpeed * Time.deltaTime);
                isMoving = true;
            }
        }

        // So de porta en moviment
        if (isMoving)
        {
            if (!audioSource.isPlaying && AudioManager.Instance.doorSound != null)
                audioSource.PlayOneShot(AudioManager.Instance.doorSound);
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    // Crida això des de la PressurePlate (amb UnityEvent)
    public void OpenDoor()
    {
        isOpening = true;
    }

    public void CloseDoor()
    {
        Debug.Log("Tancant porta");
        isOpening = false;
    }
}
