using UnityEngine;

public class auto_game_manager : MonoBehaviour
{
    void Start()
    {
        // Check if game manager already exists
        game_manager existingManager = FindObjectOfType<game_manager>();
        if (existingManager == null)
        {
            Debug.Log("No Game Manager found, creating one automatically...");
            
            // Create a new GameObject for the game manager
            GameObject gameManagerObject = new GameObject("Game Manager (Auto)");
            
            // Add the game_manager script
            gameManagerObject.AddComponent<game_manager>();
            
            Debug.Log("Game Manager created automatically!");
        }
        else
        {
            Debug.Log("Game Manager already exists in scene");
        }
    }
}
