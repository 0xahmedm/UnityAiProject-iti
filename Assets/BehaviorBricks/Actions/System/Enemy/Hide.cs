using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using UnityEngine.AI;

namespace BBUnity.Actions
{
    [Action("Enemy/Hide")]
    public class Hide : GOAction
    {
        [InParam("nearSpotThreshold")]
        public float nearSpotThreshold = 3f;

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
                Debug.LogError("Could not find GameObject named 'HideSpots'!");
                return;
            }

            HideSpot[] spots = hideSpotsParent.GetComponentsInChildren<HideSpot>();

            if (spots.Length == 0)
            {
                Debug.LogError("No HideSpot scripts found!");
                return;
            }

            Transform nearestSpot = null;
            float shortestDist = float.MaxValue;

            foreach (HideSpot spot in spots)
            {
                float dist = Vector3.Distance(gameObject.transform.position, spot.transform.position);

                if (dist < shortestDist && !spot.occupied)
                {
                    shortestDist = dist;
                    nearestSpot = spot.transform;
                    chosenSpot = spot;
                }
            }

            if (nearestSpot != null && shortestDist <= nearSpotThreshold)
            {
                target = nearestSpot.position;
                chosenSpot.occupied = true;
            }
            else
            {
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