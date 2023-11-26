using UnityEngine.AI;

namespace WGRF.AI.Nodes
{
    /* [Node Documentation]
     * 
     * [Custom Action]
     *  If the target (passed in the NodeData) is marked as dead, return the agent to his spawn position.
     *  Else run the INode node passed in the nextNode field.
     * 
     * [Must know]
     *  INodeData compatible.
     */
    public class NavToOriginalPos : INode
    {
        INodeData nodeData;
        INode nextNode;

        NavMeshAgent agent;

        public NavToOriginalPos(INodeData nodeData, INode nextNode)
        {
            this.nodeData = nodeData;
            this.nextNode = nextNode;

            this.agent = nodeData.GetNavMeshAgent();
        }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData()
        {
            return nodeData;
        }

        /// <summary>
        /// Call to check if the target is marked as dead.
        /// <para>If true, set the agents' current destination back to his spawn position and return true.</para>
        /// <para>If false, run the INode passed as nextNode and return its retun value.</para>
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            if (nodeData.GetTargetIsDead())
            {
                agent.SetDestination(nodeData.GetOriginalPos());
                nodeData.SetTargetIsDead(false);

                agent.isStopped = false;
                return true;
            }
            else
            {
                return nextNode.Run();
            }
        }
    }
}