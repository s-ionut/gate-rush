using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class game_over_ui : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("Game over panel")]
    public GameObject gameOverPanel;
    [Tooltip("You failed text")]
    public Text gameOverText;
    [Tooltip("Retry button")]
    public Button retryButton;
    [Tooltip("Main menu button")]
    public Button mainMenuButton;

    [Header("Game Manager")]
    [Tooltip("Game manager reference")]
    public game_manager gameManager;

    void Start()
    {
        // Set up button listeners
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        // Hide game over panel initially
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverText != null)
        {
            gameOverText.text = "You Failed!";
        }
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    void RetryGame()
    {
        if (gameManager != null)
        {
            gameManager.RetryGame();
        }
        else
        {
            // Fallback: reload the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void GoToMainMenu()
    {
        // Load main menu scene (you'll need to create this)
        // SceneManager.LoadScene("MainMenu");
        Debug.Log("Main menu not implemented yet");
    }
}
