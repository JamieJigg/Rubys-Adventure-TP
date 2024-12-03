using System.Collections;
using UnityEngine;

public class SuperPowerUp : MonoBehaviour
{
    // Duration of the super power-up effect
    public float duration = 10.0f;

    // Audio clip to play on power-up collection
    public AudioClip collectSound;

    private AudioSource audioSource;

    void Start()
    {
        // Add an AudioSource component if one doesn't exist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure the AudioSource (optional adjustments)
        audioSource.playOnAwake = false; // Prevent playing on start
        audioSource.spatialBlend = 0f;  // Make it 2D sound
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player collects the power-up
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Activate the special shooting ability on the player
                playerController.isSuperPowerUpEnabled = true;  // Enable the super power-up

                // Optionally, deactivate after the specified duration
                StartCoroutine(DeactivateSuperPowerAfterDuration(playerController));
            }

            // Play the collection sound
            PlayCollectSound();

            // Destroy the power-up object after a short delay to allow the sound to play
            Destroy(gameObject, 0.1f);
        }
    }

    private void PlayCollectSound()
    {
        // Play the collection sound if available
        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
        else
        {
            Debug.LogWarning("Collect sound or AudioSource not assigned!");
        }
    }

    private IEnumerator DeactivateSuperPowerAfterDuration(PlayerController playerController)
    {
        // Wait for the duration of the super power-up
        yield return new WaitForSeconds(duration);

        // Deactivate the super power-up effect
        playerController.isSuperPowerUpEnabled = false;
    }
}