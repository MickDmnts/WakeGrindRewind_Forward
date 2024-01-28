using UnityEngine;
using UnityEngine.AI;

namespace WGRF.AI
{
    /* [Node Documentation]
     * 
     * [Custom Action]
     *  Controls the walk animation state based on the Idle on patrol time passed from the inspector.
     * 
     * [Must know]
     *  EnemyNodeData compatible.
     *  Passed to the IdlePatrolChaseSelector as a INode child.
     */
    public class IdleOnArrival : INode
    {
        EnemyNodeData nodeData;
        NavMeshAgent agent;

        float timer;
        float timerCache;

        public IdleOnArrival(EnemyNodeData nodeData)
        {
            //Set up basic node data.
            this.nodeData = nodeData;
            this.agent = nodeData.GetNavMeshAgent();

            //Set up the timers
            this.timer = nodeData.GetIdleTimeOnPatrol();
            this.timerCache = timer;
        }

        /// <summary>
        /// Call to get the node data passed in the creation of the node.
        /// </summary>
        public INodeData GetNodeData()
        {
            return nodeData;
        }

        /// <summary>
        /// Checks if enough time has passed based on the idle time while patrolling value.
        /// <para>If true, sets the walking animation state to true and un-freezes the agent. Returns true.</para>
        /// <para>If false, sets the walking animation state to false and freezes the agent. Returns false.</para>
        /// </summary>
        public bool Run()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                nodeData.GetEnemyAnimations().SetWalkStateAnimation(true);

                agent.isStopped = false;
                timer = timerCache;
                return true;
            }
            else
            {
                nodeData.GetEnemyAnimations().SetWalkStateAnimation(false);

                agent.isStopped = true;
                return false;
            }
        }
    }
}