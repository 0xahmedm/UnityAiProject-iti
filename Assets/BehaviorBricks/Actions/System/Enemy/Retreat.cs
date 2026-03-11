using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using UnityEngine.AI;

namespace BBUnity.Actions
{
    [Action("Enemy/Retreat")]
    public class Retreat : GOAction
    {
        private NavMeshAgent agent;
        private Animator anim;
        private Vector3 target;

        private HideSpot chosenSpot;

        public override void OnStart()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();
            anim = gameObject.GetComponent<Animator>();

            GameObject hideSpotsParent = GameObject.Find("HideSpots");

            if (hideSpotsParent == null)
            {
                Debug.LogError("Could not find HideSpots object!");
                return;
            }

            HideSpot[] spots = hideSpotsParent.GetComponentsInChildren<HideSpot>();

            foreach (HideSpot spot in spots)
            {
                if (!spot.occupied)
                {
                    chosenSpot = spot;
                    chosenSpot.occupied = true;
                    target = spot.transform.position;
                    break;
                }
            }

            if (chosenSpot == null)
            {
                int rand = Random.Range(0, spots.Length);
                target = spots[rand].transform.position;
            }

            agent.speed = 10f;
            agent.SetDestination(target);

            anim.SetBool("isRunning", true);
            anim.SetBool("isAttacking", false);
        }

        public override TaskStatus OnUpdate()
        {
            if (agent == null)
                return TaskStatus.FAILED;

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                anim.SetBool("isRunning", false);
                return TaskStatus.COMPLETED;
            }

            return TaskStatus.RUNNING;
        }
    }
}