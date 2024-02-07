using UnityEngine;

namespace WGRF.AI
{
    public class EnforcerIdleNode : INode
    {
        EnforcerNodeData nodeData;

        public EnforcerIdleNode(INodeData nodeData)
        { this.nodeData = (EnforcerNodeData)nodeData; }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData()
        { return nodeData; }

        public bool Run()
        {            return !nodeData.CanProtect;        }
    }
}