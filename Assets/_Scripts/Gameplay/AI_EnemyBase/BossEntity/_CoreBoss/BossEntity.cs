using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using WGR.Core;

namespace WGR.Gameplay.AI
{
    /// <summary>
    /// All the available boss phases.
    /// </summary>
    public enum BossPhase
    {
        StartingPhase,
        BudhaRoomPhase,
        BedroomPhase,
        StunnedPhase,
    }

    /* [CLASS DOCUMENTATION]
     * 
     * Inspector variables: These values must be set from the inspector
     * Private Variables: These values change in runtime.
     * 
     * Base type: AIEntity
     * 
     * [Must know]
     * 0. Phase order: 
     *      A. StartingPhase
     *      B. BudhaRoomPhase
     *      C. BedroomPhase
     *      D. StunnedPhase
     */
    public class BossEntity : AIEntity
    {
        [Header("Set Boss entity values.")]
        [SerializeField] float lifePerPhase;
        [SerializeField] List<Transform> hideRoomsPerPhase;

        #region PRIVATE_VARIABLES
        BossPhase currentBossPhase;
        Transform currentHideRoom;
        float lifePerPhaseCache;

        BossNodeData bossNodeData;
        BossBTHandler btHandler;
        EnemySpawnHandler minionSpawner;
        BulletDetector bulletDetector;

        float currentSpeed;
        bool isStunned;
        bool canAttack = false;

        bool endingActivated;
        #endregion

        private BossAnimations bossAnimations;
        public BossAnimations BossAnimations
        {
            get { return bossAnimations; }
            private set { bossAnimations = value; }
        }

        private bool isDead;
        public bool IsDead
        {
            get
            {
                return isDead;
            }
            set
            {
                bossNodeData.SetIsDead(value);
            }
        }

        #region ENTRY_SETUP
        private void Awake()
        {
            CacheComponents();
        }

        //Base type summary.
        protected override void CacheComponents()
        {
            if (!(agent = GetComponent<NavMeshAgent>()))
                Utils.MissingComponent("NavMeshAgent", this);

            if (!(enemyRB = GetComponent<Rigidbody>()))
                Utils.MissingComponent("Rigidbody", this);

            if (!(FOVManager = GetComponentInChildren<AIEntityFOVManager>()))
                Utils.MissingComponent("EnemyFOVManager", this);

            if (!(AIEntityWeapon = GetComponentInChildren<BossWeapon>()))
                Utils.MissingComponent("EnemyWeapon", this);

            if (!(BossAnimations = GetComponent<BossAnimations>()))
                Utils.MissingComponent("EnemyAnimations", this);

            if (!(minionSpawner = FindObjectOfType<EnemySpawnHandler>()))
                Utils.MissingComponent("EnemySpawnHandler", this);

            if (!(bulletDetector = FindObjectOfType<BulletDetector>()))
                Utils.MissingComponent("BulletDetector", this);
        }

        private void Start()
        {
            InitialSetup();

            if (GameManager.S != null)
            {
                GameManager.S.AIEntityManager.SetBossDetectors(FOVManager);

                attackTarget = GameManager.S.PlayerEntity.transform;

                GameManager.S.GameEventHandler.onGamePause += SetSpeedToZero;
                GameManager.S.GameEventHandler.onGameResumed += ResetSpeed;
                GameManager.S.GameEventHandler.onPlayerDeath += BackToOriginalPos;
            }

            CreateNodeData();
            CreateBTHandler();

            onPlayerFound += TargetFound;

            IsDead = false;
            isAgentActive = true;
        }

        /// <summary>
        /// Call to set up the GameObject for game entry.
        /// </summary>
        void InitialSetup()
        {
            currentBossPhase = BossPhase.StartingPhase;
            lifePerPhaseCache = lifePerPhase;

            currentSpeed = agent.speed;
        }

        /// <summary>
        /// Call to send the agent back to his spawn position.
        /// </summary>
        void BackToOriginalPos()
        {
            bossNodeData.SetTargetIsDead(true);
            bossNodeData.SetTargetFound(false);
        }

        /// <summary>
        /// Call to set the agent speed to 0.
        /// </summary>
        void SetSpeedToZero()
        {
            agent.speed = 0;
            isAgentActive = false;
        }

        /// <summary>
        /// Call to set the agent speed back to his spawn speed.
        /// </summary>
        void ResetSpeed()
        {
            agent.speed = currentSpeed;
            isAgentActive = true;
        }
        #endregion

