using System.Collections.Generic;

namespace WGRF.AI
{
    public class EnemyBTHandler : AIEntityBTHandler
    {
        EnemyEntity enemyEntity;
        EnemyNodeData enemyNodeData;

        BehaviourTree mainBehaviour;
        /* BehaviourTree patrolBehaviour;
        BehaviourTree chaseBehaviour; */

        public EnemyNodeData NodeData => enemyNodeData;

        public EnemyBTHandler(EnemyNodeData nodeData, EnemyEntity enemyEntity)
        {
            this.enemyNodeData = nodeData;
            this.enemyEntity = enemyEntity;

            CreateBehaviourTree();
        }

        //Base type summary
        protected override void CreateBehaviourTree()
        {
            /*             IdleNode idle = new IdleNode(NodeData);

                        IdleOnArrival idleOnArrival = new IdleOnArrival(NodeData);
                        PatrolCheckDistance patrolCheckDistance = new PatrolCheckDistance(NodeData, idleOnArrival);
                        PatrolNavigateTo patrolNavigateTo = new PatrolNavigateTo(NodeData);
                        PatrolNavToActivator navToActivator = new PatrolNavToActivator(NodeData, patrolNavigateTo);
                        PatrolNode patrolNode = new PatrolNode(NodeData, patrolCheckDistance, navToActivator);

                        //Create the patrol BT
                        patrolBehaviour = new BehaviourTree(patrolNode, NodeData);

                        AttackTargetAction attackTargetAction = new AttackTargetAction(NodeData, enemyEntity.Controller.Access<EnemyWeapon>("eWeapon").ShootSequence);
                        NavToTarget navToTarget = new NavToTarget(NodeData);

                        ChaseAttackSelector chaseTarget = new ChaseAttackSelector(NodeData, navToTarget, attackTargetAction);
                        ChaseTargetActivator chaseActivator = new ChaseTargetActivator(NodeData, chaseTarget);

                        //Create the chase BT
                        chaseBehaviour = new BehaviourTree(chaseActivator, NodeData);

                        List<INode> childrenActivator = new List<INode>()
                        {
                            patrolBehaviour, chaseBehaviour, idle
                        }; */

            //Create the node activator
            IdlePatrolChaseActivator idlePatrolActivator = new IdlePatrolChaseActivator(NodeData, null);//childrenActivator);

            /* NavToOriginalPos navToOriginalPos = new NavToOriginalPos(NodeData, idlePatrolActivator);

            StunedAction stunedAction = new StunedAction(NodeData);

            CheckIfStunned isStunned = new CheckIfStunned(NodeData, stunedAction, navToOriginalPos); */

            //Connect with idle, attack, fallback selector
            CheckIfDead isDead = new CheckIfDead(NodeData, null);//isStunned);

            //Create the Main Behaviour tree
            mainBehaviour = new BehaviourTree(isDead, NodeData);
        }

        //Base type summary.
        public override void UpdateBT()
        {
            if (mainBehaviour == null) return;

            mainBehaviour.Run();
        }
    }
}