using UnityEngine;

namespace WGRF.AI
{
    public class CheckIfDead : INode
    {
        INodeData nodeData;
        INode child;

        public CheckIfDead(INodeData nodeData, INode child)
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
            if (!nodeData.IsDead)
            {
                return child.Run();
            }

            return false;
        }
    }
}