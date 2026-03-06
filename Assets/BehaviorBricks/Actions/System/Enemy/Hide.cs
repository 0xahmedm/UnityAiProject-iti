using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using UnityEngine.AI;

namespace BBUnity.Actions
{
    [Action("Enemy/Hide")]
    public class Hide : GOAction
    {
        private NavMeshAgent agent;
        private Animator anim;
        private Vector3 target;

        public override void OnStart()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();
            anim = gameObject.GetComponent<Animator>();

            GameObject hideSpotsParent = GameObject.Find("HideSpots");

            if (hideSpotsParent == null)
            {
                Debug.LogError("Could not find GameObject named 'HideSpots' in the scene!");
                return;
            }

            Transform[] allSpots = hideSpotsParent.GetComponentsInChildren<Transform>();
            Transform[] spots = System.Array.FindAll(allSpots, t => t.CompareTag("HideSpot"));

            if (spots.Length == 0)
            {
                Debug.LogError("No objects tagged 'HideSpot' found under HideSpots!");
                return;
            }

            int rand = Random.Range(0, spots.Length);
            target = spots[rand].position;

            agent.speed = 4f;
            agent.SetDestination(target);

            anim.SetBool("isRunning", true);
            anim.SetBool("isAttacking", false);
        }

        public override TaskStatus OnUpdate()
        {
            if (agent == null) return TaskStatus.FAILED;

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                anim.SetBool("isRunning", false);
                return TaskStatus.COMPLETED;
            }

            return TaskStatus.RUNNING;
        }
    }
}