using System.Collections.Generic;

namespace WGRF.AI
{
    /*
     * This class file handles the creation and caching of the Enemy
     * Behaviour Tree.
     */
    [System.Serializable]
    public class EnemyBTHandler : AIEntityBTHandler
    {
        public EnemyNodeData nodeData;

        EnemyEntity enemyEntity;

        BehaviourTree mainBehaviour;
        BehaviourTree patrolBehaviour;
        BehaviourTree chaseBehaviour;

        public EnemyBTHandler(EnemyNodeData nodeData, EnemyEntity enemyEntity)
        {
            this.nodeData = nodeData;
            this.enemyEntity = enemyEntity;

            CreateBehaviourTree();
        }

        //Base type summary
        protected override void CreateBehaviourTree()
        {
            #region IDLE
            IdleNode idle = new IdleNode(nodeData);
            #endregion

            #region PATROL
            IdleOnArrival idleOnArrival = new IdleOnArrival(nodeData);
            PatrolCheckDistance patrolCheckDistance = new PatrolCheckDistance(nodeData, idleOnArrival);
            PatrolNavigateTo patrolNavigateTo = new PatrolNavigateTo(nodeData);
            PatrolNavToActivator navToActivator = new PatrolNavToActivator(nodeData, patrolNavigateTo);
            PatrolNode patrolNode = new PatrolNode(nodeData, patrolCheckDistance, navToActivator);

            //Create the patrol BT
            patrolBehaviour = new BehaviourTree(patrolNode, nodeData);
            #endregion

            #region CHASE_ATTACK
            AttackTargetAction attackTargetAction = new AttackTargetAction(nodeData, enemyEntity.AIEntityWeapon.ShootSequence);
            NavToTarget navToTarget = new NavToTarget(nodeData);

            ChaseAttackSelector chaseTarget = new ChaseAttackSelector(nodeData, navToTarget, attackTargetAction);
            ChaseTargetActivator chaseActivator = new ChaseTargetActivator(nodeData, chaseTarget);

            //Create the chase BT
            chaseBehaviour = new BehaviourTree(chaseActivator, nodeData);
            #endregion

            #region MAIN_BT
            List<INode> childrenActivator = new List<INode>()
            {
                patrolBehaviour, chaseBehaviour, idle
            };

            //Create the node activator
            IdlePatrolChaseActivator idlePatrolActivator = new IdlePatrolChaseActivator(nodeData, childrenActivator);

            NavToOriginalPos navToOriginalPos = new NavToOriginalPos(nodeData, idlePatrolActivator);

            StunedAction stunedAction = new StunedAction(nodeData);

            CheckIfStunned isStunned = new CheckIfStunned(nodeData, stunedAction, navToOriginalPos);

            CheckIfDead isDead = new CheckIfDead(nodeData, isStunned);

            //Create the Main Behaviour tree
            mainBehaviour = new BehaviourTree(isDead, nodeData);
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