        #region NODE_DATA_HANDLING
        /// <summary>
        /// Call to create the behaviour tree data container for THIS enemy entity.
        /// <para>Creates a new BossNodeData instance and assigns it in the bossNodeData variable.</para>
        /// </summary>
        protected override void CreateNodeData()
        {
            bossNodeData = new BossNodeData();
            SetupNodeDataFields();
        }

        /// <summary>
        /// Call to set up the needed node data fields for correct startup agent behaviour.
        /// </summary>
        protected override void SetupNodeDataFields()
        {
            bossNodeData.SetEnemyEntity(this);
            bossNodeData.SetOriginalPos(transform.position);
            bossNodeData.SetTargetIsDead(false);
            bossNodeData.SetEnemyAnimations(BossAnimations);
            bossNodeData.SetIsDead(false);
            bossNodeData.SetCurrentBossPhase(currentBossPhase);
            bossNodeData.SetIsHiding(false);
            bossNodeData.SetCanShoot(true);
            bossNodeData.SetTarget(attackTarget);
            bossNodeData.SetNavMeshAgent(agent);
            bossNodeData.SetOcclusionLayers(occlusionLayers);

            bossNodeData.SetWeaponRange(AIEntityWeapon.GetWeaponRange());
        }

        /// <summary>
        /// Call to create an instance of BossBTHandler and assign it to the btHandler varable.
        /// </summary>
        protected override void CreateBTHandler()
        {
            btHandler = new BossBTHandler(bossNodeData, this);
        }
        #endregion

        private void Update()
        {
            //Used for preventing the boss from attacking in the entry cutscene.
            if (!canAttack) return;

            //Used for agent pausing.
            if (!isAgentActive) return;

            //Run the updateBT method ONLY if the btHandler != null or the boss is not in hide mode.
            if (btHandler != null && !isStunned)
            {
                btHandler.UpdateBT();
                currentSpeed = agent.speed;
            }
        }

        /// <summary>
        /// Call to set the agent node data target found value to true.
        /// </summary>
        protected override void TargetFound()
        {
            bossNodeData.SetTargetFound(true);
        }

