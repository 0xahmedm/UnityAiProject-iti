using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] Rigidbody rb;
    private void Start()
    {
        rb.linearVelocity = transform.forward * speed;
    }
    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}