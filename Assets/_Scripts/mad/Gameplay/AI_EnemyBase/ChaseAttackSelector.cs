namespace WGRF.AI
{
    /// <summary>
    /// Updates the NavToTarget continuously until it returns false, and then returns the return value of the AttackTargetAction Run method.
    /// </summary>
    public class ChaseAttackSelector : INode
    {
        ///<summary>The blackboard of this action</summary>
        INodeData nodeData;
        ///<summary>The nav to target action</summary>
        NavToTarget navToTarget;
        ///<summary>The attack target action</summary>
        AttackTargetAction attackTarget;

        ///<summary>Creates a ChaseAttackSelector instance</summary>
        public ChaseAttackSelector(INodeData nodeData, NavToTarget navToTarget, AttackTargetAction attackTarget)
        {
            this.nodeData = nodeData;
            this.navToTarget = navToTarget;
            this.attackTarget = attackTarget;
        }

        public INodeData GetNodeData()
        { return nodeData; }

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
