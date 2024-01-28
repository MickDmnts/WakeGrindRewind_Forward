namespace WGRF.AI
{
    /* [Node documentation]
     * 
     * [Must know]
     *  EnemyNodeData compatible.
     */
    public class PatrolNavToActivator : INode
    {
        EnemyNodeData nodeData;
        PatrolNavigateTo navTo;

        int previousWP = -1;

        public PatrolNavToActivator(EnemyNodeData nodeData, PatrolNavigateTo navTo)
        {
            this.nodeData = nodeData;
            this.navTo = navTo;
        }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData() => nodeData;

        /// <summary>
        /// Call to check if the agent is in the currently set waypoint (through his nodeData).
        /// <para>If true, returns false.</para>
        /// <para>If false, call the Run method of the navTo (PatrolNavigateTo INode) and return its return value.</para>
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            if (!previousWP.Equals(nodeData.GetCurrentWaypointIndex()))
            {
                previousWP = nodeData.GetCurrentWaypointIndex();

                return navTo.Run();
            }

            return false;
        }
    }
}