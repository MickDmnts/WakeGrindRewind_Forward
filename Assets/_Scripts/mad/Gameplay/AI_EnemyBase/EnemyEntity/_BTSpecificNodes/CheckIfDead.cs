using WGRF.AI.Entities.Hostile.Generic;

namespace WGRF.AI.Nodes
{
    /* [Node Documentation]
     * 
     * [Custom switch]
     * If the agent is not marked as dead through the node data, update the INode passed.
     * 
     * [Must know]
     * EnemyNodeData compatible.
     */
    public class CheckIfDead : INode
    {
        EnemyNodeData nodeData;

        INode child;

        public CheckIfDead(EnemyNodeData nodeData, INode child)
        {
            this.nodeData = nodeData;
            this.child = child;
        }

        public INodeData GetNodeData()
        {
            return nodeData;
        }

        /// <summary>
        /// Call to update the INode passed in the node constructor ONLY if the agent is not dead.
        /// <para>Returns the INode passed return value.</para>
        /// <para>Returns false if none of the above apply.</para>
        /// </summary>
        public bool Run()
        {
            if (!nodeData.GetIsDead())
            {
                return child.Run();
            }

            return false;
        }
    }
}