namespace WGRF.AI
{
    /// <summary>
    /// The behaviour tree base class
    /// </summary>
    public class BehaviourTree : INode
    {
        ///<summary>The blackboard of this action</summary>
        INodeData nodeData;
        ///<summary>The root node of the tree</summary>
        INode root;

        /// <summary>
        /// Creates a BehaviourTree instance
        /// </summary>
        public BehaviourTree(INode root, INodeData nodeData)
        {
            this.root = root;
            this.nodeData = nodeData;
        }

        /// <summary>
        /// Call to update the branches of the BT.
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            return root.Run();
        }

        /// <summary>
        /// Call to get the node data attached to THIS behaviou tree.
        /// </summary>
        /// <returns></returns>
        public INodeData GetNodeData()
        {
            return nodeData;
        }
    }
}