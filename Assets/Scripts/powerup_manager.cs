using UnityEngine;

public class powerup_manager : MonoBehaviour
{
    [Header("Power-up Settings")]
    [Tooltip("Current power-up level (affects model appearance)")]
    public int powerLevel = 0;
    [Tooltip("Maximum power level for model swapping")]
    public int maxPowerLevel = 5;
    [Tooltip("Minimum power level (negative for downgrades)")]
    public int minPowerLevel = -5;

    [Header("Model Swapping")]
    [Tooltip("Array of player models for different power levels (index 0 = most downgraded, last = most powered up)")]
    public GameObject[] playerModels;
    [Tooltip("Current active model index")]
    public int currentModelIndex = 0;

    [Header("Power-up Effects")]
    [Tooltip("Damage multiplier for power-ups")]
    public float damageMultiplier = 1f;
    [Tooltip("Attack speed multiplier for power-ups")]
    public float attackSpeedMultiplier = 1f;

    private player_movement playerMovement;
    private player_combat playerCombat;
    private Vector3 originalScale;

    void Start()
    {
        playerMovement = GetComponent<player_movement>();
        playerCombat = GetComponent<player_combat>();
        originalScale = transform.localScale;
        
        // Initialize with base model
        UpdateModel();
    }

    public void ApplyPowerUp(PowerUpType powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUpType.DamageBoost:
                powerLevel += 1;
                damageMultiplier += 0.3f;
                break;
            case PowerUpType.AttackSpeedBoost:
                powerLevel += 1;
                attackSpeedMultiplier += 0.2f;
                break;
            case PowerUpType.DamageDecrease:
                powerLevel -= 1;
                damageMultiplier -= 0.3f;
                break;
            case PowerUpType.AttackSpeedDecrease:
                powerLevel -= 1;
                attackSpeedMultiplier -= 0.2f;
                break;
        }

        // Clamp power level
        powerLevel = Mathf.Clamp(powerLevel, minPowerLevel, maxPowerLevel);
        
        // Apply effects
        ApplyEffects();
        UpdateModel();
        
        Debug.Log($"Power-up applied: {powerUpType}. Power Level: {powerLevel}");
    }

    void ApplyEffects()
    {
        // Apply damage multiplier to combat system
        if (playerCombat != null)
        {
            playerCombat.ApplyDamagePowerUp(damageMultiplier - 1f); // Apply the increase
        }

        // Apply attack speed multiplier to combat system
        if (playerCombat != null)
        {
            playerCombat.ApplySpeedPowerUp(attackSpeedMultiplier - 1f); // Apply the increase
        }
    }

    void UpdateModel()
    {
        if (playerModels == null || playerModels.Length == 0) return;

        // Calculate model index based on power level
        // Map power level to model index (0 to maxPowerLevel)
        float normalizedPower = (float)(powerLevel - minPowerLevel) / (maxPowerLevel - minPowerLevel);
        int modelIndex = Mathf.RoundToInt(normalizedPower * (playerModels.Length - 1));
        modelIndex = Mathf.Clamp(modelIndex, 0, playerModels.Length - 1);

        // Deactivate current model
        if (currentModelIndex < playerModels.Length && playerModels[currentModelIndex] != null)
        {
            playerModels[currentModelIndex].SetActive(false);
        }

        // Activate new model
        currentModelIndex = modelIndex;
        if (playerModels[currentModelIndex] != null)
        {
            playerModels[currentModelIndex].SetActive(true);
        }
    }

    public int GetPowerLevel()
    {
        return powerLevel;
    }

    public void ResetPowerUps()
    {
        powerLevel = 0;
        damageMultiplier = 1f;
        attackSpeedMultiplier = 1f;
        ApplyEffects();
        UpdateModel();
    }
}

public enum PowerUpType
{
    DamageBoost,
    AttackSpeedBoost,
    DamageDecrease,
    AttackSpeedDecrease
}
