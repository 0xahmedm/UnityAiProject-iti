using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using UnityEngine.AI;

namespace BBUnity.Actions
{
    [Action("Enemy/Die")]
    public class Die : GOAction
    {
        private NavMeshAgent agent;

        public override void OnStart()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();

            if (agent != null)
                agent.isStopped = true;
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.COMPLETED;
        }
    }
}