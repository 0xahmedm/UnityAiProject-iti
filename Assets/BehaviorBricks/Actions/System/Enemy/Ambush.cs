using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using UnityEngine.AI;

namespace BBUnity.Actions
{
    [Action("Enemy/Ambush")]
    public class Ambush : GOAction
    {
        [InParam("player")]
        public GameObject player;

        private NavMeshAgent agent;
        private Animator anim;

        public override void OnStart()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();
            anim = gameObject.GetComponent<Animator>();

            agent.speed = 6f;
            anim.SetBool("isRunning", true);
        }

        public override TaskStatus OnUpdate()
        {
            agent.SetDestination(player.transform.position);

            float dist = Vector3.Distance(gameObject.transform.position, player.transform.position);

            if (dist < 2f)
            {
                anim.SetBool("isRunning", false);
                anim.SetBool("isAttacking", true);
                return TaskStatus.COMPLETED;
            }

            return TaskStatus.RUNNING;
        }
    }
}