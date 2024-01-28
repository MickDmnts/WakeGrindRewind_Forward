using UnityEngine;
using WGRF.Core;
using WGRF.Entities;
using WGRF.Interactions;

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
        {
            enemyRB = GetComponent<Rigidbody>();
            //stunTimerCache = stunTimer;
        }

        /// <summary>
        /// Call to push the enemy gameobject to the opposite of the passed vector.
        /// </summary>
        public override void SimulateKnockback(Vector3 incomingDir)
        {
            enemyRB.isKinematic = false;

            oppositeDir = (transform.position - incomingDir).normalized;

            enemyRB.velocity = oppositeDir * kickForce;

            kicked = true;
            //canStun = true;

            //ManagerHub.S.GameSoundsHandler.PlayOneShot(GameAudioClip.PunchSound);
        }

        protected void Update()
        {
            /* if (!kicked | !stunOnCollision) return;

            //Deactivates the door stun ability after a short interval.
            if (canStun)
            {
                stunTimer -= Time.deltaTime;
                if (stunTimer <= 0f)
                {
                    canStun = false;
                    kicked = false;
                    stunTimer = stunTimerCache;
                }
            } */
        }

        private void OnCollisionEnter(Collision collision)
        {
            //Call the enemy stun interaction of collided enemies if the enemy got recently kicked.
            /* if (canStun)
            {
                IInteractable interaction = collision.gameObject.GetComponent<AIEntity>();

                if (interaction != null)
                {
                    interaction.StunInteraction();
                }
            } */
        }
    }
}