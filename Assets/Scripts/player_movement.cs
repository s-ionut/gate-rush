using UnityEngine;

public class player_movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Forward movement speed (units per second)")]
    public float speed = 5f;

    [Header("Lane Switching")]
    [Tooltip("Distance between left and right lanes")]
    public float laneDistance = 3f;
    [Tooltip("Speed of lane switching transition")]
    public float laneSwitchSpeed = 10f;

    [Header("Bobbing Settings")]
    [Tooltip("Vertical bobbing amplitude")]
    public float bobAmplitude = 0.25f;
    [Tooltip("Bobbing frequency (cycles per second)")]
    public float bobFrequency = 2f;

    private float startY;
    private float startZ;
    private int currentLane = 0; // 0 = left lane, 1 = right lane
    private float targetZ;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Store the initial Y position for bobbing reference
        startY = transform.position.y;
        // Store the initial Z position for lane switching
        startZ = transform.position.z;
        // Set initial target position to left lane
        targetZ = startZ - (laneDistance / 2f);
        // Set initial position to left lane
        Vector3 pos = transform.position;
        pos.z = targetZ;
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        MoveForward();
        SwitchLanes();
        ApplyBobbing();
    }

    void HandleInput()
    {
        // Check for lane switching input
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchToLane(1); // Switch to left lane
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchToLane(0); // Switch to right lane
        }
    }

    void SwitchToLane(int lane)
    {
        if (lane != currentLane)
        {
            currentLane = lane;
            // Calculate target Z position based on lane
            // Lane 0 = left (negative Z), Lane 1 = right (positive Z)
            targetZ = startZ + (lane * laneDistance) - (laneDistance / 2f);
        }
    }

    void MoveForward()
    {
        // Move along world X-axis constantly (forward movement)
        Vector3 forwardMove = Vector3.right * speed * Time.deltaTime;
        transform.position += forwardMove;
    }

    void SwitchLanes()
    {
        // Smoothly move towards target lane position
        Vector3 pos = transform.position;
        pos.z = Mathf.Lerp(pos.z, targetZ, laneSwitchSpeed * Time.deltaTime);
        transform.position = pos;
    }

    void ApplyBobbing()
    {
        // Calculate bobbing offset based on time
        float newY = startY + Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f) * bobAmplitude;

        // Apply bobbing by setting the Y position
        Vector3 pos = transform.position;
        pos.y = newY;
        transform.position = pos;
    }
}
