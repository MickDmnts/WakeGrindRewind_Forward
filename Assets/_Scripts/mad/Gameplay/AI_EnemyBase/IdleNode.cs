namespace WGRF.AI
{
    /* [Node documentation]
     * 
     * [Custom Action]
     *  If the AI is not a patroller automatically disables their walk animation.
     * 
     * [Must know]
     *  EnemyNodeData compatible.
     *  Passed to the IdlePatrolChaseSelector as a INode child.
     */
    public class IdleNode : INode
    {
        EnemyNodeData nodeData;

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