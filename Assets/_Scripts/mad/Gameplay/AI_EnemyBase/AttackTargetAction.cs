using System;
using UnityEngine;
using UnityEngine.AI;

namespace WGRF.AI
{
    /* [Node documentation]
     * 
     * [Must know]
     *  INodeData compatible.
     */
    public class AttackTargetAction : INode
    {
        INodeData nodeData;

        NavMeshAgent agent;
        Transform target;

        Action shootMethod;

        public AttackTargetAction(INodeData nodeData, Action shootMethod)
        {
            this.nodeData = nodeData;
            this.shootMethod = shootMethod;

            this.agent = nodeData.GetNavMeshAgent();
            this.target = nodeData.GetTarget();
        }

        public INodeData GetNodeData()
        {
            return nodeData;
        }

        /// <summary>
        /// Call to continuously rotate the agent towards the Target (through nodeData) and call its Action passed method.
        /// <para>Always returns true.</para>
        /// </summary>
        public bool Run()
        {
            Quaternion rotation = Quaternion.LookRotation(target.position - agent.transform.position);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, rotation, 10f * Time.deltaTime);

            //Call the passed method
            shootMethod();
            return true;
        }
    }
}