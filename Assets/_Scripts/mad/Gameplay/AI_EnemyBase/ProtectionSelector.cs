using UnityEngine;
using WGRF.Core;

namespace WGRF.AI
{
    public class ProtectionSelector : INode
    {
        EnforcerNodeData nodeData;

        FindMostVulnerable lowestHp;
        ProtectVulnerable protectNode;

        public ProtectionSelector(FindMostVulnerable lowestHp, ProtectVulnerable protectNode, EnforcerNodeData nodeData)
        {
            this.nodeData = nodeData;
            this.lowestHp = lowestHp;
            this.protectNode = protectNode;
        }

        public INodeData GetNodeData()
        {
            return nodeData;
        }

        /// <summary>
        /// If the NavToTarget Run method returns false, calls and returns the AttackTargetAction Run method value.
        /// <para>Returns false if none of the above apply.</para>
        /// </summary>
        public bool Run()
        {
            if (nodeData.CanProtect)
            {
                //Continues to next baheviour when all enemies are dead.
                if (ManagerHub.S.AIHandler.GetAliveAgentCount((int)nodeData.EnemyEntity.EnemyRoom) <= 0)
                { return false; }

                if (!lowestHp.Run())
                { return protectNode.Run(); }

                return true;
            }

            return false;
        }
    }
}
