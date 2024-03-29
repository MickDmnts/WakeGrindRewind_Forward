using UnityEngine;

using WGRF.Interactions;

namespace WGRF.Core
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
        ///<summary>The entity current life</summary>
        [SerializeField, Tooltip("The entity current life")] protected int entityLife;
        #endregion

        ///<summary>The entity current life</summary>
        public int EntityLife => entityLife;
        ///<summary>The player max health</summary>
        public int MaxHealth => maxLife;

        /// <summary>
        /// Call to initiate the attack interaction behaviour of any Entity GameObject
        /// </summary>
        public abstract void AttackInteraction(int damage);

        /* /// <summary>
        /// Call to initiate the stun interaction of any Entity GameObject
        /// </summary>
        public abstract void StunInteraction(); */
    }
}