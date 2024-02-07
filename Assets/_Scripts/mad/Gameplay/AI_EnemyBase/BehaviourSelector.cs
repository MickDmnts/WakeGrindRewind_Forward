using System.Collections.Generic;
using UnityEngine;

namespace WGRF.AI
{
    public class BehaviourSelector : INode
    {
        INodeData nodeData;
        List<INode> children;

        public BehaviourSelector(INodeData nodeData, List<INode> children)
        {
            this.nodeData = nodeData;
            this.children = children;
        }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData()
        { return nodeData; }

        /// <summary>
        /// Call to update the first branch that returns true from the passed list of INode children.
        /// </summary>
        public bool Run()
        {
            bool result = false;

            foreach (INode child in children)
            {
                if (child.Run())
                {
                    result = true;
                    break;
                }

                result = false;
            }

            return result;
        }
    }
}
