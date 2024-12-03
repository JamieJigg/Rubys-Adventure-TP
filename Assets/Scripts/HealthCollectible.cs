using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    public AudioClip collectedClip; // Audio clip to play on collection

    private AudioSource audioSource;

    void Start()
    {
        // Add or fetch the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure AudioSource settings
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Make it 2D sound
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController controller = other.GetComponent<PlayerController>();

        if (controller != null && controller.health < controller.maxHealth)
        {
            // Change player's health
            controller.ChangeHealth(1);

            // Play collection sound
            PlayCollectSound();

            // Destroy the collectible after a short delay to allow audio to play
            Destroy(gameObject, 0.1f);
        }
    }

    private void PlayCollectSound()
    {
        if (collectedClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectedClip);
        }
        else
        {
            Debug.LogWarning("CollectedClip or AudioSource is not assigned!");
        }
    }
}