using UnityEngine;
using UnityEngine.AI;

public class TestMove : MonoBehaviour {
    public Transform target;
    private NavMeshAgent agent; // Store the agent here

    void Start() {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update() {
        if (target != null) {
            // This line now runs every single frame
            agent.SetDestination(target.position);
        }
    }
}