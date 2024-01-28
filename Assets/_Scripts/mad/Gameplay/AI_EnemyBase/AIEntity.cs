using System;
using UnityEngine;
using UnityEngine.AI;
using WGRF.Core;

namespace WGRF.AI
{
    /// <summary>
    /// All the available gameObject layers the enemy AI can exist on.
    /// </summary>
    public enum EnemyLayer
    {
        Enemy = 7,
        NonInteractive = 31,
    }

    public abstract class AIEntity : Entity
    {
        [Header("Occlusion check layers")]
        public LayerMask occlusionLayers;

        [Header("After death transition")]
        [SerializeField] protected EnemyLayer layerOnDeath;

        [Header("Decal on death")]
        [SerializeField] string bloodDecalPath;

        protected bool isAgentActive;

        protected Transform attackTarget;
        protected Rigidbody enemyRB;
        protected EnemyNodeData enemyNodeData;

        protected NavMeshAgent agent;
        protected bool isDead;
        /// <summary>
        /// When set, IsDead automatically updates
        /// the enemyNodeData equivalent field
        /// with the passed value.
        /// </summary>
        public bool IsDead
        {
            get
            {
                return isDead;
            }
            set
            {
                if (enemyNodeData != null)
                {
                    enemyNodeData.SetIsDead(value);
                }

                if (value == true)
                {
                    UnityAssets.LoadAsync(bloodDecalPath, false, (cb) =>
                    {
                        GameObject temp = Instantiate(cb);
                        temp.transform.position = transform.position;
                        temp.transform.rotation = cb.transform.rotation * Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
                    });
                }


                isDead = value;
            }
        }

        public NavMeshAgent Agent => agent;

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

        /// <summary>
        /// Call to cache the necessary entity components.
        /// </summary>
        protected abstract void CacheComponents();

        //Called in Start to create the entity BT
        protected abstract void CreateNodeData();
        protected abstract void SetupNodeDataFields();
        protected abstract void CreateBTHandler();

        protected abstract void TargetFound();

        public abstract void SetAttackTarget(Transform target);

        public abstract void SetIsAgentActive(bool value);
        public abstract bool GetIsAgentActive();

        public abstract INodeData GetEntityNodeData();
    }
}