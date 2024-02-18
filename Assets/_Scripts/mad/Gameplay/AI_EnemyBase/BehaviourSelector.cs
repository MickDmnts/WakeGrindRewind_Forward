using System.Collections.Generic;

namespace WGRF.AI
{
    public class BehaviourSelector : INode
    {
        ///<summary>The blackboard of this action</summary>
        INodeData nodeData;
        ///<summary>The selector children</summary>
        List<INode> children;

        /// <summary>
        /// Creates a BehaviourSelector instance
        /// </summary>
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
