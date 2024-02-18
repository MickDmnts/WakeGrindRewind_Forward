using System.Collections.Generic;
using UnityEngine;
using WGRF.Core;

namespace WGRF.AI
{
    /// <summary>
    /// Action node responsible for finding the lowest hp enemy in the room
    /// </summary>
    public class FindMostVulnerable : INode
    {   
        ///<summary>The blackboard cache of the agent</summary>
        EnforcerNodeData nodeData;

        ///<summary>Creates a FindMostVulnerable instance</summary>
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
