namespace WGR.Gameplay.AI
{
    /*
     * This interface is used in behaviour tree node creation.
     * It also ensures the type-safety of the classes.
     */
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