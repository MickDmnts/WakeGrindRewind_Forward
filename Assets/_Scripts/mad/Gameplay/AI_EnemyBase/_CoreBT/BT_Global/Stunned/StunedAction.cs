using UnityEngine;
using UnityEngine.AI;

namespace WGRF.AI.Nodes
{
    /* [Node documentation]
     * 
     * [Custom Action]
     *  Countdowns from the moment the enemy got marked as Stunned and when the countdown finishes it resets the needed fields of the agent.
     *  If the agent is still alive, sets it TargetFound value to true so the AI can attack the player right after he gets un-stunned.
     * 
     * [Must know]
     *  EnemyNodeData compatible.
     */
    public class StunedAction : INode
    {
        EnemyNodeData nodeData;

        NavMeshAgent agent;
        float timer;

        public StunedAction(EnemyNodeData nodeData)
        {
            this.nodeData = nodeData;

            this.agent = nodeData.GetNavMeshAgent();

            ResetStunTimer();
        }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData()
        {
            return nodeData;
        }

        /// <summary>
        /// Call check if enough time has passed since the agent got marked as stunned.
        /// <para>If true, set the stunned animation state, along with the enemy entity and node data stunned values to false.</para>
        /// <para>If the agent is still alive activate his attacking behaviour by setting the TargetFound value to true through the node data.</para>
        /// </summary>
        /// <returns>False when the time reaches 0, True when is greater than 0.</returns>
        public bool Run()
        {
            agent.isStopped = true;
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                agent.isStopped = false;

                ResetStunTimer();

                nodeData.GetEnemyAnimations().SetStunnedAnimationState(false);

                nodeData.GetEnemyEntity().IsStunned = false;
                nodeData.SetIsStunned(false);

                if (nodeData.GetAttackAfterStun())
                {
                    nodeData.SetTargetFound(true);
                }

                return false;
            }

            return true;
        }

        void ResetStunTimer()
        {
            timer = 2f;
        }
    }
}