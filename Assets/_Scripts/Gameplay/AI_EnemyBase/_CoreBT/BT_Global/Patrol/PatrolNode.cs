namespace WGR.Gameplay.AI
{
    /* [Node documentation]
     * 
     * [Must know]
     *  EnemyNodeData compatible.
     */
    public class PatrolNode : INode
    {
        EnemyNodeData nodeData;

        INode distanceCheck, navTo;

        public PatrolNode(EnemyNodeData nodeData, INode distanceCheck, INode navTo)
        {
            this.nodeData = nodeData;

            this.distanceCheck = distanceCheck;
            this.navTo = navTo;
        }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData()
        {
            return nodeData;
        }

        /// <summary>
        /// Call to check if the agent has found its target OR if he is not a patroller, if true, returns false for early exit.
        /// <para>If the navTo INode returns false, calls the Run method of distanceCheck INode and returns its value.</para>
        /// <para>Returns false if none of the above apply.</para>
        /// </summary>
        public bool Run()
        {
            if (nodeData.GetTargetFound()
                || !nodeData.GetIsPatroller()) return false;

            if (!navTo.Run())
            {
                return distanceCheck.Run();
            }
            return false;
        }
    }
}