using System.Collections;
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

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float damageCooldown;

    Animator animator;
    Vector2 moveDirection = new Vector2(1, 0);

    public GameObject projectilePrefab;
    public Transform firePoint; // Point where projectiles are fired from
    public InputAction launchAction;

    public bool isSpreadShotEnabled = false; // Spread shot toggle
    public bool isSuperPowerUpEnabled = false; // Flag for the super power-up
    public float projectileForce = 300f; // Default projectile force

    AudioSource audioSource;

    public InputAction talkAction;

    public bool isDead = false;

    public ParticleSystem healthGainParticles;
    public ParticleSystem damageTakenParticles;

    // AudioClip for shooting sound
    public AudioClip shootSound;

    void Start()
    {
        // Enable the move and launch actions
        MoveAction.Enable();
        launchAction.Enable();

        // Get references to required components
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Set up action listeners
        launchAction.performed += Launch;
        talkAction.Enable();
        talkAction.performed += FindFriend;

        // Initialize health
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

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

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            gameManager?.gameOver();
            Debug.Log("Dead! Press 'R' to restart.");
        }

        if (isDead && Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Restarting game...");
            RestartGame();
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    void Launch(InputAction.CallbackContext context)
    {
        if (isSuperPowerUpEnabled)
        {
            // Shoot 4 projectiles in each direction (Up, Down, Left, Right) - spread shots
            FireSpreadShot(Vector2.up, 4);
            FireSpreadShot(Vector2.down, 4);
            FireSpreadShot(Vector2.left, 4);
            FireSpreadShot(Vector2.right, 4);
        }
        else if (isSpreadShotEnabled)
        {
            FireProjectile(Vector2.up);
            FireProjectile(Vector2.down);
            FireProjectile(Vector2.left);
            FireProjectile(Vector2.right);
        }
        else
        {
            FireProjectile(moveDirection);
        }

        // Play shooting sound
        PlaySound(shootSound);

        animator.SetTrigger("Launch");
    }

    private void FireSpreadShot(Vector2 direction, int numberOfProjectiles)
    {
        float spreadAngle = 10f; 
        float startingAngle = -spreadAngle * (numberOfProjectiles - 1) / 2;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float angle = startingAngle + i * spreadAngle;
            Vector2 spreadDirection = RotateVector(direction, angle);
            FireProjectile(spreadDirection);
        }
    }

    private Vector2 RotateVector(Vector2 vector, float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);

        float x = cos * vector.x - sin * vector.y;
        float y = sin * vector.x + cos * vector.y;
        return new Vector2(x, y);
    }

    private void FireProjectile(Vector2 direction)
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = projectileObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Launch(direction, projectileForce);
            }
        }
        else
        {
            Debug.LogError("projectilePrefab or firePoint is not assigned.");
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogError("audioSource or clip is not assigned.");
        }
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0) 
        {
            if (isInvincible) return;

            isInvincible = true;
            damageCooldown = timeInvincible;

            if (damageTakenParticles != null)
            {
                damageTakenParticles.Play();
            }
        }
        else if (amount > 0) 
        {
            if (healthGainParticles != null)
            {
                healthGainParticles.Play();
            }
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"Health changed by {amount}. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            isDead = true;
            gameManager?.gameOver();
            Debug.Log("Player is dead.");
        }
    }

    public void ActivateSuperPower()
    {
        isSuperPowerUpEnabled = true;
        StartCoroutine(DeactivateSuperPowerAfterDuration());
    }

    private IEnumerator DeactivateSuperPowerAfterDuration()
    {
        yield return new WaitForSeconds(10f);
        isSuperPowerUpEnabled = false;
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
                character.Interact();
            }
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
