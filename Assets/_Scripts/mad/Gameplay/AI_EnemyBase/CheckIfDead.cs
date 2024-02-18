namespace WGRF.AI
{
    /// <summary>
    /// Stops the rest of the execution if the agent is marked as dead
    /// </summary>
    public class CheckIfDead : INode
    {
        ///<summary>The blackboard of this action</summary>
        INodeData nodeData;
        ///<summary>The next node</summary>
        INode child;

        ///<summary>Creates a CheckIfDead instance</summary>
        public CheckIfDead(INodeData nodeData, INode child)
        {
            this.nodeData = nodeData;
            this.child = child;
        }

        public INodeData GetNodeData()
        { return nodeData; }

        /// <summary>
        /// Call to update the INode passed in the node constructor ONLY if the agent is not dead.
        /// <para>Returns the INode passed return value.</para>
        /// <para>Returns false if none of the above apply.</para>
        /// </summary>
        public bool Run()
        {
            if (!nodeData.IsDead)
            {
                return child.Run();
            }

            return false;
        }
    }
}