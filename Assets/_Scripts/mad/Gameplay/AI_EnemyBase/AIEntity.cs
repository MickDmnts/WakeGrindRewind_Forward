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
        Room6,
        Room7,
    }

    /// <summary>
    /// A base class for every ai entity in the game
    /// </summary>
    public abstract class AIEntity : Entity
    {
        ///<summary>The room this agent corresponds to</summary>
        [SerializeField, Header("Enemy Room")]
        [Tooltip("The room this agent corresponds to")] protected EnemyRoom enemyRoom;

        ///<summary>The occlusion check layers for agent traversing</summary>
        [Header("Occlusion check layers")]
        [SerializeField, Tooltip("The occlusion check layers for agent traversing")] protected LayerMask occlusionLayers;

        ///<summary>The layer when the agent dies</summary>
        [Header("After death transition")]
        [SerializeField, Tooltip("The layer when the agent dies")] protected EnemyLayer layerOnDeath;

        ///<summary>The blood decal path</summary>
        [Header("Decal on death")]
        [SerializeField, Tooltip("The blood decal path")] protected string bloodDecalPath;

        ///<summary>Is the agent active?</summary>
        protected bool isAgentActive;
        ///<summary>The target of this agent</summary>
        protected Transform attackTarget;
        ///<summary>The rigidbody of the agent</summary>
        protected Rigidbody enemyRB;
        ///<summary>The agent node data cache</summary>
        protected INodeData enemyNodeData;
        ///<summary>The agent cache</summary>
        protected NavMeshAgent agent;
        ///<summary>The dead state of the agent</summary>
        protected bool isDead;
        ///<summary>Score increase value of this enemy</summary>
        protected int scoreIncrease = 10;

        ///<summary>Returns the room this agent corresponds to </summary>
        public EnemyRoom EnemyRoom => enemyRoom;

        ///<summary>The dead state of the agent</summary>
        public bool IsDead
        {
            get
            {
                return isDead;
            }
            set
            {
                if (value)
                {
                    if (ManagerHub.S.SettingsHandler.UserSettings.goreVFX)
                    {
                        UnityAssets.LoadAsync(bloodDecalPath, false, (cb) =>
                        {
                            GameObject temp = Instantiate(cb);
                            temp.transform.position = transform.position;
                            temp.transform.rotation = cb.transform.rotation * Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));
                        });
                    }

                    ManagerHub.S.ScoreHandler.IncreaseRoomScoreBy(scoreIncrease);
                    ManagerHub.S.HUDHandler.BloomOnKill();
                }

                isDead = value;
            }
        }

        public NavMeshAgent Agent => agent;

        public abstract void SetIsAgentActive(bool value);

        public abstract INodeData GetEntityNodeData();

        public abstract void InitiateFallback(float range);
        public abstract void OnPlayerAbilityStart(float animatorPlaybackSpeed);
        public abstract void OnPlayerAbilityFinish();
        public abstract void DisableShootingBehaviour();
    }
}