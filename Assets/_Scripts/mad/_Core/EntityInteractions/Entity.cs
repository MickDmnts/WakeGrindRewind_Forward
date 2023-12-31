using UnityEngine;

using WGRF.Core;
using WGRF.Interactions;

namespace WGRF.Entities
{
    /// <summary>
    /// Abstract base class to be inherited from every entity of the game.
    /// </summary>
    public abstract class Entity : CoreBehaviour, IInteractable
    {
        #region INPECTOR_VALUES
        ///<summary>The entity name</summary>
        [Header("Set base entity data")]
        [SerializeField, Tooltip("The entity name")] protected string entityName;
        ///<summary>The entity max life</summary>
        [SerializeField, Tooltip("The entity max life")] protected int maxLife;
        ///<summary>The entity base life</summary>
        [SerializeField, Tooltip("The entity base life")] protected int entityLife;
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