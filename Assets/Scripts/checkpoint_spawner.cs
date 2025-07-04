using UnityEngine;
/// <summary>
/// Spawns gate prefabs periodically ahead of the player, spaced to the left and right,
/// and destroys each when the player passes through.
/// </summary>
public class checkpoint_spawner : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Player transform (tag as 'Player' or assign manually)")]
    public Transform player;

    [Header("Gate Settings")]
    [Tooltip("Gate prefab with trigger collider and Gate script")]
    public GameObject gatePrefab;
    [Tooltip("Distance along X between successive gate spawns based on player's movement")]
    public float spawnInterval = 30f;
    [Tooltip("Distance ahead of the player to place the gates (so they spawn off-screen)")]
    public float aheadOffset = 50f;
    [Tooltip("Z-offset from center for left/right gate positions")]
    public float zOffset = 5f;

    // Tracks player's X at last spawn
    private float lastSpawnX;

    void Start()
    {
        // Auto-assign player by tag if not set
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("checkpoint_spawner: Player not assigned or tagged 'Player'.");
            enabled = false;
            return;
        }
        if (gatePrefab == null)
        {
            Debug.LogError("checkpoint_spawner: Gate prefab not assigned.");
            enabled = false;
            return;
        }

        lastSpawnX = player.position.x;
    }

    void Update()
    {
        // Spawn when player has moved beyond the next interval
        if (player.position.x >= lastSpawnX + spawnInterval)
        {
            SpawnGates();
            lastSpawnX = player.position.x;
        }
    }

    private void SpawnGates()
    {
        // Calculate spawn X ahead of player
        float spawnX = player.position.x + aheadOffset;
        Vector3 leftPos  = new Vector3(spawnX, 0f, -zOffset);
        Vector3 rightPos = new Vector3(spawnX, 0f,  zOffset);

        Instantiate(gatePrefab, leftPos,  Quaternion.identity);
        Instantiate(gatePrefab, rightPos, Quaternion.identity);
    }
}

/// <summary>
/// Gate script: destroys itself when the player triggers its collider.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Gate : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            Destroy(gameObject);
    }
}
