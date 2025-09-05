using UnityEngine;

public class health_bar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public float healthBarWidth = 2f;
    public float healthBarHeight = 0.2f;
    public float healthBarYOffset = 2f;
    public Color currentHealthColor = Color.red;
    public Color missingHealthColor = Color.white;
    
    private float maxHealth = 100f;
    private float currentHealth = 100f;
    private Camera playerCamera;
    
    void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            playerCamera = FindObjectOfType<Camera>();
        }
    }
    
    void Update()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                playerCamera = FindObjectOfType<Camera>();
            }
        }
    }
    
    void OnGUI()
    {
        if (playerCamera == null) return;
        
        Vector3 worldPosition = transform.position + Vector3.up * healthBarYOffset;
        Vector3 screenPosition = playerCamera.WorldToScreenPoint(worldPosition);
        
        if (screenPosition.z < 0) return;
        
        screenPosition.y = Screen.height - screenPosition.y;
        
        float healthPercentage = maxHealth > 0 ? currentHealth / maxHealth : 0f;
        healthPercentage = Mathf.Clamp01(healthPercentage);
        
        float barWidth = healthBarWidth * 50f;
        float barHeight = healthBarHeight * 50f;
        
        float currentHealthWidth = barWidth * healthPercentage;
        
        Rect backgroundRect = new Rect(screenPosition.x - barWidth / 2f, screenPosition.y - barHeight / 2f, barWidth, barHeight);
        GUI.color = missingHealthColor;
        GUI.DrawTexture(backgroundRect, Texture2D.whiteTexture);
        
        if (currentHealthWidth > 0)
        {
            Rect healthRect = new Rect(screenPosition.x - barWidth / 2f, screenPosition.y - barHeight / 2f, currentHealthWidth, barHeight);
            GUI.color = currentHealthColor;
            GUI.DrawTexture(healthRect, Texture2D.whiteTexture);
        }
        
        GUI.color = Color.white;
    }
    
    public void SetMaxHealth(float max)
    {
        maxHealth = max;
        currentHealth = maxHealth;
    }
    
    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0f, maxHealth);
    }
    
    public float GetHealth()
    {
        return currentHealth;
    }
    
    public float GetMaxHealth()
    {
        return maxHealth;
    }
}
