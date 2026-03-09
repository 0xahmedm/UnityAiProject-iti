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
        private bool isAttacking = false;

        public override void OnStart()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();
            anim = gameObject.GetComponent<Animator>();

            agent.speed = 12f;
            anim.SetBool("isRunning", true);
            isAttacking = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (!isAttacking)
            {
                agent.SetDestination(player.transform.position);

                float dist = Vector3.Distance(gameObject.transform.position, player.transform.position);

                if (dist < 3f)
                {
                    agent.isStopped = true;
                    anim.SetBool("isRunning", false);
                    anim.SetBool("isAttacking", true);
                    isAttacking = true;
                }

                return TaskStatus.RUNNING;
            }
            else
            {
                // Wait for attack animation to finish
                AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

                bool isPlayingAttack = stateInfo.IsTag("Attack");
                bool animFinished = isPlayingAttack && stateInfo.normalizedTime >= 1f;

                if (animFinished)
                {
                    anim.SetBool("isAttacking", false);
                    agent.isStopped = false;
                    return TaskStatus.COMPLETED;
                }

                return TaskStatus.RUNNING;
            }
        }
    }
}