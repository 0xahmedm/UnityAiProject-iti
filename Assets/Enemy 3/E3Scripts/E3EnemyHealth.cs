using UnityEngine;

public class E3EnemyHealth : MonoBehaviour
{
    public int health = 100;
    public bool isDead = false;

    public Animator anim;

    [HideInInspector]
    public E3EnemySpawner spawner;

    [HideInInspector]
    public bool isClone = false; // spawner will mark clones

    void Start()
    {
        if(anim == null)
            anim = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(50);
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        health -= dmg;
        anim.SetTrigger("Hit");

        if (health <= 0)
        {
            isDead = true;
            anim.SetTrigger("isDead");

            if (spawner != null)
                spawner.RegisterKill();

            if (isClone)
                Destroy(gameObject, 3f); // only destroy clones, not the original
        }
    }
}