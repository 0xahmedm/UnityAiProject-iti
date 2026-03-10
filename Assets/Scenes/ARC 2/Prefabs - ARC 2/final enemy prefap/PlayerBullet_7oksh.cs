using UnityEngine;

public class PlayerBullet_7oksh : MonoBehaviour
{
    public float damage = 25f; // 4 shots to kill 100 health
    public float speed = 40f;

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the hit object has the Enemy tag
        if (other.CompareTag("Enemy"))
        {
            RangedTacticalAI enemy = other.GetComponent<RangedTacticalAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject); // Bullet disappears on hit
        }
    }
}