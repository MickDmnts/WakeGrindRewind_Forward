using UnityEngine;
using WGRF.Core.Managers;
using WGRF.Interactions;

namespace WGRF.Entities.Player
{
    public class PlayerKick : MonoBehaviour
    {
        [Header("Set in inspector")]
        [SerializeField] float maxDistance;

        //External use
        public bool IsKicking { get; set; }

        #region PRIVATE_VARIABLES
        IKickable objToKick;
        bool isKickEnabled = false;
        float kickCooldown;
        #endregion

        void Start()
        {
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.onPlayerStateChange += SetIsKickEnabled;
                kickCooldown = 0;
            }
        }

        /// <summary>
        /// Subscribed to the PlayerEntity.onPlayerStateChange event.
        /// <para>Sets the isKickEnabled to the passed value.</para>
        /// </summary>
        void SetIsKickEnabled(bool value)
        {
            isKickEnabled = value;
        }

        void Update()
        {
            //Early exit if the kick functionality is not enabled
            if (!isKickEnabled) return;

            if (CanKick(kickCooldown))
            { IsKicking = false; }

            if (Input.GetKeyDown(KeyCode.Space) && !IsKicking)
            {
                InitiateKickSequence();
            }

            if (kickCooldown > 0) //Kick cooldown
            {
                kickCooldown -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Call to check if the player can kick again based on time passed.
        /// <para>If the time passed is greater than 0 then returns false, true otherwise.</para>
        /// </summary>
        bool CanKick(float timer)
        {
            if (timer > 0)
            {
                return false;
            }

            return true;
        }

        #region KICK_SEQUENCE
        /// <summary>
        /// Call to play the kicking animation and cast a ray forward.
        /// <para>Call IsKickable(...) with the passed hit info.</para>
        /// <para>Call PushKickable is the IsKickable(...) returns a reference.</para>
        /// <para>Sets kick cooldown to 0.5f at the end</para>
        /// </summary>
        void InitiateKickSequence()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            IsKicking = true;
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.PlayerAnimations.PlayKickAnimation();
            }

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                objToKick = IsKickable(hit);
                if (objToKick != null) //check if the hit is kickable
                {
                    PushKickable(objToKick);
                    objToKick = null;
                }
            }

            kickCooldown = 0.5f;
        }

        /// <summary>
        /// <para>If the passed ray hit info hits an Interactable object call its StunInteraction and then</para>
        /// <para>If the passed ray hit info hits a kickable object cache it and return it.</para>
        /// </summary>
        /// <returns>Null if none of the above apply.</returns>
        private IKickable IsKickable(RaycastHit hit)
        {
            IInteractable interaction = hit.transform.GetComponent<IInteractable>();
            if (interaction != null)
            {
                interaction.StunInteraction();
            }

            IKickable kickInteraction = hit.transform.GetComponent<IKickable>();
            if (kickInteraction != null)
            {
                return kickInteraction;
            }


            return null;
        }

        /// <summary>
        /// Call to invoke the SimulateKnockback from the passed IKickable reference.
        /// </summary>
        void PushKickable(IKickable kickable)
        {
            kickable.SimulateKnockback(transform.position);
        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        }
#endif

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.onPlayerStateChange -= SetIsKickEnabled;
            }
        }
    }
}