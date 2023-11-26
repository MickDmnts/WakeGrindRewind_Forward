using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using WGRF.AI.FOV;
using WGRF.AI.Nodes;
using WGRF.Core;
using WGRF.BattleSystem;

namespace WGRF.AI.Entities.Hostile.Generic
{
    public class EnemyEntity : AIEntity
    {
        #region INSPECTOR_VARIABLES
        [Header("Set Armor value IF any")]
        [SerializeField] float armorValue;

        [Header("Set in inspector")]
        [SerializeField] bool isPatroller;
        [SerializeField] float idleTimeOnPatrol;
        [SerializeField] bool attackAfterStun;
        [SerializeField] List<Transform> waypoints;
        [SerializeField] float waypointDistanceOffset;

        [Header("Decal on death")]
        [SerializeField] GameObject bloodDecal;

        [HideInInspector] public bool IsStunned { get; set; }
        #endregion

        #region PRIVATE_VARIABLES
        private bool isDead;
        /// <summary>
        /// When set, IsDead automatically updates
        /// the enemyNodeData equivalent field
        /// with the passed value.
        /// <para>If the value passed is true, instatiates a blood pool VFX.</para>
        /// <para>If the value passed is true AND the focused scene is the BOSS FIGHT, calls the GameEventHandler.OnBossLevelEnemyDeath event.</para>
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
                    Instantiate(bloodDecal, transform.position, bloodDecal.transform.rotation * Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));

