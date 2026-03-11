using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement; // Critical for scene loading

public class RangedTacticalAI : MonoBehaviour
{
    public enum EnemyState { MoveToSpot, Aiming, Shooting, Repositioning, Die }
    public EnemyState currentState;

    [Header("Basic References")]
    public Transform player;
    public Transform[] shootingSpots;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Enemy Stats")]
    public float health = 100f;
    public float aimDuration = 2f;
    public float repositionDelay = 3f;

    [Header("Sequential Spawn Settings")]
    public GameObject enemyPrefab;     // Drag this Enemy Prefab from Assets here
    public int enemiesLeftToSpawn = 3;  // Total extra enemies to spawn one-by-one
    public string nextSceneName;       // Exact name of the scene to load at the end

    private NavMeshAgent agent;
    private Animator anim;
    private float stateTimer;
    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        // Auto-find player by tag if reference is missing
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        SwitchState(EnemyState.MoveToSpot);
        GoToNextSpot();
    }

    void Update()
    {
        if (isDead) return;

        // Drive the animator speed for Walk/Run
        if (anim != null && agent != null)
            anim.SetFloat("Speed", agent.velocity.magnitude);

        // Check for Death or Cheat Key
        if (health <= 0 || Input.GetKeyDown(KeyCode.K))
            SwitchState(EnemyState.Die);

        // State Machine logic
        switch (currentState)
        {
            case EnemyState.MoveToSpot:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                    SwitchState(EnemyState.Aiming);
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

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        health -= amount;
        Debug.Log(gameObject.name + " Health: " + health);
    }

    void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null) agent.isStopped = true;
        if (anim != null) anim.SetTrigger("Die");

        // Logic for next spawn or scene load
        if (enemiesLeftToSpawn > 0)
        {
            SpawnNextEnemy();
        }
        else
        {
            Debug.Log("Chain complete. Loading scene: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }

        Destroy(gameObject, 4f); // Remove corpse after animation
    }

    void SpawnNextEnemy()
    {
        // Pick a random spot from your shooting spots to spawn the next one
        Transform spawnPoint = shootingSpots[Random.Range(0, shootingSpots.Length)];
        GameObject nextObj = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        
        RangedTacticalAI nextAI = nextObj.GetComponent<RangedTacticalAI>();
        if (nextAI != null)
        {
            // Pass the reduced count to the next enemy
            nextAI.enemiesLeftToSpawn = this.enemiesLeftToSpawn - 1;
            
            // Pass essential references
            nextAI.nextSceneName = this.nextSceneName;
            nextAI.player = this.player;
            nextAI.shootingSpots = this.shootingSpots;
            nextAI.enemyPrefab = this.enemyPrefab;
        }
    }

    // --- Helper Functions ---
    void SwitchState(EnemyState newState) { currentState = newState; stateTimer = 0; }
    
    void GoToNextSpot()
    {
        if (shootingSpots.Length > 0 && agent != null)
            agent.SetDestination(shootingSpots[Random.Range(0, shootingSpots.Length)].position);
    }

    void LookAtPlayer()
    {
        if (player == null) return;
        Vector3 dir = (player.position - transform.position).normalized;
        Quaternion look = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 5f);
    }

    void FireWeapon()
    {
        if (projectilePrefab && firePoint)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            if (anim != null) anim.SetTrigger("Shoot");
        }
    }
}