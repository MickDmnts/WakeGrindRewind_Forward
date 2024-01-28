namespace WGRF.AI
{
    /// <summary>
    /// Interface used for Behaviour Tree traversing
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Call to update the node behaviour.
        /// </summary>
        /// <returns></returns>
        bool Run();

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        INodeData GetNodeData();
    }
}