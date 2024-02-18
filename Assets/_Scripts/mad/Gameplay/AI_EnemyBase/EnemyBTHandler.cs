using System.Collections.Generic;

namespace WGRF.AI
{
    /// <summary>
    /// The simple enemy behaviour tree handler
    /// </summary>
    public class EnemyBTHandler : AIEntityBTHandler
    {
        ///<summary>The entity of this behaviour tree</summary>
        EnemyEntity enemyEntity;
        ///<summary>The AI node data of this behaviour tree</summary>
        EnemyNodeData enemyNodeData;

        ///<summary>Main behaviour tree (entry)</summary>
        BehaviourTree mainBehaviour;
        ///<summary>Idle behaviour (first phase)</summary>
        BehaviourTree idleBehaviour;
        ///<summary>Attack behaviour (second phase)</summary>
        BehaviourTree attackBehaviour;
        ///<summary>Fallback behaviour (final phase)</summary>
        BehaviourTree fallbackBehaviour;

        ///<summary>The assigned AIs node data</summary>
        public EnemyNodeData NodeData => enemyNodeData;

        ///<summary>Creates a EnemyBTHandler instance</summary>
        public EnemyBTHandler(EnemyNodeData nodeData, EnemyEntity enemyEntity)
        {
            this.enemyNodeData = nodeData;
            this.enemyEntity = enemyEntity;

            CreateBehaviourTree();
        }

        //Base type summary
        protected override void CreateBehaviourTree()
        {
            FallbackAction fallbackAction = new FallbackAction(NodeData);
            fallbackBehaviour = new BehaviourTree(fallbackAction, NodeData);

            AttackTargetAction attackTargetAction = new AttackTargetAction(NodeData, enemyEntity.Controller.Access<EnemyWeapon>("eWeapon").ShootSequence);
            NavToTarget navToTarget = new NavToTarget(NodeData);
            ChaseAttackSelector chaseTarget = new ChaseAttackSelector(NodeData, navToTarget, attackTargetAction);
            ChaseTargetActivator chaseActivator = new ChaseTargetActivator(NodeData, chaseTarget);
            attackBehaviour = new BehaviourTree(chaseActivator, NodeData);

            IdleNode idleNode = new IdleNode(NodeData);
            idleBehaviour = new BehaviourTree(idleNode, NodeData);

            BehaviourSelector idleAttackFallbackSelector = new BehaviourSelector(NodeData, new List<INode>() { fallbackBehaviour, attackBehaviour, idleBehaviour });

            //Connect with idle, attack, fallback selector
            CheckIfDead isDead = new CheckIfDead(NodeData, idleAttackFallbackSelector);

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