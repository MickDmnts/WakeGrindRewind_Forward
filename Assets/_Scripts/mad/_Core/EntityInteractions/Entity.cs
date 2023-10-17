using UnityEngine;
using WGRF.Interactions;

namespace WGRF.Entities
{
    public abstract class Entity : MonoBehaviour, IInteractable
    {
        #region INPECTOR_VALUES
        ///<summary>The entity name</summary>
        [Header("Set base entity data")]
        [SerializeField, Tooltip("The entity name")] protected string entityName;
        ///<summary>The entity base life</summary>
        [SerializeField, Tooltip("The entity base life")] protected float entityLife;
        #endregion

        /// <summary>
        /// Call to initiate the attack interaction behaviour of any Entity GameObject
        /// </summary>
        public abstract void AttackInteraction();

        /// <summary>
        /// Call to initiate the stun interaction of any Entity GameObject
        /// </summary>
        public abstract void StunInteraction();
    }
}