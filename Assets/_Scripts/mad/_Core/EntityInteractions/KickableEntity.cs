using UnityEngine;
using WGRF.Interactions;

namespace WGRF.Core
{   
    /// <summary>
    /// Attach this script to any game object you want to be kickable from the player
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class KickableEntity : MonoBehaviour, IKickable
    {
        [Header("Set in inspector")]
        ///<summary>The force which the object will get displaced with</summary>
        [SerializeField,Tooltip("The force which the object will get displaced with")] 
        protected float kickForce;

        #region PRIVATE_VARIABLES
        ///<summary>The attached rigidbody of the gameObject</summary>
        protected Rigidbody entityRB;
        ///<summary>Is the object kicked now?</summary>
        protected bool kicked = false;
        ///<summary>Caches the direction the object got kicked from</summary>
        protected Vector3 oppositeDir;
        #endregion

        private void Start()
        {entityRB = GetComponent<Rigidbody>();}

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
        }
    }
}