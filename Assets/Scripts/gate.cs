using UnityEngine;

[RequireComponent(typeof(Collider))]
public class gate : MonoBehaviour
{
    [Header("Gate Settings")]
    [Tooltip("Reference to the other gate in the pair")]
    public GameObject otherGate;
    [Tooltip("Power-up types that can be given")]
    public PowerUpType[] possiblePowerUps = { PowerUpType.DamageBoost, PowerUpType.AttackSpeedBoost, PowerUpType.DamageDecrease, PowerUpType.AttackSpeedDecrease };
    [Tooltip("Chance of getting a power-up vs downgrade (0-1)")]
    public float powerUpChance = 0.6f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Start combat with nearby enemy
            StartCombat(other.gameObject);
        }
    }

    void StartCombat(GameObject player)
    {
        // Find nearby enemy
        GameObject enemy = FindNearbyEnemy();
        
        if (enemy != null)
        {
            // Start combat
            player_combat playerCombat = player.GetComponent<player_combat>();
            if (playerCombat != null)
            {
                playerCombat.StartCombat(enemy);
                
                // Give power-up after combat ends
                StartCoroutine(GivePowerUpAfterCombat(player));
            }
        }
        else
        {
            Debug.LogWarning("No enemy found for combat!");
        }

        // Destroy both gates
        DestroyBothGates();
    }

    System.Collections.IEnumerator GivePowerUpAfterCombat(GameObject player)
    {
        // Wait for combat to end
        player_combat playerCombat = player.GetComponent<player_combat>();
        while (playerCombat != null && playerCombat.isInCombat)
        {
            yield return null;
        }

        // Give power-up after combat
        powerup_manager powerUpManager = player.GetComponent<powerup_manager>();
        if (powerUpManager != null)
        {
            PowerUpType randomPowerUp = GetRandomPowerUp();
            powerUpManager.ApplyPowerUp(randomPowerUp);
        }
    }

    GameObject FindNearbyEnemy()
    {
        // Find the closest enemy
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float closestDistance = float.MaxValue;

        Debug.Log($"Looking for enemies. Found {enemies.Length} enemies.");

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            Debug.Log($"Enemy at {enemy.transform.position}, distance: {distance}");
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            Debug.Log($"Found closest enemy at distance {closestDistance}");
        }
        else
        {
            Debug.LogWarning("No enemies found!");
        }

        return closestEnemy;
    }

    PowerUpType GetRandomPowerUp()
    {
        if (possiblePowerUps.Length == 0) return PowerUpType.DamageBoost;

        // Determine if it's a power-up or downgrade
        bool isPowerUp = Random.Range(0f, 1f) < powerUpChance;
        
        if (isPowerUp)
        {
            // Choose from power-up types (first two in enum)
            PowerUpType[] powerUps = { PowerUpType.DamageBoost, PowerUpType.AttackSpeedBoost };
            return powerUps[Random.Range(0, powerUps.Length)];
        }
        else
        {
            // Choose from downgrade types (last two in enum)
            PowerUpType[] downgrades = { PowerUpType.DamageDecrease, PowerUpType.AttackSpeedDecrease };
            return downgrades[Random.Range(0, downgrades.Length)];
        }
    }

    void DestroyBothGates()
    {
        // Destroy the other gate first
        if (otherGate != null)
        {
            Destroy(otherGate);
        }
        
        // Destroy this gate
        Destroy(gameObject);
    }

    // Method to set the other gate reference (called by spawner)
    public void SetOtherGate(GameObject other)
    {
        otherGate = other;
    }
}