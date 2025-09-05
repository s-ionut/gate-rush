using UnityEngine;

public class player_combat : MonoBehaviour
{
    [Header("Combat Stats")]
    [Tooltip("Base attack damage")]
    public float baseDamage = 10f;
    [Tooltip("Current attack damage (modified by power-ups)")]
    public float currentDamage = 10f;
    [Tooltip("Attack speed (arrows per second)")]
    public float attackSpeed = 2f;
    [Tooltip("Attack range")]
    public float attackRange = 10f;

    [Header("Arrow Settings")]
    [Tooltip("Arrow prefab to shoot")]
    public GameObject arrowPrefab;
    [Tooltip("Arrow spawn point")]
    public Transform arrowSpawnPoint;
    [Tooltip("Arrow speed")]
    public float arrowSpeed = 15f;

    [Header("Combat State")]
    [Tooltip("Is the player currently in combat?")]
    public bool isInCombat = false;
    [Tooltip("Current target enemy")]
    public GameObject currentTarget = null;

    private float lastAttackTime = 0f;
    private player_movement playerMovement;
    private powerup_manager powerUpManager;
    private player_health playerHealth;

    void Start()
    {
        playerMovement = GetComponent<player_movement>();
        powerUpManager = GetComponent<powerup_manager>();
        playerHealth = GetComponent<player_health>();
        
        // Debug player components
        Debug.Log($"Player Combat Start - Movement: {playerMovement != null}, PowerUp: {powerUpManager != null}, Health: {playerHealth != null}");
        
        // Check if player has collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("Player needs a Collider component for arrow collision detection!");
        }
        else
        {
            Debug.Log($"Player has collider: {col.GetType().Name}, isTrigger: {col.isTrigger}");
        }
        
        // Set initial damage
        currentDamage = baseDamage;
    }

    void Update()
    {
        if (isInCombat && currentTarget != null)
        {
            // Check if target is still alive and in range
            if (currentTarget.activeInHierarchy && Vector3.Distance(transform.position, currentTarget.transform.position) <= attackRange)
            {
                // Auto-attack based on attack speed
                if (Time.time - lastAttackTime >= 1f / attackSpeed)
                {
                    ShootArrow();
                    lastAttackTime = Time.time;
                }
            }
            else
            {
                // Target is dead or out of range, end combat
                EndCombat();
            }
        }
    }

    public void StartCombat(GameObject enemy)
    {
        isInCombat = true;
        currentTarget = enemy;
        
        // Stop player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        
        Debug.Log("Combat started with enemy!");
    }

    public void EndCombat()
    {
        isInCombat = false;
        currentTarget = null;
        
        // Resume player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
        
        Debug.Log("Combat ended!");
    }

    void ShootArrow()
    {
        if (arrowPrefab == null || arrowSpawnPoint == null || currentTarget == null) 
        {
            Debug.LogWarning($"Cannot shoot arrow: arrowPrefab={arrowPrefab != null}, spawnPoint={arrowSpawnPoint != null}, target={currentTarget != null}");
            return;
        }

        // Create arrow at spawn point
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
        
        // Tag the arrow as PlayerArrow
        arrow.tag = "PlayerArrow";
        
        // Set arrow properties
        arrow_script arrowScript = arrow.GetComponent<arrow_script>();
        if (arrowScript != null)
        {
            arrowScript.SetDamage(currentDamage);
            arrowScript.SetTarget(currentTarget);
            arrowScript.SetSpeed(arrowSpeed);
        }
        else
        {
            Debug.LogError("Arrow prefab doesn't have arrow_script component!");
        }
        
        Debug.Log($"Shot arrow for {currentDamage} damage at target {currentTarget.name}!");
    }

    public void ApplyDamagePowerUp(float damageIncrease)
    {
        currentDamage += damageIncrease;
        Debug.Log($"Damage increased by {damageIncrease}. New damage: {currentDamage}");
    }

    public void ApplySpeedPowerUp(float speedIncrease)
    {
        attackSpeed += speedIncrease;
        Debug.Log($"Attack speed increased by {speedIncrease}. New speed: {attackSpeed}");
    }

    public void ResetCombatStats()
    {
        currentDamage = baseDamage;
        attackSpeed = 2f;
    }

    public float GetCurrentDamage()
    {
        return currentDamage;
    }

    public float GetAttackSpeed()
    {
        return attackSpeed;
    }

    // Removed OnTriggerEnter - arrow_script.cs handles arrow collisions
}
