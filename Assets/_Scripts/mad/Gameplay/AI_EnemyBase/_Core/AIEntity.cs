using System;
using UnityEngine;
using UnityEngine.AI;
using WGRF.AI.Entities.Hostile;
using WGRF.AI.FOV;
using WGRF.AI.Nodes;
using WGRF.Entities;

namespace WGRF.AI.Entities
{
    /// <summary>
    /// All the available gameObject layers the enemy AI can exist on.
    /// </summary>
    public enum EnemyLayer
    {
        Enemy = 7,
        NonInteractive = 31,
    }

    /* [CLASS DOCUMENTATION]
     * 
     * * This abstract class acts as a base for every AI Entity present in the game.
     *      Abstraction was used so every AI can have its own unique, but also universal behaviour.
     * 
     * Inspector variables: These variables must be set from the inspector
     * Protected variables: These variables are shared in derived classes and changed throughout the game.
     * 
     * [Must know]
     * 1. Components are cached separately in each entity.
     * 2. Local events are used primarily from each Entity's FOV manager so info about the world can be transfered through the BehaviourTree.
     * 3. Each AI entity creates its own BT  handler instance.
     * 
     */
    public abstract class AIEntity : Entity
    {
        [Header("Occlusion check layers")]
        public LayerMask occlusionLayers;

        [Header("After death transition")]
        [SerializeField] protected EnemyLayer layerOnDeath;

        #region PROTECTED_VARIABLES
        protected bool isAgentActive;

        protected Transform attackTarget;
        protected Rigidbody enemyRB;

        protected NavMeshAgent agent;
        #endregion

        #region BEHAVIOUR_CACHING
        protected AIEntityFOVManager fovManager;
        public AIEntityFOVManager FOVManager
        {
            get { return fovManager; }
            protected set { fovManager = value; }
        }

        protected AIEntityWeapon aiEntityWeapon;
        public AIEntityWeapon AIEntityWeapon
        {
            get { return aiEntityWeapon; }
            protected set { aiEntityWeapon = value; }
        }
        #endregion

        #region AI_ENTITY_SPECIFIC_EVENTS
        public event Action onPlayerFound;
        public void OnPlayerFound()
        {
            if (onPlayerFound != null)
            {
                onPlayerFound();
            }
        }

        public event Action onObserverDeath;
        public void OnObserverDeath()
        {
            if (onObserverDeath != null)
            {
                onObserverDeath();
            }
        }
        #endregion

        #region PROTECTED_METHODS
        //Called in Awake to cache the necessary components
        /// <summary>
        /// Call to cache the necessary entity components.
        /// </summary>
        protected abstract void CacheComponents();

        //Called in Start to create the entity BT
        protected abstract void CreateNodeData();
        protected abstract void SetupNodeDataFields();
        protected abstract void CreateBTHandler();

        protected abstract void TargetFound();

        protected abstract void SetHealth(float value);
        protected abstract bool CheckIfDead();
        #endregion

        #region PUBLIC_METHODS
        public abstract void SetAttackTarget(Transform target);

        public abstract void SetIsAgentActive(bool value);
        public abstract bool GetIsAgentActive();

        public abstract bool GetIsDead();

        public abstract INodeData GetEntityNodeData();

        public abstract NavMeshAgent GetAgent();
        #endregion
    }
}