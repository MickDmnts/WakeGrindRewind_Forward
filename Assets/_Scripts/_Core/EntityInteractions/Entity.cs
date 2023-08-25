using UnityEngine;
using WGR.Interactions;

namespace WGR.Entities
{
    /* CLASS DOCUMENTATION *\
     * [Variable Specifics]
     * Inspector values: Inspector values must be set from the editor inpsector for the script to work correctly
     * 
     * Implements: IInteractable interface
     * 
     * [Must Know]
     * 0. This is the base class for every entity of the game ie. Player and AIEntities
     * 1. AttackInteraction() + StunInteraction() are used throughout the game
     *      for player-enemy interactions. 
     *      e.g. When a bullet touches either the player or the enemy it calls the
     *          AttackInteraction() through the interface using polymorphism.
     */

    public abstract class Entity : MonoBehaviour, IInteractable
    {
        #region INPECTOR_VALUES
        [Header("Set base entity data")]
        [SerializeField] protected Stat<string> entityName;

        [SerializeField] protected Stat<float> entityLife;
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