        #region INTERACTIONS
        /// <summary>
        /// Call to decrease the boss health by one and increase his boss phase to the next phase.
        /// <para>Moves the boss to his next phase hiding room and freezes the player.</para>
        /// <para>If the boss is stunned and gets attacked the bad ending sequence gets initiated.</para>
        /// <para>All enemies are killed if the boss enters his stunned state.</para>
        /// </summary>
        public override void AttackInteraction()
        {
            //If the boss in his stunned state and gets attacked
            //The player gets the bad ending.
            if (isStunned)
            {
                InitiateBadEndingSequence();
                return;
            }

            //Continue only if the Boss is not dead.
            if (IsDead || bossNodeData.GetIsHiding()) return;

            float tempLife = entityLife.GetValue();

            SetHealth(--tempLife);

            if (CheckIfDead())
            {
                //Deactivate the boss player detectors
                FOVManager.DeactivateAllDetectors();
                bossNodeData.SetTargetFound(false);
                bossNodeData.SetCanShoot(true);

                switch (currentBossPhase)
                {
                    //Default boss phase.
                    case BossPhase.StartingPhase:
                        IncrementBossPhase();
                        entityLife.SetValue(lifePerPhaseCache);
                        break;

                    //Second boss phase
                    case BossPhase.BudhaRoomPhase:
                        IncrementBossPhase();
                        entityLife.SetValue(lifePerPhaseCache);
                        break;

                    //Final boss phase
                    case BossPhase.BedroomPhase:
                        IncrementBossPhase();
                        entityLife.SetValue(999999);

                        BossAnimations.SetIsIdleState(true);

                        DropWeaponOnStun();

                        bulletDetector.IsActive = false;

                        if (GameManager.S != null)
                        {
                            GameManager.S.AIEntityManager.KillAllEnemiesInLevel();
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Call to play the boss death animation and invoke the 
        /// Ending_KillBossAndContinue method from BossStunnedSequence object.
        /// </summary>
        void InitiateBadEndingSequence()
        {
            if (endingActivated) return;

            bossAnimations.PlayDeathAnimation();

            endingActivated = true;
            FindObjectOfType<BossStunnedSequence>().Ending_KillBossAndContinue();
        }

        /// <summary>
        /// Call to set the entityLife to the passed value.
        /// </summary>
        protected override void SetHealth(float value)
        {
            entityLife.SetValue(value);
        }

        /// <summary>
        /// Call to check the entity life value.
        /// </summary>
        /// <returns>True if entityLife is smaller than 0, false otherwise.</returns>
        protected override bool CheckIfDead()
        {
            if (entityLife.GetValue() <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Call to move the boss to his next phase based on currentBossPhase.
        /// <para>Every time this method gets called the PlayerEntity controlls get deactivated too.</para>
        /// <para>When the boss is in the BedroomPhase he gets stunned.</para>
        /// </summary>
        void IncrementBossPhase()
        {
            switch (currentBossPhase)
            {
                case BossPhase.StartingPhase:
                    currentBossPhase = BossPhase.BudhaRoomPhase;
                    currentHideRoom = hideRoomsPerPhase[0];

                    minionSpawner.SpawnEnemyWave();

                    bossNodeData.SetCurrentBossPhase(currentBossPhase);
                    bossNodeData.SetCurrentHideRoom(currentHideRoom);
                    bossNodeData.SetIsHiding(true);
                    break;

                case BossPhase.BudhaRoomPhase:
                    currentBossPhase = BossPhase.BedroomPhase;
                    currentHideRoom = hideRoomsPerPhase[1];

                    minionSpawner.EnableFinalBossRoom();

                    bossNodeData.SetCurrentBossPhase(currentBossPhase);
                    bossNodeData.SetCurrentHideRoom(currentHideRoom);
                    bossNodeData.SetIsHiding(true);
                    break;

                case BossPhase.BedroomPhase:
                    currentBossPhase = BossPhase.StunnedPhase;

                    bossNodeData.SetCurrentBossPhase(currentBossPhase);
                    bossNodeData.SetCurrentHideRoom(currentHideRoom);
                    bossNodeData.SetIsHiding(false);
                    bossNodeData.SetIsStunned(true);
                    break;
            }

            currentSpeed = agent.speed;

            if (GameManager.S != null)
            {
                GameManager.S.PlayerEntity.IsActive = false;
                GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.StopActivate);
            }

            foreach (AIEntity aIEntity in GameManager.S.AIEntityManager.GetEnemyEntityRefs())
            {
                if (aIEntity == null) continue;

                aIEntity.GetEntityNodeData().SetOriginalPos(currentHideRoom.position);
            }
        }

        /// <summary>
        /// Call to grab a weapon prefab reference from the weapon pool based and place it in front of the agent.
        /// </summary>
        void DropWeaponOnStun()
        {
            GameObject toBeDropped = GameManager.S.WeaponManager.GetWeaponByType(AIEntityWeapon.equipedWeapon.WeaponType);

            if (toBeDropped != null)
            {
                toBeDropped.transform.position = AIEntityWeapon.GetFirepointTransform().position;
                toBeDropped.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0);

                toBeDropped.SetActive(true);

                aiEntityWeapon.ClearWeaponSprite();
            }
        }

        /// <summary>
        /// Call to disable the boss behaviour and invoke the:
        /// <para>GameEventHandler.OnBossStunnedPhase event</para>
        /// <para>GameSoundsHandler.ForcePlayBossStunnedMusic method.</para>
        /// </summary>
        public void OnBossStunEnd()
        {
            //Prevents the BT from updating
            isStunned = true;
            enemyRB.isKinematic = true;

            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.OnBossStunnedPhase();
                GameManager.S.GameSoundsHandler.ForcePlayBossStunnedMusic();
            }
        }

        /// <summary>
        /// <para>Boss is not stunable, nothing happens.</para>
        /// Call to play a punch SFX.
        /// </summary>
        public override void StunInteraction()
        {
            /*Boss is not stunable*/

            GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.Punch);
        }
        #endregion

        #region UTILITIES
        /// <summary>
        /// Call to get the node data of THIS entity.
        /// </summary>
        public override INodeData GetEntityNodeData()
        {
            return bossNodeData;
        }

        /// <summary>
        /// Call to get the NavMeshAgent component reference.
        /// </summary>
        public override NavMeshAgent GetAgent() => agent;

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

        /// <summary>
        /// Call to enable the boss attack behvaiour.
        /// </summary>
        public void EnableBossEntryAttacking()
        {
            canAttack = true;
            TargetFound();
        }
        #endregion

        private void OnDestroy()
        {
            if (GameManager.S != null)
            {
                GameManager.S.GameEventHandler.onPlayerDeath -= BackToOriginalPos;

                GameManager.S.GameEventHandler.onGamePause -= SetSpeedToZero;
                GameManager.S.GameEventHandler.onGameResumed -= ResetSpeed;
            }

            onPlayerFound -= TargetFound;
        }
    }
}