                    /*if (ManagerHub.S.LevelManager.FocusedScene.Equals(GameScenes.Level5))
                    {
                        //Notify the boss fight countdown handler that an enemy died.
                        ManagerHub.S.GameEventHandler.OnBossLevelEnemyDeath();
                    }*/
                }


                isDead = value;
            }
        }

        EnemyNodeData enemyNodeData;
        EnemyBTHandler btHandler;
        #endregion

        #region Agent_Speed_Cache
        float startSpeed;
        float startAngularSpeed;
        float startAcceleration;
        #endregion

        private EnemyAnimations enemyAnimations;
        public EnemyAnimations EnemyAnimation
        {
            get { return enemyAnimations; }
            private set { enemyAnimations = value; }
        }

        protected override void PreAwake()
        {
            CacheComponents();
        }

        //Base type summary.
        protected override void CacheComponents()
        {
            agent = GetComponent<NavMeshAgent>();

            enemyRB = GetComponent<Rigidbody>();

            FOVManager = GetComponentInChildren<AIEntityFOVManager>();
            AIEntityWeapon = GetComponentInChildren<EnemyWeapon>();

            EnemyAnimation = GetComponent<EnemyAnimations>();
        }

        private void Start()
        {
            InitialSetup();

            if (ManagerHub.S != null)
            {
                ManagerHub.S.GameEventHandler.onPlayerDeath += BackToOriginalPos;

                ManagerHub.S.GameEventHandler.onGamePause += SetSpeedToZero;
                ManagerHub.S.GameEventHandler.onGameResumed += ResetSpeed;

                PassEntityReference();
            }

            CreateNodeData();
            CreateBTHandler();

            onPlayerFound += TargetFound;
        }

        /// <summary>
        /// Call to set up the GameObject for game entry.
        /// </summary>
        void InitialSetup()
        {
            IsDead = false;
            IsStunned = false;

            startSpeed = agent.speed;
            startAngularSpeed = agent.angularSpeed;
            startAcceleration = agent.acceleration;
        }

        /// <summary>
        /// Call to send the agent back to his spawn position.
        /// </summary>
        void BackToOriginalPos()
        {
            if (isDead) return;

            if (enemyNodeData.GetTargetFound() != true) return;

            enemyNodeData.SetTargetIsDead(true);
            enemyNodeData.SetTargetFound(false);
            agent.isStopped = false;
        }

        /// <summary>
        /// Call to set the agent speed to 0.
        /// </summary>
        void SetSpeedToZero()
        {
            agent.speed = 0;
        }

        /// <summary>
        /// Call to set the agent speed back to his spawn speed.
        /// </summary>
        void ResetSpeed()
        {
            agent.speed = startSpeed;
        }

        /// <summary>
        /// Call to pass THIS EnemyEntity script reference to the AIEntityManager.
        /// </summary>
        void PassEntityReference()
        {
            //ManagerHub.S.AIEntityManager.AddEntityReference(this);
        }

        #region NODE_DATA_HANDLING
        /// <summary>
        /// Call to create the behaviour tree data container for THIS enemy entity.
        /// <para>Creates a new EnemyNodeData instance and assigns it in the enemyNodeData variable.</para>
        /// </summary>
        protected override void CreateNodeData()
        {
            enemyNodeData = new EnemyNodeData();
            SetupNodeDataFields();
        }

        /// <summary>
        /// Call to set up the needed node data fields for correct startup agent behaviour.
        /// </summary>
        protected override void SetupNodeDataFields()
        {
            enemyNodeData.SetEnemyEntity(this);
            enemyNodeData.SetOriginalPos(transform.position);
            enemyNodeData.SetEnemyAnimations(EnemyAnimation);
            enemyNodeData.SetIsPatroller(isPatroller);
            enemyNodeData.SetAttackAfterStun(attackAfterStun);
            enemyNodeData.SetIsDead(IsDead);
            enemyNodeData.SetIsStunned(IsStunned);
            enemyNodeData.SetCanShoot(true);
            enemyNodeData.SetWaypoints(waypoints);
            enemyNodeData.SetIdleTimeOnPatrol(idleTimeOnPatrol);
            enemyNodeData.SetWaypointDistanceOffset(waypointDistanceOffset);
            enemyNodeData.SetCurrentWaypointIndex(0);
            enemyNodeData.SetTarget(attackTarget);
            enemyNodeData.SetNavMeshAgent(agent);
            enemyNodeData.SetOcclusionLayers(occlusionLayers);

            enemyNodeData.SetWeaponRange(aiEntityWeapon.GetWeaponRange());
        }

        /// <summary>
        /// Call to create an instance of EnemyBTHandler and assign it to the btHandler varable.
        /// </summary>
        protected override void CreateBTHandler()
        {
            btHandler = new EnemyBTHandler(enemyNodeData, this);
        }
        #endregion

        /// <summary>
        /// Call to set the agent node data target found value to true.
        /// </summary>
        protected override void TargetFound()
        {
            enemyNodeData.SetTargetFound(true);
        }

        #region AGENT_UPDATE
        private void Update()
        {
            //Dont update if the agent is marked as inactive.
            if (!isAgentActive) return;

            //Run the updateBT method ONLY if the btHandler != null
            if (btHandler != null)
            {
                btHandler.UpdateBT();
            }
        }

        #endregion

        #region ATTACK_INTERACTION
        /// <summary>
        /// If the agent has armor, decrease its armor value by 1.
        /// <para>If the agent ahs no armor left, decrease its life value by 1.</para>
        /// <para>Early return if the enemy is dead OR inactive.</para>
        /// <para>Calls CheckIfDead(), if true then calls InitiateDeathSequence().</para>
        /// </summary>
        public override void AttackInteraction()
        {
            //Continue only if the enemy is not dead.
            if (IsDead || !GetIsAgentActive()) return;

            if (armorValue > 0)
            {
                SetArmorValue(--armorValue);
                TargetFound();
            }
            else
            {
                float tempLife = entityLife;

                SetHealth(--tempLife);

                if (CheckIfDead())
                {
                    InitiateDeathSequence();
                }
            }
        }

        /// <summary>
        /// Call to set the armorValue to the passed value.
        /// </summary>
        void SetArmorValue(float value)
        {
            armorValue = value;
        }

        /// <summary>
        /// Call to set the entityLife to the passed value.
        /// </summary>
        protected override void SetHealth(float value)
        {
            entityLife = value;
        }

        /// <summary>
        /// Call to check the entity life value.
        /// </summary>
        /// <returns>True if entityLife is smaller than 0, false otherwise.</returns>
        protected override bool CheckIfDead()
        {
            if (entityLife <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region DEATH_SEQUENCE_INIT
        /// <summary>
        /// Invokes:
        /// <para>AgentDeath_LocalSetup()</para>
        /// <para>AgentDeath_LocalWeaponManagement()</para>
        /// <para>AgentDeath_GlobalNotifiers()</para>
        /// <para>SetIsAgentActive(false value)</para>
        /// </summary>
        void InitiateDeathSequence()
        {
            AgentDeath_LocalSetup();

            AgentDeath_LocalWeaponManagement();

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

            OnObserverDeath();

            EnemyAnimation.PlayDeathAnimation();

            agent.isStopped = true;

            gameObject.layer = (int)EnemyLayer.NonInteractive;
            agent.radius = 0.1f;
        }

        /// <summary>
        /// Invokes:
        /// <para>1. InstatiateWeaponOnDeath()</para>
        /// <para>2. AIEntityWeapon.ClearWeaponSprite()</para>
        /// </summary>
        void AgentDeath_LocalWeaponManagement()
        {
            GenerateWeaponOnDeath();
            AIEntityWeapon.ClearWeaponSprite();
        }

        /// <summary>
        /// Call to grab a weapon prefab reference from the weapon pool based and place it in front of the agent.
        /// <para>Early return if the weapon is of Unarmed category.</para>
        /// </summary>
        void GenerateWeaponOnDeath()
        {
            if (AIEntityWeapon.equipedWeapon.WeaponCategory != WeaponCategory.Unarmed)
            {
                GameObject toBeDropped = null; //ManagerHub.S.WeaponManager.GetWeaponByType(AIEntityWeapon.equipedWeapon.WeaponType);

                if (toBeDropped != null)
                {
                    toBeDropped.transform.position = AIEntityWeapon.GetFirepointTransform().position;
                    toBeDropped.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0);

                    toBeDropped.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Invokes:
        /// <para>GameEventHandler.OnEnemyDeath()</para>
        /// <para>GameEventHandler.CameraShakeOnEnemyDeath(random generated value)</para>
        /// <para>Prints debug message if the GM ref is null.</para>
        /// </summary>
        void AgentDeath_GlobalNotifiers()
        {
            if (ManagerHub.S != null)
            {
                ManagerHub.S.GameEventHandler.OnEnemyDeath();
                float rndShakeStrength = UnityEngine.Random.Range(2f, 7f);
                ManagerHub.S.GameEventHandler.CameraShakeOnEnemyDeath(0.5f, rndShakeStrength);
            }
        }
        #endregion

        /// <summary>
        /// Call to play the the stunned agent animation.
        /// <para>Set is stunned to true.</para>
        /// <para>Play the Punch SFX.</para>
        /// </summary>
        public override void StunInteraction()
        {
            if (IsDead || !GetIsAgentActive()) return;

            EnemyAnimation.SetStunnedAnimationState(true);
            IsStunned = true;
            enemyNodeData.SetIsStunned(true);

            if (ManagerHub.S != null)
            {
                //ManagerHub.S.GameSoundsHandler.PlayOneShot(GameAudioClip.Punch);
            }
        }

        #region UTILITIES
        /// <summary>
        /// Call to get the node data of THIS entity.
        /// </summary>
        public override INodeData GetEntityNodeData()
        {
            return enemyNodeData;
        }

        /// <summary>
        /// Call to get the NavMeshAgent component reference.
        /// </summary>
        public override NavMeshAgent GetAgent() => agent;

        /// <summary>
        /// Call to set the enemyNodeData canShoot to false.
        /// </summary>
        public void DisableShootingBehaviour()
        {
            enemyNodeData.SetCanShoot(false);
        }

        /// <summary>
        /// Call to set the animator playback speed to the passed value.
        /// </summary>
        public void OnPlayerAbilityStart(float animatorPlaybackSpeed)
        {
            EnemyAnimation.SetAnimatorPlaybackSpeed(animatorPlaybackSpeed);
        }

        /// <summary>
        /// Called from each ability when the ability behaviour has finished executing to reset the agent values.
        /// </summary>
        public void OnPlayerAbilityFinish()
        {
            //General ability usage
            agent.speed = startSpeed;
            agent.angularSpeed = startAngularSpeed;
            agent.acceleration = startAcceleration;
            EnemyAnimation.SetAnimatorPlaybackSpeed(1f);

            //For stop time ability
            enemyNodeData.SetCanShoot(true);
        }

        /// <summary>
        /// Called from the EnemyEntityManager at startup to update the target of THIS enemy.
        /// </summary>
        public override void SetAttackTarget(Transform target)
        {
            attackTarget = target;
        }

        /// <summary>
        /// Call to get the agents' current state.
        /// </summary>
        public override bool GetIsAgentActive()
        {
            return isAgentActive;
        }

        /// <summary>
        /// Call to set the agent is active to the passed value.
        /// </summary>
        public override void SetIsAgentActive(bool value)
        {
            isAgentActive = value;
        }

        /// <summary>
        /// Call to get the agents' is dead value.
        /// </summary>
        public override bool GetIsDead()
        {
            return IsDead;
        }
        #endregion

        protected override void PreDestroy()
        {
            if (ManagerHub.S != null)
            {
                ManagerHub.S.GameEventHandler.onPlayerDeath -= BackToOriginalPos;
                ManagerHub.S.GameEventHandler.onGamePause -= SetSpeedToZero;
                ManagerHub.S.GameEventHandler.onGameResumed -= ResetSpeed;
            }

            onPlayerFound -= TargetFound;
        }
    }
}