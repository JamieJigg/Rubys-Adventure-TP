using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    // AudioClip for shooting sound
    public AudioClip shootSound;
    private AudioSource audioSource;

    // Awake is called when the Projectile GameObject is instantiated
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        // Ensure there's an AudioSource on the projectile
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource if not present
        }
    }

    void Update()
    {
        // Destroy the projectile if it goes too far
        if (transform.position.magnitude > 100.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);

        // Play the shoot sound when the projectile is fired
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);  // Play the shoot sound once
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // Check if the projectile hits an enemy
        EnemyController enemy = other.collider.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.ChangeHealth(-1); // Reduce enemy health by 1
        }

        // Destroy the projectile after collision
        Destroy(gameObject);
    }
}
