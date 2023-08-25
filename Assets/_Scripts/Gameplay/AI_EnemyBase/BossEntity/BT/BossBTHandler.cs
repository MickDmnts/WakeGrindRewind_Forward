using WGR.AI.Nodes;

namespace WGR.AI.Entities.Hostile.Boss
{
    /*
     * This class file handles the creation and caching of the Boss
     * Behaviour Tree.
     */
    [System.Serializable]
    public class BossBTHandler : AIEntityBTHandler
    {
        public BossNodeData bossNodeData;

        BossEntity bossEntity;

        BehaviourTree mainBehaviour;
        BehaviourTree chaseBehaviour;

        public BossBTHandler(BossNodeData bossNodeData, BossEntity bossEntity)
        {
            this.bossNodeData = bossNodeData;
            this.bossEntity = bossEntity;

            CreateBehaviourTree();
        }

        //Base type summary
        protected override void CreateBehaviourTree()
        {
            #region CHASE_ATTACK
            AttackTargetAction attackTargetAction = new AttackTargetAction(bossNodeData, bossEntity.AIEntityWeapon.ShootSequence);
            NavToTarget navToTarget = new NavToTarget(bossNodeData);

            ChaseAttackSelector chaseTarget = new ChaseAttackSelector(bossNodeData, navToTarget, attackTargetAction);
            ChaseTargetActivator chaseActivator = new ChaseTargetActivator(bossNodeData, chaseTarget);

            //Create the chase BT
            chaseBehaviour = new BehaviourTree(chaseActivator, bossNodeData);
            #endregion

            #region BOSS_PHASES
            BossPhaseUpdater bossDefaultPhase = new BossPhaseUpdater(bossNodeData, chaseBehaviour);
            BossPhaseUpdater bossSecondPhase = new BossPhaseUpdater(bossNodeData, chaseBehaviour);
            BossPhaseUpdater bossThirdPhase = new BossPhaseUpdater(bossNodeData, chaseBehaviour);

            BossStunnedPhase bossStunnedPhase = new BossStunnedPhase(bossNodeData, bossEntity.OnBossStunEnd);
            #endregion

            #region PHASE_SELECTOR
            //The BossBT is built in such way that can be further developed for
            //a future release.
            //Phase behaviour can be changed by swapping a phase node below
            INode[] bossPhases = new INode[4]
            {
                //Phase1 - entry phase
                bossDefaultPhase,

                //Phase2 - Budha room 
                bossSecondPhase,

                //Phase3 - bedroom phase
                bossThirdPhase,

                //Phase4 - Stunned phase
                bossStunnedPhase,
            };

            BossPhaseSelector bossPhaseSelector = new BossPhaseSelector(bossNodeData, bossPhases);
            #endregion

            NavToRoom navToRoom = new NavToRoom(bossNodeData, bossPhaseSelector);

            #region MAIN_BT_ENTRY
            NavToOriginalPos navToOriginalPos = new NavToOriginalPos(bossNodeData, navToRoom);

            mainBehaviour = new BehaviourTree(navToOriginalPos, bossNodeData);
            #endregion
        }

        //Base type summary.
        public override void UpdateBT()
        {
            if (mainBehaviour == null) return;

            mainBehaviour.Run();
        }
    }
}