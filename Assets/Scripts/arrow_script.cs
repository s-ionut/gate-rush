using UnityEngine;

public class arrow_script : MonoBehaviour
{
    [Header("Arrow Settings")]
    [Tooltip("Damage this arrow deals")]
    public float damage = 10f;
    [Tooltip("Speed of the arrow")]
    public float speed = 15f;
    [Tooltip("Lifetime before arrow disappears")]
    public float lifetime = 5f;

    private GameObject target;
    private Vector3 direction;
    private float startTime;

    void Start()
    {
        startTime = Time.time;
        
        // Ensure the arrow has a trigger collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("Arrow prefab needs a Collider component!");
        }
        else if (!col.isTrigger)
        {
            Debug.LogWarning("Arrow collider should be set as trigger!");
            col.isTrigger = true;
        }
    }

    void Update()
    {
        // Move arrow in the calculated direction
        if (direction != Vector3.zero)
        {
            // Use the calculated direction vector directly
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            // Fallback: move forward if no direction calculated
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        
        // Check if we're close to the target enemy
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToTarget < 1f) // If very close to target
            {
                Debug.Log($"Arrow very close to target: {distanceToTarget} units");
            }
            
            // Manual collision check as backup
            if (distanceToTarget < 0.5f) // If very close, manually trigger hit
            {
                Debug.Log("Arrow manually hitting enemy due to close distance");
                HitEnemy(target);
            }
        }
        
        // Destroy arrow after lifetime
        if (Time.time - startTime >= lifetime)
        {
            Debug.Log("Arrow destroyed due to lifetime");
            Destroy(gameObject);
        }
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        
        // Calculate direction to target
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position;
            Vector3 arrowPosition = transform.position;
            
            // Calculate direction vector
            direction = (targetPosition - arrowPosition).normalized;
            
            // Since arrow prefab points upward (Y-axis), we need to rotate it to point toward the enemy
            // Calculate the direction in the XZ plane (ignoring Y for horizontal movement)
            Vector3 horizontalDirection = new Vector3(direction.x, 0f, direction.z).normalized;
            
            // Use LookRotation to point the arrow toward the target
            // Then rotate -90 degrees around X axis to convert from upward to forward
            transform.rotation = Quaternion.LookRotation(horizontalDirection) * Quaternion.Euler(-90f, 0f, 0f);
            
            Debug.Log($"Arrow targeting enemy at {targetPosition}, direction: {direction}, horizontalDirection: {horizontalDirection}, rotation: {transform.rotation.eulerAngles}");
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Arrow OnTriggerEnter with: {other.name}, tag: {other.tag}");
        
        // Check if arrow hit an enemy
        if (other.CompareTag("Enemy"))
        {
            HitEnemy(other.gameObject);
        }
        else
        {
            Debug.Log($"Arrow hit something else: {other.name} with tag {other.tag}");
        }
    }
    
    void HitEnemy(GameObject enemy)
    {
        Debug.Log($"Arrow hit enemy: {enemy.name}");
        
        // Deal damage to enemy
        enemy_script enemyScript = enemy.GetComponent<enemy_script>();
        if (enemyScript != null)
        {
            enemyScript.TakeDamage(damage);
            Debug.Log($"Arrow dealt {damage} damage to enemy");
        }
        else
        {
            Debug.LogWarning($"Enemy {enemy.name} doesn't have enemy_script component!");
        }
        
        // Destroy arrow
        Debug.Log("Arrow destroyed after hitting enemy");
        Destroy(gameObject);
    }
}
