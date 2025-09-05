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

    [Header("Health Bar Settings")]
    [Tooltip("Health bar width")]
    public float healthBarWidth = 1.5f;
    [Tooltip("Health bar height")]
    public float healthBarHeight = 0.15f;
    [Tooltip("Health bar Y offset above enemy")]
    public float healthBarYOffset = 1.5f;

    [Header("Arrow Settings")]
    [Tooltip("Arrow prefab to shoot")]
    public GameObject arrowPrefab;
    [Tooltip("Arrow spawn point")]
    public Transform arrowSpawnPoint;
    [Tooltip("Arrow speed")]
    public float arrowSpeed = 10f;
    [Tooltip("Arrow damage")]
    public float arrowDamage = 15f;
    [Tooltip("Shooting range")]
    public float shootingRange = 8f;
    [Tooltip("Shooting cooldown")]
    public float shootingCooldown = 3f;

    private float lastAttackTime = 0f;
    private float lastShootTime = 0f;
    private bool isDead = false;
    private health_bar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        
        // Find player if not assigned
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
        
        // Create health bar at runtime
        CreateHealthBar();
        
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

    void CreateHealthBar()
    {
        // Create a child GameObject for the health bar
        GameObject healthBarObject = new GameObject("HealthBar");
        healthBarObject.transform.SetParent(transform);
        healthBarObject.transform.localPosition = Vector3.zero;

        // Add the health_bar component
        healthBar = healthBarObject.AddComponent<health_bar>();
        
        // Configure health bar settings
        healthBar.healthBarWidth = healthBarWidth;
        healthBar.healthBarHeight = healthBarHeight;
        healthBar.healthBarYOffset = healthBarYOffset;
        healthBar.currentHealthColor = Color.red;
        healthBar.missingHealthColor = Color.white;

        // Set initial health values
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);
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

        // Check if player is in shooting range and shoot arrows
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= shootingRange)
            {
                if (Time.time - lastShootTime >= shootingCooldown)
                {
                    Debug.Log($"Enemy in shooting range! Distance: {distanceToPlayer}, Range: {shootingRange}");
                    ShootArrow();
                    lastShootTime = Time.time;
                }
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log($"Enemy took {damageAmount} damage. Health: {currentHealth}/{maxHealth}");

        // Update health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

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

    void ShootArrow()
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning("Enemy cannot shoot: No arrow prefab assigned!");
            return;
        }
        
        if (player == null)
        {
            Debug.LogWarning("Enemy cannot shoot: No player found!");
            return;
        }

        // Create arrow spawn point if not assigned
        Vector3 spawnPosition = transform.position + Vector3.up * 1f;
        if (arrowSpawnPoint != null)
        {
            spawnPosition = arrowSpawnPoint.position;
        }

        // Create arrow
        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
        
        // Set arrow properties
        arrow_script arrowScript = arrow.GetComponent<arrow_script>();
        if (arrowScript != null)
        {
            arrowScript.SetDamage(arrowDamage);
            arrowScript.SetTarget(player);
            arrowScript.SetSpeed(arrowSpeed);
        }
        else
        {
            Debug.LogError("Enemy arrow prefab doesn't have arrow_script component!");
        }

        // Tag the arrow as EnemyArrow
        arrow.tag = "EnemyArrow";
        
        Debug.Log($"Enemy shot arrow at player for {arrowDamage} damage! Arrow position: {spawnPosition}, Player position: {player.transform.position}");
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
