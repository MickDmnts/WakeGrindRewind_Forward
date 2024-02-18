namespace WGRF.AI
{
    /// <summary>
    /// The enforcer idling action node
    /// </summary>
    public class EnforcerIdleNode : INode
    {
        ///<summary>The enforer blackboard cache</summary>
        EnforcerNodeData nodeData;

        ///<summary>Creates an EnforcerIdleNode instance</summary>
        public EnforcerIdleNode(INodeData nodeData)
        { this.nodeData = (EnforcerNodeData)nodeData; }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData()
        { return nodeData; }

        public bool Run()
        { return !nodeData.CanProtect; }
    }
}