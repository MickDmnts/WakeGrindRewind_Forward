using UnityEngine;
using UnityEngine.AI;

namespace WGRF.AI
{
    /* [Node documentation]
     * 
     * [Custom Selector]
     * Navigates the agent to the set target while checking if the target is behind an -OcclusionLayer- set gameObject. 
     * Returns false if the target can be shot directly, true otherwise.
     * 
     * [Must know]
     *  INodeData compatible.
     */
    public class NavToTarget : INode
    {
        INodeData nodeData;

        NavMeshAgent agent;
        Transform target;

        public NavToTarget(INodeData nodeData)
        {
            this.nodeData = nodeData;

            this.agent = nodeData.GetNavMeshAgent();
            this.target = nodeData.GetTarget();
        }

        public INodeData GetNodeData()
        {
            return nodeData;
        }

        /// <summary>
        /// Call to set the agent destination to the currenty set Target transfrom (through the node data).
        /// <para>When the target is inside attack range (nodeData.WeaponRange) the agent freezes and checks for occlusion
        /// based on the occlusion layers passed in node data. 
        /// <para>If the Occlusion check returns true, the agent unfreezes and the node returns true. Else, returns false without un-freezing.</para></para>
        /// <para>While the target is not inside attack range the node returns true.</para>
        /// </summary>
        public bool Run()
        {
            agent.SetDestination(target.position);

            Vector3 pos1 = agent.transform.position;
            Vector3 pos2 = target.position;

            if ((pos1 - pos2).magnitude <= nodeData.GetWeaponRange())
            {
                agent.isStopped = true;

                if (CheckForOcclusion())
                {
                    agent.isStopped = false;
                    return true;
                }

                return false;
            }

            agent.isStopped = false;
            return true;
        }

        /// <summary>
        /// Call to check if the target is behind a -OcclusionLayer- defined object.
        /// <para>If yes, returns true, false otherwise.</para>
        /// </summary>
        /// <returns></returns>
        bool CheckForOcclusion()
        {
            Vector3 occlLine = agent.transform.position;

            RaycastHit hitInfo;
            if (Physics.Linecast(occlLine, target.position, out hitInfo, nodeData.GetOcclusionLayers()))
            {
                Debug.DrawLine(occlLine, target.position, Color.red);
                return true;
            }
            return false;
        }
    }
}