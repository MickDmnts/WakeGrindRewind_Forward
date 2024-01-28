using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using WGRF.Core;
using WGRF.BattleSystem;

namespace WGRF.AI
{
    public class EnemyEntity : AIEntity
    {
        [Header("Set Armor value IF any")]
        [SerializeField] float armorValue;

        [Header("Set in inspector")]
        [SerializeField] bool isPatroller;
        [SerializeField] float idleTimeOnPatrol;
        [SerializeField] bool attackAfterStun;
        [SerializeField] List<Transform> waypoints;
        [SerializeField] float waypointDistanceOffset;

        EnemyBTHandler btHandler;

        protected override void PreAwake()
        {
            CacheComponents();
        }

        protected override void CacheComponents()
        {
            agent = GetComponent<NavMeshAgent>();
            enemyRB = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            IsDead = false;
            //ManagerHub.S.AIEntityManager.AddEntityReference(this);

            CreateNodeData();
            CreateBTHandler();

            onPlayerFound += TargetFound;
        }

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
            enemyNodeData.SetEnemyAnimations(Controller.Access<EnemyAnimations>("eAnimations"));
            enemyNodeData.SetIsPatroller(isPatroller);
            enemyNodeData.SetAttackAfterStun(attackAfterStun);
            enemyNodeData.SetIsDead(IsDead);
            //enemyNodeData.SetIsStunned(IsStunned);
            enemyNodeData.SetCanShoot(true);
            enemyNodeData.SetWaypoints(waypoints);
            enemyNodeData.SetIdleTimeOnPatrol(idleTimeOnPatrol);
            enemyNodeData.SetWaypointDistanceOffset(waypointDistanceOffset);
            enemyNodeData.SetCurrentWaypointIndex(0);
            enemyNodeData.SetTarget(attackTarget);
            enemyNodeData.SetNavMeshAgent(agent);
            enemyNodeData.SetOcclusionLayers(occlusionLayers);

            enemyNodeData.SetWeaponRange(Controller.Access<EnemyWeapon>("eWeapon").GetWeaponRange());
        }

        /// <summary>
        /// Call to create an instance of EnemyBTHandler and assign it to the btHandler varable.
        /// </summary>
        protected override void CreateBTHandler()
        { btHandler = new EnemyBTHandler(enemyNodeData, this); }

        /// <summary>
        /// Call to set the agent node data target found value to true.
        /// </summary>
        protected override void TargetFound()
        { enemyNodeData.SetTargetFound(true); }

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

        /// <summary>
        /// If the agent has armor, decrease its armor value by 1.
        /// <para>If the agent ahs no armor left, decrease its life value by 1.</para>
        /// <para>Early return if the enemy is dead OR inactive.</para>
        /// <para>Calls CheckIfDead(), if true then calls InitiateDeathSequence().</para>
        /// </summary>
        public override void AttackInteraction(int damage)
        {
            //Continue only if the enemy is not dead.
            if (IsDead || !GetIsAgentActive()) return;

            if (armorValue > 0)
            {
                armorValue -= damage;
                TargetFound();
            }
            else
            {
                entityLife -= damage;
                if (entityLife <= 0)
                {
                    InitiateDeathSequence();
                }
            }
        }

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

            Controller.Access<EnemyAnimations>("eAnimations").PlayDeathAnimation();

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
            Controller.Access<EnemyWeapon>("eWeapon").ClearWeaponSprite();
        }

        /// <summary>
        /// Call to grab a weapon prefab reference from the weapon pool based and place it in front of the agent.
        /// <para>Early return if the weapon is of Unarmed category.</para>
        /// </summary>
        void GenerateWeaponOnDeath()
        {
            if (Controller.Access<EnemyWeapon>("eWeapon").equipedWeapon.WeaponCategory != WeaponCategory.Unarmed)
            {
                GameObject toBeDropped = null; //ManagerHub.S.WeaponManager.GetWeaponByType(AIEntityWeapon.equipedWeapon.WeaponType);

                if (toBeDropped != null)
                {
                    toBeDropped.transform.position = Controller.Access<EnemyWeapon>("eWeapon").GetFirepointTransform().position;
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

            ManagerHub.S.GameEventHandler.OnEnemyDeath();
            float rndShakeStrength = UnityEngine.Random.Range(2f, 7f);
            ManagerHub.S.GameEventHandler.CameraShakeOnEnemyDeath(0.5f, rndShakeStrength);
        }

        /// <summary>
        /// Call to get the node data of THIS entity.
        /// </summary>
        public override INodeData GetEntityNodeData()
        {
            return enemyNodeData;
        }

        /// <summary>
        /// Call to set the enemyNodeData canShoot to false.
        /// </summary>
        public void DisableShootingBehaviour()
        { enemyNodeData.SetCanShoot(false); }

        /// <summary>
        /// Call to set the animator playback speed to the passed value.
        /// </summary>
        public void OnPlayerAbilityStart(float animatorPlaybackSpeed)
        { Controller.Access<EnemyAnimations>("eAnimations").SetAnimatorPlaybackSpeed(animatorPlaybackSpeed); }

        /// <summary>
        /// Called from each ability when the ability behaviour has finished executing to reset the agent values.
        /// </summary>
        public void OnPlayerAbilityFinish()
        {
            Controller.Access<EnemyAnimations>("eAnimations").SetAnimatorPlaybackSpeed(1f);

            //For stop time ability
            enemyNodeData.SetCanShoot(true);
        }

        /// <summary>
        /// Called from the EnemyEntityManager at startup to update the target of THIS enemy.
        /// </summary>
        public override void SetAttackTarget(Transform target)
        { attackTarget = target; }

        /// <summary>
        /// Call to get the agents' current state.
        /// </summary>
        public override bool GetIsAgentActive()
        { return isAgentActive; }

        /// <summary>
        /// Call to set the agent is active to the passed value.
        /// </summary>
        public override void SetIsAgentActive(bool value)
        { isAgentActive = value; }

        protected override void PreDestroy()
        { onPlayerFound -= TargetFound; }
    }
}