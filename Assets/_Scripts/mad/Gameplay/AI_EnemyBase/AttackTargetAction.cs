using System;
using UnityEngine;
using UnityEngine.AI;

namespace WGRF.AI
{
    public class AttackTargetAction : INode
    {
        INodeData nodeData;

        NavMeshAgent agent;

        Action shootMethod;

        public AttackTargetAction(INodeData nodeData, Action shootMethod)
        {
            this.nodeData = nodeData;
            this.shootMethod = shootMethod;

            this.agent = nodeData.EnemyEntity.Agent;
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
            Quaternion rotation = Quaternion.LookRotation(nodeData.Target.position - agent.transform.position);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, rotation, 10f * Time.deltaTime);

            //Call the passed method
            shootMethod();
            Debug.Log("Attack action");
            return true;
        }
    }
}