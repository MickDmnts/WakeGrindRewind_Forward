using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace WGR.AI.Nodes
{
    /* [Node documentation]
     * 
     * [Custom Action]
     * Caches the waypoints in the node creation.
     * Checks if the agent is nearing the currently set waypoint based on the node data set offset float and
     *  increases the currently set waypoint index. Calls the IdleOnArrival Run method when the agent reaches the waypoint.
     * 
     * [Must know]
     *  EnemyNodeData compatible.
     */
    public class PatrolCheckDistance : INode
    {
        EnemyNodeData nodeData;
        IdleOnArrival idleOnArrival;

        NavMeshAgent agent;
        List<Transform> points;

        int currentTargetIndex = 0;

        public PatrolCheckDistance(EnemyNodeData nodeData, IdleOnArrival idleNode)
        {
            this.nodeData = nodeData;
            this.idleOnArrival = idleNode;

            this.agent = nodeData.GetNavMeshAgent();
            this.points = nodeData.GetWaypoints();
        }

        public INodeData GetNodeData()
        {
            return nodeData;
        }

        /// <summary>
        /// Call to check if the agent is nearing the currently set waypoint based on the node data set offset float.
        /// <para>When true, run the idleNode (IdleOnArrival INode) and return its return value. Increments the current waypoint index.</para>
        /// <para>When the idleNode returns false, THIS node returns false.</para>
        /// </summary>
        public bool Run()
        {
            currentTargetIndex = nodeData.GetCurrentWaypointIndex();

            float distance = Vector3.Distance(agent.transform.position, points[currentTargetIndex].position);

            if (distance <= nodeData.GetWaypointDistanceOffset())
            {
                currentTargetIndex++;

                if (currentTargetIndex >= points.Count)
                {
                    currentTargetIndex = 0;
                }

                if (idleOnArrival.Run())
                {
                    nodeData.SetCurrentWaypointIndex(currentTargetIndex);
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }
}
