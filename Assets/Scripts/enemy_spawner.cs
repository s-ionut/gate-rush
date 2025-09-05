using UnityEngine;

public class enemy_spawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("Enemy prefab to spawn")]
    public GameObject enemyPrefab;
    [Tooltip("Base health for enemies")]
    public float baseHealth = 50f;
    [Tooltip("Health increase per gate")]
    public float healthIncrease = 25f;
    [Tooltip("Base damage for enemies")]
    public float baseDamage = 20f;
    [Tooltip("Damage increase per gate")]
    public float damageIncrease = 5f;

    [Header("Spawn Settings")]
    [Tooltip("Distance between gates where enemies spawn (should match gate spawn interval)")]
    public float spawnDistance = 30f;
    [Tooltip("Z-offset for enemy spawn positions")]
    public float zOffset = 0f;
    [Tooltip("Offset from gate position to spawn enemy between gates")]
    public float betweenGatesOffset = 35f;

    private Transform player;
    private float lastSpawnX = 0f;
    private int gateCount = 0;
    private checkpoint_spawner gateSpawner;
    private bool hasFirstGateSpawned = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Enemy Spawner: Player not found!");
            enabled = false;
        }

        // Find the gate spawner to sync with gate spawning
        gateSpawner = FindObjectOfType<checkpoint_spawner>();
        if (gateSpawner != null)
        {
            // Sync spawn distance with gate spawn interval
            spawnDistance = gateSpawner.spawnInterval;
        }
    }

    void Update()
    {
        // Wait for player to reach the first gate spawn distance before starting enemy spawning
        if (!hasFirstGateSpawned)
        {
            // Gates spawn when player reaches spawnInterval (30f), so wait for that
            float firstGateDistance = gateSpawner != null ? gateSpawner.spawnInterval : 30f;
            if (player.position.x >= firstGateDistance)
            {
                hasFirstGateSpawned = true;
                lastSpawnX = player.position.x; // Set this so enemies spawn after the first gate
                Debug.Log("Player reached first gate spawn distance, starting enemy spawning");
            }
            return;
        }

        // Spawn enemies when player moves forward
        // Spawn enemies at the same interval as gates, but offset to be between them
        if (player.position.x >= lastSpawnX + spawnDistance)
        {
            SpawnEnemy();
            lastSpawnX = player.position.x;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;
        
        float spawnX = player.position.x + (gateSpawner != null ? gateSpawner.aheadOffset - betweenGatesOffset : 25f);
        Vector3 spawnPos = new Vector3(spawnX, 1f, zOffset);
        Quaternion rot = Quaternion.Euler(0f, 180f, 0f);

        // Create enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, rot);
        
        // Configure enemy stats based on gate count
        enemy_script enemyScript = enemy.GetComponent<enemy_script>();
        if (enemyScript != null)
        {
            // Increase difficulty with each gate
            float health = baseHealth + (gateCount * healthIncrease);
            float damage = baseDamage + (gateCount * damageIncrease);
            
            enemyScript.maxHealth = health;
            enemyScript.currentHealth = health;
            enemyScript.damage = damage;
            
            Debug.Log($"Spawned enemy with {health} health and {damage} damage (Gate #{gateCount + 1}) at position {spawnPos}");
        }

        gateCount++;
    }

    public void ResetSpawner()
    {
        gateCount = 0;
        lastSpawnX = 0f;
        hasFirstGateSpawned = false;
    }

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
        if (player != null)
        {
            lastSpawnX = player.position.x;
        }
    }
}
