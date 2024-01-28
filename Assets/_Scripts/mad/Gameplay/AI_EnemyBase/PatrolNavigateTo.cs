using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace WGRF.AI
{
    /* [Node documentation]
     * 
     * [Custom Action]
     * Caches the waypoints in the node creation.
     * Navigates the agent to the currently nodeData set waypoint.
     * 
     * [Must know]
     *  EnemyNodeData compatible.
     */
    public class PatrolNavigateTo : INode
    {
        EnemyNodeData nodeData;
        List<Transform> points;
        NavMeshAgent agent;

        public PatrolNavigateTo(EnemyNodeData nodeData)
        {
            this.nodeData = nodeData;

            this.points = nodeData.GetWaypoints();
            this.agent = nodeData.GetNavMeshAgent();
        }

        public INodeData GetNodeData()
        {
            return nodeData;
        }

        /// <summary>
        /// Call to set the agent stoppin distance to 0, navigate him to the currently set waypoint (through his node data) 
        /// and set its walk animation state to true.
        /// <para>Always returns true.</para>
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            agent.stoppingDistance = 0f;
            agent.SetDestination(points[nodeData.GetCurrentWaypointIndex()].position);

            nodeData.GetEnemyAnimations().SetWalkStateAnimation(true);

            return true;
        }
    }
}