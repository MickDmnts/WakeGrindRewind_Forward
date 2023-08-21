using System.Collections.Generic;

namespace WGR.Gameplay.AI
{
    /* [Node Documentation]
     * 
     * [Custom Selector]
     *  Used to change between Idle and Patrolling AI behaviour.
     */
    public class IdlePatrolChaseActivator : INode
    {
        EnemyNodeData nodeData;
        List<INode> children;

        public IdlePatrolChaseActivator(EnemyNodeData nodeData, List<INode> children)
        {
            this.nodeData = nodeData;

            this.children = children;
        }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData()
        {
            return nodeData;
        }

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
