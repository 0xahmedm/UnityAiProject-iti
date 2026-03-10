using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    // CRITICAL: This must be a PUBLIC FIELD (not a property) 
    // so the DeathWaveSpawner's Reflection can find it.
    public bool isDead = false; 

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Call this method from your Player's bullet script
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log(gameObject.name + " took damage! Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " has officially died.");

        // Optional: Disable the collider so bullets pass through the corpse
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
    }
}