using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource footstepAudioSource;  // Reference to the AudioSource component for footstep sounds
    public AudioClip footstepSound;          // The footstep sound clip
    public float stepInterval = 0.5f;       // Interval in seconds between each footstep sound
    private float stepCooldown;             // Cooldown timer to control footstep intervals

    private Vector2 lastPosition;           // To track the player's last position

    void Start()
    {
        lastPosition = transform.position;  // Initialize the last position
        stepCooldown = stepInterval;        // Set the cooldown interval for the footstep sounds
    }

    void Update()
    {
        // Check if the player is moving using WASD keys
        float moveX = Input.GetAxisRaw("Horizontal"); // A/D keys (left/right)
        float moveY = Input.GetAxisRaw("Vertical");   // W/S keys (up/down)

        // If there's any movement and the cooldown period has passed, play the footstep sound
        if ((moveX != 0 || moveY != 0) && stepCooldown <= 0f)
        {
            PlayFootstepSound();  // Play footstep sound
            stepCooldown = stepInterval;  // Reset cooldown timer
        }

        // Update the cooldown timer
        stepCooldown -= Time.deltaTime;
    }

    void PlayFootstepSound()
    {
        // Check if AudioSource and AudioClip are assigned
        if (footstepAudioSource != null && footstepSound != null)
        {
            footstepAudioSource.PlayOneShot(footstepSound);  // Play footstep sound once
        }
        else
        {
            Debug.LogWarning("Footstep AudioSource or AudioClip is missing!");
        }
    }
}