using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool checkpointActivated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !checkpointActivated)
        {
            PlayerManager playerManager = other.GetComponent<PlayerManager>();
            if (playerManager != null)
            {
                playerManager.SetCurrentCheckpoint(transform);

                checkpointActivated = true;
                Debug.Log("Checkpoint activat!");
            }
        }
    }
}