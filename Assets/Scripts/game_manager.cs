using UnityEngine;
using UnityEngine.UI;

public class game_manager : MonoBehaviour
{
    [Header("Game State")]
    [Tooltip("Is the game currently paused?")]
    public bool isPaused = false;
    [Tooltip("Is the game over?")]
    public bool isGameOver = false;

    [Header("UI References")]
    [Tooltip("Game over screen UI")]
    public GameObject gameOverScreen;
    [Tooltip("Retry button")]
    public Button retryButton;
    [Tooltip("You failed text")]
    public Text gameOverText;

    [Header("Player References")]
    [Tooltip("Player GameObject")]
    public GameObject player;
    [Tooltip("Enemy spawner")]
    public enemy_spawner enemySpawner;

    private player_combat playerCombat;
    private player_movement playerMovement;
    private powerup_manager powerUpManager;

    void Start()
    {
        // Get player components
        if (player != null)
        {
            playerCombat = player.GetComponent<player_combat>();
            playerMovement = player.GetComponent<player_movement>();
            powerUpManager = player.GetComponent<powerup_manager>();
        }

        // Set up retry button
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryGame);
        }

        // Hide game over screen initially
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
    }

    void Update()
    {
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
        if (isGameOver) return;

        isGameOver = true;
        isPaused = true;

        // Show game over screen
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        // Pause all game systems
        Time.timeScale = 0f;

        Debug.Log("Game Over!");
    }

    public void RetryGame()
    {
        // Reset game state
        isGameOver = false;
        isPaused = false;
        Time.timeScale = 1f;

        // Hide game over screen
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }

        // Reset player
        if (player != null)
        {
            // Reset player position
            player.transform.position = Vector3.zero;
            
            // Reset player stats
            if (powerUpManager != null)
            {
                powerUpManager.ResetPowerUps();
            }
            
            if (playerCombat != null)
            {
                playerCombat.ResetCombatStats();
            }
        }

        // Reset enemy spawner
        if (enemySpawner != null)
        {
            enemySpawner.ResetSpawner();
        }

        // Destroy all existing enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
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
}
