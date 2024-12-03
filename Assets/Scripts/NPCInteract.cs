using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    // Maximum distance for interaction
    public float interactDistance = 2.0f;

    // AudioSource and AudioClip for footstep or interaction sound
    public AudioSource audioSource;  // Reference to the AudioSource
    public AudioClip interactionSound;  // Sound to play when interaction happens

    void Update()
    {
        // Check if the player presses the X key
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("X key pressed.");

            // Perform interaction logic
            if (IsPlayerCloseEnough())
            {
                // Play interaction sound if close
                PlayInteractionSound();
                Debug.Log("IT WORKED IT WORKED IT WORKED");  // Display the success message
            }
            else
            {
                Debug.Log("Player not in range.");
            }
        }
    }

    private bool IsPlayerCloseEnough()
    {
        // Find the player using their tag
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Player object not found! Is the Player tag assigned?");
            return false;
        }

        // Calculate the distance to the player
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        Debug.Log($"Distance to Player: {distanceToPlayer}");

        // Check if the player is within the interaction distance
        return distanceToPlayer <= interactDistance;
    }

    // Play interaction sound
    private void PlayInteractionSound()
    {
        if (audioSource != null && interactionSound != null)
        {
            audioSource.PlayOneShot(interactionSound);  // Play the interaction sound once
        }
        else
        {
            Debug.LogWarning("AudioSource or InteractionSound is not assigned!");
        }
    }

    void OnDrawGizmos()
    {
        // Visualize interaction range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }
}
