using UnityEngine;
using WGR.Interactions;

namespace WGR.Entities
{
    /* CLASS DOCUMENTATION *\
     * [Variable Specifics]
     * Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
     * Dynamically changed: These variables are changed throughout the game.
     * 
     * [Class Flow]
     * 1. SimulateKnockback(...) gets called through the IKickable interface by polymorphism.
     * 
     * [Must Know]
     * 1. This script can be attached to ANY gameObject that we want to be kickable from the player.
     */
    public class KickableEntity : MonoBehaviour, IKickable
    {
        [Header("Set in inspector")]
        [SerializeField] protected float kickForce;
        [SerializeField] protected bool stunOnCollision;

        #region PRIVATE_VARIABLES
        protected Rigidbody entityRB;
        protected bool kicked = false;
        protected Vector3 oppositeDir;

        protected float stunTimer = 1f;
        protected float stunTimerCache;
        protected bool canStun = false;
        #endregion

        private void Start()
        {
            if (!(entityRB = GetComponent<Rigidbody>()))
                Utils.MissingComponent("Rigidbody", this);
        }

        /// <summary>
        /// Call to start a knockback simulation on the gameObject that THIS script is attached to.
        /// <para>Knockback force is set from the inpsector (kickForce)</para>
        /// </summary>
        /// <param name="incomingDir">The transform of the Kicker</param>
        public virtual void SimulateKnockback(Vector3 incomingDir)
        {
            oppositeDir = (transform.position - incomingDir).normalized;

            entityRB.velocity = oppositeDir * kickForce;

            kicked = true;
            canStun = true;
        }
    }
}