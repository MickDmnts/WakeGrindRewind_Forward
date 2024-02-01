using UnityEngine;
using WGRF.Core;

namespace WGRF.AI
{
    public class EnemyKickable : KickableEntity
    {
        Rigidbody enemyRB;

        private void Awake()
        {
            EntrySetup();
        }

        void EntrySetup()
        { enemyRB = GetComponent<Rigidbody>(); }

        /// <summary>
        /// Call to push the enemy gameobject to the opposite of the passed vector.
        /// </summary>
        public override void SimulateKnockback(Vector3 incomingDir)
        {
            enemyRB.isKinematic = false;

            oppositeDir = (transform.position - incomingDir).normalized;

            enemyRB.velocity = oppositeDir * kickForce;

            //ManagerHub.S.GameSoundsHandler.PlayOneShot(GameAudioClip.PunchSound);
        }
    }
}