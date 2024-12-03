using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    Rigidbody2D rigidbody2d;
    Animator animator;
    float timer;
    int direction = 1;
    bool aggressive = true;

    public ParticleSystem smokeParticles;
    public string fixedAnimationTrigger = "Fixed";

    public int maxHealth = 2; 
    private int currentHealth; 

    bool isFixed = false;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        timer = changeTime;

        currentHealth = maxHealth;

        if (smokeParticles != null)
        {
            smokeParticles.Play();
        }
    }

    void Update()
    {
        if (isFixed)
        {
            if (smokeParticles != null && smokeParticles.isPlaying)
            {
                smokeParticles.Stop();
            }

            animator.SetTrigger(fixedAnimationTrigger);
            return;
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

    void FixedUpdate()
    {
        if (isFixed || !aggressive)
        {
            return;
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

    void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.ChangeHealth(-1);
        }
    }

    public void FixRobot()
    {
        isFixed = true;
        if (smokeParticles != null)
        {
            smokeParticles.Stop();
        }
        animator.SetTrigger(fixedAnimationTrigger);
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;

        currentHealth = Mathf.Max(0, currentHealth);

        if (currentHealth <= 0 && !isFixed)
        {
            isFixed = true;
            aggressive = false;
            if (smokeParticles != null && smokeParticles.isPlaying)
            {
                smokeParticles.Stop();
            }
            animator.SetTrigger("Dead");
            Debug.Log("Enemy is destroyed");

            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
            
            }
        }
    }
}
