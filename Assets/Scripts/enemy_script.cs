using UnityEngine;

public class enemy_script : MonoBehaviour
{
    [Header("Enemy Stats")]
    [Tooltip("Maximum health points")]
    public float maxHealth = 50f;
    [Tooltip("Current health points")]
    public float currentHealth;
    [Tooltip("Damage dealt to player if not defeated")]
    public float damage = 20f;
    [Tooltip("Attack range")]
    public float attackRange = 5f;
    [Tooltip("Attack cooldown in seconds")]
    public float attackCooldown = 2f;

    [Header("Enemy Settings")]
    [Tooltip("Is this enemy currently attacking?")]
    public bool isAttacking = false;
    [Tooltip("Player reference")]
    public GameObject player;

    private float lastAttackTime = 0f;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        
        // Find player if not assigned
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
        
        // Ensure enemy has a collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError($"Enemy {gameObject.name} needs a Collider component!");
        }
        else
        {
            Debug.Log($"Enemy {gameObject.name} has collider: {col.GetType().Name}, isTrigger: {col.isTrigger}");
        }
    }

    void Update()
    {
        if (isDead) return;

        // Check if player is in range and attack
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                AttackPlayer();
                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void AttackPlayer()
    {
        if (isDead) return;

        isAttacking = true;
        Debug.Log("Enemy attacks player!");

        // Get player combat script and deal damage
        player_combat playerCombat = player.GetComponent<player_combat>();
        if (playerCombat != null)
        {
            // This would trigger game over in a real implementation
            // For now, just log it
            Debug.Log($"Player takes {damage} damage from enemy!");
        }

        // End attacking after a short time
        Invoke("EndAttack", 0.5f);
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Enemy defeated!");

        // Notify player combat that enemy is dead
        if (player != null)
        {
            player_combat playerCombat = player.GetComponent<player_combat>();
            if (playerCombat != null)
            {
                playerCombat.EndCombat();
            }
        }

        // Destroy enemy
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}
