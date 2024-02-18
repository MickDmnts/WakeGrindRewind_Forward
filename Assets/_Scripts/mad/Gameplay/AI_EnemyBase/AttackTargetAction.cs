using System;
using UnityEngine;
using UnityEngine.AI;

namespace WGRF.AI
{
    /// <summary>
    /// Handles the attacking action of the agent
    /// </summary>
    public class AttackTargetAction : INode
    {
        ///<summary>The blackboard of this action</summary>
        INodeData nodeData;
        ///<summary>The agent of this action</summary>
        NavMeshAgent agent;
        ///<summary>The shoot method of this action</summary>
        Action shootMethod;

        /// <summary>
        /// Creates an AttackTargetAction 
        /// </summary>
        /// <param name="shootMethod">The method used for the attacking</param>
        public AttackTargetAction(INodeData nodeData, Action shootMethod)
        {
            this.nodeData = nodeData;
            this.shootMethod = shootMethod;

            this.agent = nodeData.EnemyEntity.Agent;
        }

        public INodeData GetNodeData()
        { return nodeData; }

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
            return true;
        }
    }
}