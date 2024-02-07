using UnityEngine;

namespace WGRF.AI
{
    public class FallbackAction : INode
    {
        INodeData nodeData;

        public FallbackAction(INodeData nodeData)
        { this.nodeData = nodeData; }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData()
        { return nodeData; }

        /// <summary>
        /// Checks if the AI is a patroller through the NodeData passed.
        /// <para>If false, sets their walk animation state to false and returns true,
        /// else returns false.</para>
        /// </summary>
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