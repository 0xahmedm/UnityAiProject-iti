using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifeSpan = 5f; // Kill itself if it misses everything

    void Start()
    {
        // Destroy the bullet after 5 seconds so it doesn't lag the game
        Destroy(gameObject, lifeSpan);
    }

    void Update()
    {
        // Move forward every frame
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // This runs when the bullet hits something
    private void OnTriggerEnter(Collider other)
    {
        // Don't hit the enemy who shot it! 
        // We will check if the thing we hit is NOT the enemy.
        if (other.gameObject.tag != "Enemy") 
        {
            Debug.Log("Bullet hit: " + other.name);
            // Add damage logic here later
            Destroy(gameObject); // The bullet disappears
        }
    }
}