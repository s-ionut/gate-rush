using UnityEngine;

public class player_health : MonoBehaviour
{
    [Header("Health Settings")]
    [Tooltip("Maximum health points")]
    public float maxHealth = 100f;
    [Tooltip("Current health points")]
    public float currentHealth;
    [Tooltip("Is the player dead?")]
    public bool isDead = false;

    [Header("Health Bar Settings")]
    [Tooltip("Health bar width")]
    public float healthBarWidth = 2f;
    [Tooltip("Health bar height")]
    public float healthBarHeight = 0.2f;
    [Tooltip("Health bar Y offset above player")]
    public float healthBarYOffset = 2f;

    private health_bar healthBar;
    private game_manager gameManager;

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        
        // Ensure health is valid
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
        }

        // Create health bar at runtime
        CreateHealthBar();

        // Find game manager
        gameManager = FindObjectOfType<game_manager>();
        if (gameManager == null)
        {
            Debug.LogWarning("Player Health: No game manager found!");
        }

        Debug.Log($"Player health initialized: {currentHealth}/{maxHealth}");
    }

    void Update()
    {
        // Test damage with T key
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T key pressed - testing player damage!");
            TakeDamage(10f);
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

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);

        // Update health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        Debug.Log($"Player took {damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);

        // Update health bar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        Debug.Log($"Player healed {healAmount}. Health: {currentHealth}/{maxHealth}");
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.LogError($"Player died! Health: {currentHealth}/{maxHealth}");

        // Try to find game manager if not found
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<game_manager>();
            Debug.Log($"Game manager found: {gameManager != null}");
        }

        // Trigger game over
        if (gameManager != null)
        {
            Debug.Log("Calling gameManager.GameOver()");
            gameManager.GameOver();
        }
        else
        {
            Debug.LogError("No game manager found to trigger game over!");
        }
    }

    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
        
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        Debug.Log($"Player health reset: {currentHealth}/{maxHealth}");
    }

    public float GetHealthPercentage()
    {
        return maxHealth > 0 ? currentHealth / maxHealth : 0f;
    }

    public bool IsDead()
    {
        return isDead;
    }

    // Test method to manually damage player (for debugging)
    [ContextMenu("Test Take Damage")]
    public void TestTakeDamage()
    {
        TakeDamage(10f);
    }
}
