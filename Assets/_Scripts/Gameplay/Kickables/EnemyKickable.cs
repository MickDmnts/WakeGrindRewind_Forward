using UnityEngine;
using WGR.AI.Entities;
using WGR.Core.Managers;
using WGR.Interactions;

namespace WGR.Entities
{
    /* [CLASS DOCUMENTATION] 
     * 
     * [Variable Specifics]
     * Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
     * Dynamically changed: These variables are changed throughout the game.
     * 
     * [Implements] KickableEntity.cs
     */
    public class EnemyKickable : KickableEntity
    {
        //Private variable
        Rigidbody enemyRB;

        private void Awake()
        {
            EntrySetup();
        }

        void EntrySetup()
        {
            enemyRB = GetComponent<Rigidbody>();
            stunTimerCache = stunTimer;
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
            canStun = true;

            GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.PunchSound);
        }

        protected void Update()
        {
            if (!kicked | !stunOnCollision) return;

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
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            //Call the enemy stun interaction of collided enemies if the enemy got recently kicked.
            if (canStun)
            {
                IInteractable interaction = (IInteractable)collision.gameObject.GetComponent<AIEntity>();

                if (interaction != null)
                {
                    interaction.StunInteraction();
                }
            }
        }
    }
}