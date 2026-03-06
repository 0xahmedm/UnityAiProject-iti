using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    [Action("Enemy/Wait")]
    public class Wait : GOAction
    {
        [InParam("waitTime")]
        public float waitTime;

        private float timer;

        public override void OnStart()
        {
            timer = 0f;
        }

        public override TaskStatus OnUpdate()
        {
            timer += Time.deltaTime;

            if (timer >= waitTime)
                return TaskStatus.COMPLETED;

            return TaskStatus.RUNNING;
        }
    }
}