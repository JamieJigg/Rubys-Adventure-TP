using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameManagerScript gameManager;
    public InputAction MoveAction;
    Rigidbody2D rigidbody2d;
    Vector2 move;
    public float speed = 3.0f;

    public int maxHealth = 5;
    int currentHealth;
    public int health { get { return currentHealth; } }

    public float timeInvinvible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;
    public InputAction launchAction;

    AudioSource audioSource;

    public InputAction talkAction;

    public bool isDead = false;

    public ParticleSystem healthGainParticles;
    public ParticleSystem damageTakenParticles;

    void Start()
    {
        MoveAction.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        launchAction.Enable();
        launchAction.performed += Launch;

        talkAction.Enable();
        talkAction.performed += FindFriend;

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return; // Prevent movement and other actions if dead

        move = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }

        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        // Handle death and restart logic
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true; // Mark the player as dead
            gameManager?.gameOver(); // Call GameManager's gameOver method if it's assigned
            Debug.Log("Dead! Press 'R' to restart.");
        }

        // Wait for 'R' key to restart
        if (isDead && Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Restarting game...");
            RestartGame();
        }
    } // <-- Correctly added the closing brace for Update

    void FixedUpdate()
    {
        if (isDead) return; // Don't move if dead

        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0) // Damage
        {
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            damageCooldown = timeInvinvible;
            animator.SetTrigger("Hit");

            if (damageTakenParticles != null)
            {
                damageTakenParticles.transform.position = transform.position;
                damageTakenParticles.Play();
                Destroy(damageTakenParticles.gameObject, damageTakenParticles.main.duration);
            }
        }
        else if (amount > 0) // Health gain
        {
            if (healthGainParticles != null)
            {
                healthGainParticles.transform.position = transform.position;
                healthGainParticles.Play();
                Destroy(healthGainParticles.gameObject, healthGainParticles.main.duration);
            }
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth / (float)maxHealth);
    }

    void Launch(InputAction.CallbackContext context)
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(moveDirection, 300f);

        animator.SetTrigger("Launch");
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void FindFriend(InputAction.CallbackContext context)
    {
        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, moveDirection, 1.5f, LayerMask.GetMask("NPC"));

        if (hit.collider != null)
        {
            NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
            if (character != null)
            {
                UIHandler.instance.DisplayDialogue();
            }
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
