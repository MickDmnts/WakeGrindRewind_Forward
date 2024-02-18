using UnityEngine;

namespace WGRF.AI
{
    /// <summary>
    /// The fallback action node of the agents
    /// </summary>
    public class FallbackAction : INode
    {
        ///<summary>The blackboard reference</summary>
        INodeData nodeData;

        ///<summary>Creates a FallbackAction instance</summary>
        public FallbackAction(INodeData nodeData)
        { this.nodeData = nodeData; }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData()
        { return nodeData; }

        public bool Run()
        {
            if (!(nodeData.EnemyEntity.EntityLife <= 10f / 100f * nodeData.EnemyEntity.EntityLife))
            { return false; }

            if (Random.Range(0f, 1f) <= 0.3f)
            {
                AIEntity entity = nodeData.EnemyEntity;
                entity.InitiateFallback(10);

                //This pseudo-deactivates the agents BT from updating.
                nodeData.IsDead = true;
                return true;
            }

            return false;
        }
    }
}