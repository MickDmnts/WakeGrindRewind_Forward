using UnityEngine;

namespace WGRF.AI
{
    /// <summary>
    /// The enforcer protect vulnerable action node
    /// </summary>
    public class ProtectVulnerable : INode
    {
        ///<summary>The agent blackboard</summary>
        EnforcerNodeData nodeData;

        ///<summary>Creates a ProtectVulnerable instance</summary>
        public ProtectVulnerable(EnforcerNodeData nodeData)
        { this.nodeData = nodeData; }

        public INodeData GetNodeData()
        { return nodeData; }

        public bool Run()
        {
            Vector3 dest = (nodeData.Target.position + nodeData.LowestHpEnemy.Agent.transform.position) / 2f;
            nodeData.EnemyEntity.Agent.SetDestination(dest);

            Quaternion rotation = Quaternion.LookRotation(nodeData.Target.position - nodeData.EnemyEntity.transform.position);
            nodeData.EnemyEntity.transform.rotation = Quaternion.Slerp(nodeData.EnemyEntity.transform.rotation, rotation, 10f * Time.deltaTime);

            return true;
        }
    }
}
