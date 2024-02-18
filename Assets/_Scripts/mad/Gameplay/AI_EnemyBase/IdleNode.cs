namespace WGRF.AI
{
    /// <summary>
    /// Idling action node
    /// </summary>
    public class IdleNode : INode
    {
        ///<summary>The agent blackboard</summary>
        EnemyNodeData nodeData;

        ///<summary>Creates an IdleNode instance</summary>
        public IdleNode(INodeData nodeData)
        { this.nodeData = (EnemyNodeData)nodeData; }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData()
        { return nodeData; }

        /// <summary>
        /// Checks if the AI is a patroller through the NodeData passed.
        /// <para>If false, sets their walk animation state to false and returns true,
        /// else returns false.</para>
        /// </summary>
        public bool Run()
        { return !nodeData.CanAttack; }
    }
}