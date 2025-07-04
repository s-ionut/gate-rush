using UnityEngine;

public class player_movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Forward movement speed (units per second)")]
    public float speed = 5f;

    [Header("Bobbing Settings")]
    [Tooltip("Vertical bobbing amplitude")]
    public float bobAmplitude = 0.25f;
    [Tooltip("Bobbing frequency (cycles per second)")]
    public float bobFrequency = 2f;

    private float startY;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store the initial Y position for bobbing reference
        startY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Move along world Z-axis constantly
        Vector3 forwardMove = Vector3.right * speed * Time.deltaTime;
        transform.position += forwardMove;

        // Calculate bobbing offset based on time
        float newY = startY + Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f) * bobAmplitude;

        // Apply bobbing by setting the Y position
        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;
    }
}
