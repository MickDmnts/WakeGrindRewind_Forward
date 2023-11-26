namespace WGRF.AI.Nodes
{
    /* [Node documentation]
     * 
     * [Custom Selector]
     * Updates the NavToTarget continuously until it returns false, and then returns the return value of the AttackTargetAction Run method.
     * 
     * [Must know]
     *  INodeData compatible.
     */
    public class ChaseAttackSelector : INode
    {
        INodeData nodeData;

        NavToTarget navToTarget;
        AttackTargetAction attackTarget;

        public ChaseAttackSelector(INodeData nodeData, NavToTarget navToTarget, AttackTargetAction attackTarget)
        {
            this.nodeData = nodeData;
            this.navToTarget = navToTarget;
            this.attackTarget = attackTarget;
        }

        public INodeData GetNodeData()
        {
            return nodeData;
        }

        /// <summary>
        /// If the NavToTarget Run method returns false, calls and returns the AttackTargetAction Run method value.
        /// <para>Returns false if none of the above apply.</para>
        /// </summary>
        public bool Run()
        {
            if (!navToTarget.Run())
            {
                return attackTarget.Run();
            }

            return false;
        }
    }
}
