using UnityEngine;
using UnityEngine.AI;

public class RangedTacticalAI : MonoBehaviour
{
    public enum EnemyState { MoveToSpot, Aiming, Shooting, Repositioning, Die }
    public EnemyState currentState;

    [Header("References")]
    public Transform player; // Assign the Player Transform here in the Inspector
    public Transform[] shootingSpots;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Stats")]
    public float health = 100f;
    public float aimDuration = 2f;
    public float repositionDelay = 3f;

    private NavMeshAgent agent;
    private float stateTimer;
    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if (player == null)
        {
            Debug.LogError("Player reference is missing on " + gameObject.name);
        }

        SwitchState(EnemyState.MoveToSpot);
        GoToNextSpot();
    }

    void Update()
    {
        if (isDead) return;

        // 1. Update Animations
        if (anim != null && agent != null)
        {
            float velocity = agent.velocity.magnitude;
            anim.SetFloat("Speed", velocity);
        }

        // 2. Brain Logic
        switch (currentState)
        {
            case EnemyState.MoveToSpot:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    SwitchState(EnemyState.Aiming);
                }
                break;

            case EnemyState.Aiming:
                LookAtPlayer();
                stateTimer += Time.deltaTime;
                if (stateTimer >= aimDuration) SwitchState(EnemyState.Shooting);
                break;

            case EnemyState.Shooting:
                FireWeapon();
                SwitchState(EnemyState.Repositioning);
                break;

            case EnemyState.Repositioning:
                stateTimer += Time.deltaTime;
                if (stateTimer >= repositionDelay)
                {
                    GoToNextSpot();
                    SwitchState(EnemyState.MoveToSpot);
                }
                break;

            case EnemyState.Die:
                HandleDeath();
                break;
        }
    }

    // This is the function you call when the player hits him
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        Debug.Log(gameObject.name + " took damage! Current Health: " + health);

        if (health <= 0)
        {
            SwitchState(EnemyState.Die);
        }
    }

    // Detecting physical bullets (Colliders)
    private void OnTriggerEnter(Collider other)
    {
        // Make sure your bullet prefab has the tag "Bullet"
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(25f); // Damage value
            Destroy(other.gameObject); // Destroy the bullet
        }
    }

    void SwitchState(EnemyState newState)
    {
        currentState = newState;
        stateTimer = 0;
    }

    void GoToNextSpot()
    {
        if (shootingSpots.Length == 0) return;
        int randomIndex = Random.Range(0, shootingSpots.Length);
        agent.SetDestination(shootingSpots[randomIndex].position);
    }

    void LookAtPlayer()
    {
        if (player == null) return;
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void FireWeapon()
    {
        if (projectilePrefab && firePoint)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            if (anim != null) anim.SetTrigger("Shoot");
        }
    }

    void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        agent.isStopped = true;
        agent.enabled = false; // Disable NavMesh to prevent weird sliding
        
        if (anim != null) anim.SetTrigger("Die");
        
        Debug.Log("Enemy has died.");
        
        // Optional: Destroy the enemy object after 5 seconds to clean up the scene
        Destroy(gameObject, 5f);
    }
}