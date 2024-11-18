using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Public variables
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    // Private variables
    Rigidbody2D rigidbody2d;
    Animator animator;
    float timer;
    int direction = 1;
    bool aggressive = true;

    // Particle System for smoke
    public ParticleSystem smokeParticles;

    // Animation trigger for when the robot is fixed
    public string fixedAnimationTrigger = "Fixed";

    // Health system
    public int maxHealth = 2; // Enemy starts with 2 health
    private int currentHealth; // Tracks current health

    // Flag to check if the robot is fixed
    bool isFixed = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;

        currentHealth = maxHealth; // Initialize health

        // Start emitting smoke particles
        if (smokeParticles != null)
        {
            smokeParticles.Play();
        }
    }

    // Update is called every frame
    void Update()
    {
        if (isFixed)
        {
            // Stop moving and play the "fixed" animation once repaired
            if (smokeParticles != null && smokeParticles.isPlaying)
            {
                smokeParticles.Stop();
            }

            animator.SetTrigger(fixedAnimationTrigger); // Play the "fixed" animation
            return; // Skip the rest of the logic if fixed
        }

        if (!aggressive)
        {
            return;
        }

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    // FixedUpdate has the same call rate as the physics system
    void FixedUpdate()
    {
        if (isFixed || !aggressive)
        {
            return; // Do not move if fixed or not aggressive
        }

        Vector2 position = rigidbody2d.position;

        if (vertical)
        {
            position.y = position.y + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + speed * direction * Time.deltaTime;
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
        }

        rigidbody2d.MovePosition(position);
    }

    // OnCollisionEnter2D to handle damage to the player
    void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    // Method to fix the robot, stop the smoke, and play the fixed animation
    public void FixRobot()
    {
        isFixed = true;
        if (smokeParticles != null)
        {
            smokeParticles.Stop(); // Stop the smoke particle effect
        }
        animator.SetTrigger(fixedAnimationTrigger); // Trigger the "fixed" animation
    }

    // Method to handle damage and health change
    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        // Ensure health doesn't drop below 0
        currentHealth = Mathf.Max(0, currentHealth);

        if (currentHealth <= 0)
        {
            // Stop the enemy's movement when health reaches 0
            isFixed = true;
            aggressive = false; // Stop movement
            if (smokeParticles != null && smokeParticles.isPlaying)
            {
                smokeParticles.Stop(); // Stop smoke when "dead"
            }
            animator.SetTrigger("Dead"); // Trigger "dead" animation
            Debug.Log("Enemy is destroyed");
        }
    }
}
