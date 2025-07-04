using UnityEngine;

[RequireComponent(typeof(Collider))]
public class gate : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("COLLIDED :)");
            Destroy(gameObject);
        }
    }
}