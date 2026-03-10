using UnityEngine;

public class E3EnemyHealth : MonoBehaviour
{
public int health = 100;
    public bool isDead = false;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Bullet")) {
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
        }
    }
}
