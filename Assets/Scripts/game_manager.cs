using UnityEngine;
using UnityEngine.UI;

public class game_manager : MonoBehaviour
{
    [Header("Game State")]
    [Tooltip("Is the game currently paused?")]
    public bool isPaused = false;
    [Tooltip("Is the game over?")]
    public bool isGameOver = false;

    [Header("Player References")]
    [Tooltip("Player GameObject")]
    public GameObject player;
    [Tooltip("Enemy spawner")]
    public enemy_spawner enemySpawner;

    [Header("UI Settings")]
    [Tooltip("Font size for game over text")]
    public int fontSize = 48;
    [Tooltip("Button width")]
    public float buttonWidth = 200f;
    [Tooltip("Button height")]
    public float buttonHeight = 50f;

    private player_combat playerCombat;
    private player_movement playerMovement;
    private powerup_manager powerUpManager;
    private checkpoint_spawner checkpointSpawner;
    private camera_follow cameraFollow;
    private endless_plane_spawner endlessPlaneSpawner;

    void Start()
    {
        Debug.Log("Game Manager Start() called");
        
        // Find player if not assigned
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        // Get player components
        if (player != null)
        {
            playerCombat = player.GetComponent<player_combat>();
            playerMovement = player.GetComponent<player_movement>();
            powerUpManager = player.GetComponent<powerup_manager>();
        }

        // Find other components
        checkpointSpawner = FindObjectOfType<checkpoint_spawner>();
        cameraFollow = FindObjectOfType<camera_follow>();
        endlessPlaneSpawner = FindObjectOfType<endless_plane_spawner>();
        
        Debug.Log($"Game Manager initialized - Player: {player != null}, isGameOver: {isGameOver}");
    }

    void Update()
    {
        // Test game over with G key
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("G key pressed - testing game over UI");
            GameOver();
        }

        // Debug: Log every few seconds to see if Update is running
        if (Time.time % 5f < 0.1f)
        {
            Debug.Log($"Game Manager Update running - isGameOver: {isGameOver}");
        }

