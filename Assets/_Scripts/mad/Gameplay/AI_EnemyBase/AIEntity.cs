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

    /// <summary>
    /// All the rooms the enemies can be present at.
    /// </summary>
    public enum EnemyRoom
    {
        Room1,
        Room2,
        Room3,
        Room4,
        Room5,
    }

    public abstract class AIEntity : Entity
    {
        [SerializeField, Header("Enemy Room")]
        protected EnemyRoom enemyRoom;
        protected int scoreIncrease = 10;

        [Header("Occlusion check layers")]
        [SerializeField] protected LayerMask occlusionLayers;

        [Header("After death transition")]
        [SerializeField] protected EnemyLayer layerOnDeath;

        [Header("Decal on death")]
        [SerializeField] protected string bloodDecalPath;

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
                    enemyNodeData.IsDead = value;
                }

                if (value == true)
                {
                    UnityAssets.LoadAsync(bloodDecalPath, false, (cb) =>
                    {
                        GameObject temp = Instantiate(cb);
                        temp.transform.position = transform.position;
                        temp.transform.rotation = cb.transform.rotation * Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
                    });

                    ManagerHub.S.ScoreHandler.IncreaseRoomScoreBy(scoreIncrease);
                }


                isDead = value;
            }
        }

        public NavMeshAgent Agent => agent;

        public abstract void SetAttackTarget(Transform target);

        public abstract void SetIsAgentActive(bool value);
        public abstract bool GetIsAgentActive();

        public abstract INodeData GetEntityNodeData();
    }
}