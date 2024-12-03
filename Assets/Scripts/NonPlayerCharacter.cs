using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    // Define the properties for the NPC
    public string npcName;
    public string[] dialogueLines;

    // AudioSource and AudioClip for interaction sound
    private AudioSource audioSource;  // Reference to the AudioSource component
    public AudioClip interactionSound;  // Interaction sound to play

    void Awake()
    {
        // Ensure an AudioSource is attached to the NPC GameObject
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not present
        }
    }

    // Method to start interaction with the NPC
    public void Interact()
    {
        Debug.Log("Interacting with " + npcName);

        // Play the interaction sound
        PlayInteractionSound();

        // Display dialogue or perform other actions
        foreach (string line in dialogueLines)
        {
            Debug.Log(line);
        }
    }

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
}