using System.Collections.Generic;
using UnityEngine;
using WGRF.Core;

namespace WGRF.AI
{
    public class FindMostVulnerable : INode
    {
        EnforcerNodeData nodeData;

        public FindMostVulnerable(EnforcerNodeData nodeData)
        { this.nodeData = nodeData; }

        public INodeData GetNodeData()
        { return nodeData; }

        public bool Run()
        {
            List<AIEntity> agents = ManagerHub.S.AIHandler.GetRoomAgents((int)nodeData.EnemyEntity.EnemyRoom);
            float life = Mathf.Infinity;
            AIEntity mostVulnerable = null;

            for (int i = 0; i < agents.Count; i++)
            {
                if (agents[i].EntityLife < life)
                {
                    if (agents[i] == nodeData.EnemyEntity)
                    { continue; }

                    mostVulnerable = agents[i];
                }
            }

            nodeData.LowestHpEnemy = mostVulnerable;

            return false;
        }
    }
}
