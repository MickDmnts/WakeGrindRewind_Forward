using System.Collections.Generic;
using UnityEngine;

namespace WGRF.AI
{
    /* [Node Documentation]
     * 
     * [Custom Selector]
     *  Used to change between Idle and Patrolling AI behaviour.
     */
    public class BehaviourSelector : INode
    {
        EnemyNodeData nodeData;
        List<INode> children;

        public BehaviourSelector(INodeData nodeData, List<INode> children)
        {
            this.nodeData = (EnemyNodeData)nodeData;
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
                    Debug.Log(child);
                    result = true;
                    break;
                }

                result = false;
            }

            return result;
        }
    }
}
