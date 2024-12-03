using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public float duration = 10.0f; // Duration of the power-up effect

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
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                StartCoroutine(ActivateSpreadShot(playerController));
            }

            // Play the collection sound
            PlayCollectSound();

            // Display the message in the console
            Debug.Log("YEAH THIS WORKS I HOPE");

            // Destroy the power-up object
            Destroy(gameObject, 0.1f); // Delay destruction to allow audio to play
        }
    }

    private IEnumerator ActivateSpreadShot(PlayerController playerController)
    {
        // Enable spread shot
        playerController.isSpreadShotEnabled = true;

        // Wait for the duration of the power-up
        yield return new WaitForSeconds(duration);

        // Disable spread shot
        playerController.isSpreadShotEnabled = false;
    }

    private void PlayCollectSound()
    {
        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
        else
        {
            Debug.LogWarning("Collect sound or AudioSource not assigned!");
        }
    }
}