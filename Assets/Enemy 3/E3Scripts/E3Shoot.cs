using UnityEngine;

public class E3Shoot : MonoBehaviour
{
    public int damage = 25;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100f))
            {
                E3EnemyHealth enemy = hit.collider.GetComponent<E3EnemyHealth>();

                if(enemy != null)
                    enemy.TakeDamage(damage);
            }
        }
    }
}