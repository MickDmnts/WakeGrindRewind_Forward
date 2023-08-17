namespace WGR.Gameplay.AI
{
    /*
     * [Root Node]
     *  This is the entry point for every Behaviour Tree created in the game.
     */
    public class BehaviourTree : INode
    {
        INodeData nodeData;
        INode root;

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