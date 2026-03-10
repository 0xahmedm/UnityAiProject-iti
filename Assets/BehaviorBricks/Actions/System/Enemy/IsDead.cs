using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Conditions
{
    [Condition("Enemy/IsDead")]
    public class IsDead : GOCondition
    {
        private E3EnemyHealth health;

        public override bool Check()
        {
            if (health == null)
                health = gameObject.GetComponent<E3EnemyHealth>();

            return health != null && health.isDead;
        }
    }
}