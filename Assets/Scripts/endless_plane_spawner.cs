using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns and reuses plane segments to create an endless world along the X-axis.
/// Segments are instantiated ahead of the player and recycled behind.
/// </summary>
public class endless_plane_spawner : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Transform of the player moving along world X-axis.")]
    public Transform player;

    [Header("Plane Segment Settings")]
    [Tooltip("Prefab of the plane segment to spawn.")]
    public GameObject planePrefab;
    [Tooltip("Number of segments to maintain in the world.")]
    public int segmentCount = 5;
    [Tooltip("Length of each segment along the X-axis (world units).")]
    public float segmentLength = 10f;

    // Pool of active segments
    private readonly Queue<GameObject> segmentPool = new Queue<GameObject>();
    // X-position at which to spawn the next segment
    private float nextSpawnX;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("endless_plane_spawner: No player assigned or tagged. Please assign in Inspector or tag 'Player'.");
            enabled = false;
            return;
        }
        if (planePrefab == null)
        {
            Debug.LogError("endless_plane_spawner: No plane prefab assigned.");
            enabled = false;
            return;
        }

        // Initialize pool and spawn initial segments
        nextSpawnX = player.position.x;
        for (int i = 0; i < segmentCount; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        // Spawn a new segment when the player approaches the end of the last one
        if (player.position.x + segmentCount * segmentLength > nextSpawnX - segmentLength)
        {
            SpawnSegment();
        }
    }

    private void SpawnSegment()
    {
        Vector3 spawnPos = new Vector3(nextSpawnX, 0f, 0f);
        GameObject segment = Instantiate(planePrefab, spawnPos, Quaternion.identity, transform);
        segmentPool.Enqueue(segment);

        // If pool exceeds desired count, destroy the oldest segment behind the player
        if (segmentPool.Count > segmentCount)
        {
            GameObject old = segmentPool.Dequeue();
            Destroy(old);
        }

        nextSpawnX += segmentLength;
    }

    public void ResetSpawner()
    {
        // Destroy all existing segments
        foreach (GameObject segment in segmentPool)
        {
            if (segment != null)
            {
                Destroy(segment);
            }
        }
        segmentPool.Clear();

        // Reset spawn position
        if (player != null)
        {
            nextSpawnX = player.position.x;
        }
        else
        {
            nextSpawnX = 0f;
        }

        // Spawn initial segments again
        for (int i = 0; i < segmentCount; i++)
        {
            SpawnSegment();
        }

        Debug.Log("Endless plane spawner reset");
    }
}