        // Check for game over conditions
        if (!isGameOver && playerCombat != null)
        {
            // Check if player is in combat and taking damage
            if (playerCombat.isInCombat)
            {
                // This would be triggered by enemy attacks in a real implementation
                // For now, we'll add a simple check
                CheckGameOverConditions();
            }
        }
    }

    void CheckGameOverConditions()
    {
        // Add your game over conditions here
        // For example: if player health <= 0, if enemy defeats player, etc.
        // This is a placeholder - you'll need to implement actual health system
    }

    public void GameOver()
    {
        if (isGameOver) 
        {
            Debug.Log("GameOver called but already game over");
            return;
        }

        Debug.Log("=== GAME OVER CALLED ===");
        isGameOver = true;
        isPaused = true;

        // Pause all game systems
        Time.timeScale = 0f;

        Debug.Log("Game Over! UI should be showing now.");
    }

    public void RetryGame()
    {
        Debug.Log("=== RETRY GAME CALLED ===");
        
        // Reset game state
        isGameOver = false;
        isPaused = false;
        Time.timeScale = 1f;

        Debug.Log($"Game state reset - isGameOver: {isGameOver}, isPaused: {isPaused}, timeScale: {Time.timeScale}");

        // Reset player
        if (player != null)
        {
            Debug.Log("Resetting player...");
            
            // Reset player position
            player.transform.position = Vector3.zero;
            Debug.Log($"Player position reset to: {player.transform.position}");
            
            // Reset player health
            player_health playerHealth = player.GetComponent<player_health>();
            if (playerHealth != null)
            {
                playerHealth.ResetHealth();
                Debug.Log("Player health reset");
            }
            else
            {
                Debug.LogWarning("Player health component not found!");
            }
            
            // Reset player stats
            if (powerUpManager != null)
            {
                powerUpManager.ResetPowerUps();
                Debug.Log("Power-ups reset");
            }
            
            if (playerCombat != null)
            {
                playerCombat.ResetCombatStats();
                Debug.Log("Combat stats reset");
            }
        }
        else
        {
            Debug.LogError("Player is null! Cannot reset player.");
        }

        // Reset spawners
        if (enemySpawner != null)
        {
            enemySpawner.ResetSpawner();
            Debug.Log("Enemy spawner reset");
        }

        if (checkpointSpawner != null)
        {
            // Reset checkpoint spawner if it has a reset method
            // checkpointSpawner.ResetSpawner();
        }

        if (endlessPlaneSpawner != null)
        {
            endlessPlaneSpawner.ResetSpawner();
            Debug.Log("Endless plane spawner reset");
        }

        // Reassign camera target
        if (cameraFollow != null && player != null)
        {
            cameraFollow.SetTarget(player.transform);
        }

        // Reassign spawner targets
        if (checkpointSpawner != null && player != null)
        {
            checkpointSpawner.SetPlayer(player.transform);
        }

        if (enemySpawner != null && player != null)
        {
            enemySpawner.SetPlayer(player.transform);
        }

        // Destroy all existing enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        // Destroy all existing gates
        GameObject[] gates = GameObject.FindGameObjectsWithTag("Gate");
        foreach (GameObject gate in gates)
        {
            Destroy(gate);
        }

        Debug.Log("Game Retried!");
    }

    public void PauseGame()
    {
        if (isGameOver) return;

        isPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (isGameOver) return;

        isPaused = false;
        Time.timeScale = 1f;
    }

    void OnGUI()
    {
        if (isGameOver)
        {
            // Create animated pulsing background
            float pulse = Mathf.Sin(Time.time * 3f) * 0.1f + 0.9f;
            GUI.color = new Color(0, 0, 0, 0.85f * pulse);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;

            // Calculate center position
            float centerX = Screen.width / 2f;
            float centerY = Screen.height / 2f;

            // Create modern gradient background panel
            DrawGradientPanel(centerX, centerY);

            // Create styles with better fonts
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 84;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.normal.textColor = new Color(1f, 0.2f, 0.2f, 1f); // Bright red

            GUIStyle subtitleStyle = new GUIStyle(GUI.skin.label);
            subtitleStyle.fontSize = 28;
            subtitleStyle.fontStyle = FontStyle.Italic;
            subtitleStyle.alignment = TextAnchor.MiddleCenter;
            subtitleStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f, 1f);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 32;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.alignment = TextAnchor.MiddleCenter;

            // Animated title with glow effect
            DrawGlowingText(centerX, centerY - 100f, "GAME OVER", titleStyle, 0.8f);

            // Subtitle with fade effect
            Rect subtitleRect = new Rect(centerX - 300f, centerY - 30f, 600f, 40f);
            GUI.Label(subtitleRect, "Your journey ends here...", subtitleStyle);

            // Simple retry button that definitely works
            Rect buttonRect = new Rect(centerX - 100f, centerY + 40f, 200f, 50f);
            
            // Draw button background
            GUI.color = new Color(0.2f, 0.6f, 0.2f, 1f);
            GUI.Box(buttonRect, "");
            
            // Draw button text
            GUI.color = Color.white;
            GUI.Label(buttonRect, "TRY AGAIN", buttonStyle);
            
            // Check for click
            if (GUI.Button(buttonRect, ""))
            {
                Debug.Log("TRY AGAIN button clicked!");
                RetryGame();
            }
            
            GUI.color = Color.white;

            // Decorative elements
            DrawModernDecorations(centerX, centerY);
        }
    }

    void DrawGradientPanel(float centerX, float centerY)
    {
        // Create a modern gradient panel background
        Rect panelRect = new Rect(centerX - 350f, centerY - 250f, 700f, 500f);
        
        // Outer glow
        GUI.color = new Color(1f, 0.3f, 0.3f, 0.1f);
        GUI.DrawTexture(new Rect(panelRect.x - 5, panelRect.y - 5, panelRect.width + 10, panelRect.height + 10), Texture2D.whiteTexture);
        
        // Main panel with gradient effect
        GUI.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
        GUI.DrawTexture(panelRect, Texture2D.whiteTexture);
        
        // Inner border
        GUI.color = new Color(0.3f, 0.3f, 0.4f, 0.8f);
        GUI.DrawTexture(new Rect(panelRect.x + 2, panelRect.y + 2, panelRect.width - 4, 2f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(panelRect.x + 2, panelRect.y + panelRect.height - 4, panelRect.width - 4, 2f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(panelRect.x + 2, panelRect.y + 2, 2f, panelRect.height - 4), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(panelRect.x + panelRect.width - 4, panelRect.y + 2, 2f, panelRect.height - 4), Texture2D.whiteTexture);
        
        GUI.color = Color.white;
    }

    void DrawGlowingText(float centerX, float centerY, string text, GUIStyle style, float intensity)
    {
        // Multiple shadow layers for glow effect
        Vector2 textSize = style.CalcSize(new GUIContent(text));
        Rect textRect = new Rect(centerX - textSize.x / 2f, centerY - textSize.y / 2f, textSize.x, textSize.y);
        
        // Outer glow (red)
        GUI.color = new Color(1f, 0.2f, 0.2f, 0.3f * intensity);
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            float offsetX = Mathf.Cos(angle) * 3f;
            float offsetY = Mathf.Sin(angle) * 3f;
            GUI.Label(new Rect(textRect.x + offsetX, textRect.y + offsetY, textRect.width, textRect.height), text, style);
        }
        
        // Inner glow (white)
        GUI.color = new Color(1f, 1f, 1f, 0.6f * intensity);
        GUI.Label(new Rect(textRect.x + 1, textRect.y + 1, textRect.width, textRect.height), text, style);
        
        // Main text
        GUI.color = style.normal.textColor;
        GUI.Label(textRect, text, style);
        GUI.color = Color.white;
    }

    void DrawModernButton(float centerX, float centerY, string text, GUIStyle style)
    {
        Rect buttonRect = new Rect(centerX - 150f, centerY - 25f, 300f, 50f);
        
        // Button shadow
        GUI.color = new Color(0, 0, 0, 0.4f);
        GUI.Button(new Rect(buttonRect.x + 3, buttonRect.y + 3, buttonRect.width, buttonRect.height), "", style);
        
        // Button background with gradient
        GUI.color = new Color(0.2f, 0.6f, 0.2f, 1f);
        GUI.Button(new Rect(buttonRect.x, buttonRect.y, buttonRect.width, buttonRect.height), "", style);
        
        // Button highlight
        GUI.color = new Color(0.3f, 0.8f, 0.3f, 0.7f);
        GUI.DrawTexture(new Rect(buttonRect.x + 2, buttonRect.y + 2, buttonRect.width - 4, 15f), Texture2D.whiteTexture);
        
        // Button text
        GUI.color = Color.white;
        GUI.Label(buttonRect, text, style);
        
        // Check for click - use the actual button instead of invisible overlay
        GUI.color = new Color(1f, 1f, 1f, 0.01f); // Almost transparent but still clickable
        if (GUI.Button(buttonRect, "", GUIStyle.none))
        {
            Debug.Log("TRY AGAIN button clicked!");
            RetryGame();
        }
        GUI.color = Color.white;
    }


    void DrawModernDecorations(float centerX, float centerY)
    {
        // Animated corner decorations
        float time = Time.time;
        GUI.color = new Color(1f, 0.3f, 0.3f, 0.6f);
        
        // Corner brackets
        float bracketSize = 40f + Mathf.Sin(time * 2f) * 5f;
        
        // Top-left bracket
        GUI.DrawTexture(new Rect(centerX - 300f, centerY - 200f, bracketSize, 4f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX - 300f, centerY - 200f, 4f, bracketSize), Texture2D.whiteTexture);
        
        // Top-right bracket
        GUI.DrawTexture(new Rect(centerX + 300f - bracketSize, centerY - 200f, bracketSize, 4f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX + 300f - 4f, centerY - 200f, 4f, bracketSize), Texture2D.whiteTexture);
        
        // Bottom-left bracket
        GUI.DrawTexture(new Rect(centerX - 300f, centerY + 200f - 4f, bracketSize, 4f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX - 300f, centerY + 200f - bracketSize, 4f, bracketSize), Texture2D.whiteTexture);
        
        // Bottom-right bracket
        GUI.DrawTexture(new Rect(centerX + 300f - bracketSize, centerY + 200f - 4f, bracketSize, 4f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX + 300f - 4f, centerY + 200f - bracketSize, 4f, bracketSize), Texture2D.whiteTexture);
        
        // Center cross
        GUI.color = new Color(1f, 1f, 1f, 0.1f);
        GUI.DrawTexture(new Rect(centerX - 1f, centerY - 100f, 2f, 200f), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX - 100f, centerY - 1f, 200f, 2f), Texture2D.whiteTexture);
        
        GUI.color = Color.white;
    }
}
