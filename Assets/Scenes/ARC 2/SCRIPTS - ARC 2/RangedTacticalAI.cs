using UnityEngine;
using UnityEngine.AI;

public class RangedTacticalAI : MonoBehaviour
{
    public enum EnemyState { MoveToSpot, Aiming, Shooting, Repositioning, Die }
    public EnemyState currentState;

    [Header("References")]
    public Transform player;
    public Transform[] shootingSpots;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Stats")]
    public float health = 100f;
    public float aimDuration = 2f;
    public float repositionDelay = 3f;

    [Header("Spawn Settings")]
    public Transform[] SpawnPoints;
    public GameObject enemyPrefab;
    public int minSpawn = 2;
    public int maxSpawn = 5;

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

        if (anim != null && agent != null)
        {
            float velocity = agent.velocity.magnitude;
            anim.SetFloat("Speed", velocity);
        }

        switch (currentState)
        {
            case EnemyState.MoveToSpot:

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    SwitchState(EnemyState.Aiming);

                break;

            case EnemyState.Aiming:

                LookAtPlayer();
                stateTimer += Time.deltaTime;

                if (stateTimer >= aimDuration)
                    SwitchState(EnemyState.Shooting);

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

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;

        if (health <= 0)
        {
            SwitchState(EnemyState.Die);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(25f);
            Destroy(other.gameObject);
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

        Quaternion lookRotation =
            Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        transform.rotation =
            Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void FireWeapon()
    {
        if (projectilePrefab && firePoint)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            if (anim != null)
                anim.SetTrigger("Shoot");
        }
    }

    void HandleDeath()
    {
        if (isDead) return;

        isDead = true;

        agent.isStopped = true;
        agent.enabled = false;

        if (anim != null)
            anim.SetTrigger("Die");

        SpawnEnemies();

        Destroy(gameObject, 5f);
    }

    void SpawnEnemies()
    {
        if (SpawnPoints.Length == 0 || enemyPrefab == null) return;

        int spawnCount = Random.Range(minSpawn, maxSpawn + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            int randomPoint = Random.Range(0, SpawnPoints.Length);

            Instantiate(
                enemyPrefab,
                SpawnPoints[randomPoint].position,
                SpawnPoints[randomPoint].rotation
            );
        }
    }
}