using System.Collections;
using UnityEngine;
using UnityEngine.AI;

using WGRF.Core;

namespace WGRF.AI
{
    /// <summary>
    /// The simple enemy entity agent handler
    /// </summary>
    public class EnemyEntity : AIEntity
    {
        ///<summary>The armor value of this agent</summary>
        [Header("Set Armor value IF any")]
        [SerializeField, Tooltip("The armor value of this agent")] float armorValue;

        ///<summary>The behaviour tree handler of the agent</summary>
        EnemyBTHandler btHandler;
        ///<summary>The player sprite renderer</summary>
        SpriteRenderer spriteRenderer;
        ///<summary>The default sprite color</summary>
        Color spriteColorCache;

        ///<summary>Returns the target of this agent.</summary>
        public Transform Target => attackTarget;
        ///<summary>Returns the active state of the agent</summary>
        public bool IsAgentActive => isAgentActive;

        /// <summary>
        /// Call to set the agent is active to the passed value.
        /// </summary>
        public override void SetIsAgentActive(bool value)
        {
            ((EnemyNodeData)enemyNodeData).CanAttack = true;
            ((EnemyNodeData)enemyNodeData).CanShoot = true;
            isAgentActive = value;
        }

        /// <summary>
        /// Call to get the node data of THIS entity.
        /// </summary>
        public override INodeData GetEntityNodeData()
        { return enemyNodeData; }

        protected override void PreAwake()
        {
            SetController(GetComponent<Controller>());
            agent = GetComponent<NavMeshAgent>();
            enemyRB = GetComponent<Rigidbody>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteColorCache = spriteRenderer.color;
        }

        private void Start()
        {
            IsDead = false;
            enemyNodeData = new EnemyNodeData(this);
            btHandler = new EnemyBTHandler((EnemyNodeData)enemyNodeData, this);
            attackTarget = ManagerHub.S.PlayerController.gameObject.transform;
            ManagerHub.S.AIHandler.RegisterAgent(enemyRoom, this);
        }

        private void Update()
        {
            //Dont update if the agent is marked as inactive.
            if (!isAgentActive || ManagerHub.S.GameState == GameState.Paused) return;

            //Run the updateBT method ONLY if the btHandler != null
            if (btHandler != null)
            { btHandler.UpdateBT(); }
        }

        /// <summary>
        /// If the agent has armor, decrease its armor value by 1.
        /// <para>If the agent ahs no armor left, decrease its life value by 1.</para>
        /// <para>Early return if the enemy is dead OR inactive.</para>
        /// <para>Calls CheckIfDead(), if true then calls InitiateDeathSequence().</para>
        /// </summary>
        public override void AttackInteraction(int damage)
        {
            //Continue only if the enemy is not dead.
            if (IsDead) return;

            if (!ManagerHub.S.SettingsHandler.UserSettings.goreVFX)
            { StartCoroutine(TurnRed()); }

            if (armorValue > 0)
            { armorValue -= damage; }
            else
            {
                entityLife -= damage;
                if (entityLife <= 0)
                { InitiateDeathSequence(); }
            }
        }

        ///<summary>Briefly turns the entity sprite red</summary>
        IEnumerator TurnRed()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSecondsRealtime(0.5f);
            spriteRenderer.color = spriteColorCache;
        }

        /// <summary>
        /// Invokes:
        /// <para>AgentDeath_LocalSetup()</para>
        /// <para>AgentDeath_GlobalNotifiers()</para>
        /// <para>SetIsAgentActive(false value)</para>
        /// </summary>
        void InitiateDeathSequence()
        {
            AgentDeath_LocalSetup();

            Controller.Access<EnemyWeapon>("eWeapon").ClearWeaponSprite();

            AgentDeath_GlobalNotifiers();

            //Deactivate the agent at the end.
            SetIsAgentActive(false);
        }

        /// <summary>
        /// Call to set up the agent for death simulation.
        /// <para>Calls OnObserverDeath() event.</para>
        /// <para>Plays the agent death animation.</para>
        /// <para>Deactivates the agent and moves his GameObject to the inactive layer.</para>
        /// </summary>
        void AgentDeath_LocalSetup()
        {
            IsDead = true;

            Controller.Access<EnemyAnimations>("eAnimations").PlayDeathAnimation();

            agent.isStopped = true;

            gameObject.layer = (int)EnemyLayer.NonInteractive;
            agent.radius = 0.1f;
        }

        /// <summary>
        /// Invokes:
        /// <para>GameEventHandler.OnEnemyDeath()</para>
        /// <para>GameEventHandler.CameraShakeOnEnemyDeath(random generated value)</para>
        /// <para>Prints debug message if the GM ref is null.</para>
        /// </summary>
        void AgentDeath_GlobalNotifiers()
        {
            ManagerHub.S.GameEventHandler.OnEnemyDeath();
            float rndShakeStrength = Random.Range(2f, 7f);
            ManagerHub.S.GameEventHandler.CameraShakeOnEnemyDeath(0.5f, rndShakeStrength);
        }

        /// <summary>
        /// Call to set the enemyNodeData canShoot to false.
        /// </summary>
        public override void DisableShootingBehaviour()
        { ((EnemyNodeData)enemyNodeData).CanShoot = false; }

        /// <summary>
        /// Call to set the animator playback speed to the passed value.
        /// </summary>
        public override void OnPlayerAbilityStart(float animatorPlaybackSpeed)
        { Controller.Access<EnemyAnimations>("eAnimations").SetAnimatorPlaybackSpeed(animatorPlaybackSpeed); }

        /// <summary>
        /// Called from each ability when the ability behaviour has finished executing to reset the agent values.
        /// </summary>
        public override void OnPlayerAbilityFinish()
        {
            Controller.Access<EnemyAnimations>("eAnimations").SetAnimatorPlaybackSpeed(1f);

            //For stop time ability
            ((EnemyNodeData)enemyNodeData).CanShoot = true;
        }

        /// <summary>
        /// Call to initiate the new position searching and agent moving to it.
        /// </summary>
        /// <param name="range">The maximum range to search a position.</param>
        public override void InitiateFallback(float range)
        {
            StopAllCoroutines();
            StartCoroutine(SearchPosition(transform.position, range));
        }

        /// <summary> Searches for a new agent destination around the passed center with a max range </summary>
        IEnumerator SearchPosition(Vector3 center, float range)
        {
            int reps = 30;
            Vector3 possiblePos = Vector3.zero;

            for (int i = 0; i < reps; i++)
            {
                //Translate XY to XZ
                Vector2 randomHit = Random.insideUnitCircle;
                Vector3 xzTranslation = new Vector3(randomHit.x, 0f, randomHit.y);

                //set random point
                Vector3 randomPoint = center + xzTranslation * range;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, Agent.height * 2, NavMesh.AllAreas))
                {
                    if ((hit.position - transform.position).magnitude >= range / 1.5f)
                    { possiblePos = hit.position; }
                }

                yield return null;
            }

            if (possiblePos == Vector3.zero)
            { possiblePos = transform.position; }

            //Continue to new location
            ManagerHub.S.GameSoundsHandler.PlayOneShotSFX(GameAudioClip.Fart);
            Agent.SetDestination(possiblePos);
            StopAllCoroutines();
        }
    }
}