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

    private NavMeshAgent agent;
    private float stateTimer;
    private Animator anim; // FIX: Added the missing definition

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
        // FIX: Find the animator on the child object (the Mixamo model)
        anim = GetComponentInChildren<Animator>();

        SwitchState(EnemyState.MoveToSpot);
        GoToNextSpot();
    }

    void Update()
    {
        // 1. UPDATE ANIMATION SPEED ALWAYS
        if (anim != null && agent != null)
        {
            float velocity = agent.velocity.magnitude;
            anim.SetFloat("Speed", velocity);
        }

        // 2. CHECK FOR DEATH
        if (Input.GetKeyDown(KeyCode.K)) health = 0;

        if (health <= 0 && currentState != EnemyState.Die)
        {
            SwitchState(EnemyState.Die);
        }

        // 3. BRAIN LOGIC
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
            if (anim != null) anim.SetTrigger("Shoot"); // Trigger shooting animation
        }
    }

    void HandleDeath()
    {
        agent.isStopped = true;
        if (anim != null) anim.SetTrigger("Die"); // Trigger death animation
        this.enabled = false;
    }
}