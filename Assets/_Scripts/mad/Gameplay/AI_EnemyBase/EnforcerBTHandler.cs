using System.Collections.Generic;

namespace WGRF.AI
{
    /// <summary>
    /// The enforcer enemy behaviour tree handler
    /// </summary>
    public class EnforcerBTHandler : AIEntityBTHandler
    {
        ///<summary>The entity of this behaviour tree</summary>
        EnforcerEntity enemyEntity;
        ///<summary>The AI node data of this behaviour tree</summary>
        EnforcerNodeData enemyNodeData;

        ///<summary>Main behaviour tree (entry)</summary>
        BehaviourTree mainBehaviour;
        ///<summary>Idle behaviour (first phase)</summary>
        BehaviourTree idleBehaviour;
        ///<summary>Attack behaviour (second phase)</summary>
        BehaviourTree protectBehaviour;
        ///<summary>Fallback behaviour (final phase)</summary>
        BehaviourTree fallbackBehaviour;

        ///<summary>The assigned AIs node data</summary>
        public EnforcerNodeData NodeData => enemyNodeData;

        ///<summary>Creates an EnforcerBTHandler instance</summary>
        public EnforcerBTHandler(EnforcerNodeData nodeData, EnforcerEntity enemyEntity)
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

            ProtectVulnerable protectVulnerable = new ProtectVulnerable(NodeData);
            FindMostVulnerable findMostVulnerable = new FindMostVulnerable(NodeData);
            ProtectionSelector selector = new ProtectionSelector(findMostVulnerable, protectVulnerable, NodeData);
            protectBehaviour = new BehaviourTree(selector, NodeData);

            EnforcerIdleNode idleNode = new EnforcerIdleNode(NodeData);
            idleBehaviour = new BehaviourTree(idleNode, NodeData);

            BehaviourSelector idleAttackFallbackSelector = new BehaviourSelector(NodeData, new List<INode>() { fallbackBehaviour, protectBehaviour, idleBehaviour });
            CheckIfDead isDead = new CheckIfDead(NodeData, idleAttackFallbackSelector);
            //Creates the Main Behaviour tree
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