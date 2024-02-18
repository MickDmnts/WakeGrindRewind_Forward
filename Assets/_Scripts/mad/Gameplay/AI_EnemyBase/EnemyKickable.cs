using UnityEngine;
using WGRF.Core;

namespace WGRF.AI
{
    /// <summary>
    /// Makes the attached enemy a kickable entity
    /// </summary>
    public class EnemyKickable : KickableEntity
    {
        ///<summary>The agent rigidbody</summary>
        Rigidbody enemyRB;

        private void Awake()
        {
            enemyRB = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Call to push the enemy gameobject to the opposite direction of the passed vector.
        /// </summary>
        public override void SimulateKnockback(Vector3 incomingDir)
        {
            enemyRB.isKinematic = false;

            oppositeDir = (transform.position - incomingDir).normalized;

            enemyRB.velocity = oppositeDir * kickForce;

        }
    }
}