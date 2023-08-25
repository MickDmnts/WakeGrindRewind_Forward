using UnityEngine;
using UnityEngine.AI;
using WGR.Core;
using WGR.Core.Managers;

namespace WGR.AI.Nodes
{
    /* [Node documentation]
     * 
     * This node is responsible for sending the boss to his currently set hiding room ONLY when he's in hiding mode (isHiding = true).
     * When he arrives at the hiding room the node un-freezes the player.
     * 
     * [Must know]
     * 1. BossNodeData compatible.
     */
    public class NavToRoom : INode
    {
        BossNodeData bossNodeData;
        BossPhaseSelector bossPhaseSelector;

        NavMeshAgent bossAgent;

        public NavToRoom(BossNodeData bossNodeData, BossPhaseSelector phaseSelector)
        {
            this.bossNodeData = bossNodeData;
            this.bossPhaseSelector = phaseSelector;

            this.bossAgent = bossNodeData.GetNavMeshAgent();
        }

        public INodeData GetNodeData()
        {
            return bossNodeData;
        }

        /// <summary>
        /// Call to send the agent to his current set room waypoint when he is in hiding mode.
        /// </summary>
        /// <returns>True when he is in hiding mode and close to the waypoint.
        /// <para>Else runs the BossPhaseSelector and returns its return value.</para></returns>
        public bool Run()
        {
            if (bossNodeData.GetIsHiding())
            {
                bossAgent.speed = 15;
                bossAgent.isStopped = false;
                bossAgent.SetDestination(bossNodeData.GetCurrentHideRoom().position);
                bossNodeData.SetOriginalPos(bossNodeData.GetCurrentHideRoom().position);

                if (Vector3.Distance(bossAgent.transform.position, bossNodeData.GetCurrentHideRoom().position) <= 1f)
                {
                    bossNodeData.SetTargetFound(false);

                    if (GameManager.S != null)
                    {
                        GameManager.S.AIEntityManager.ActivateBossPlayerDetectors();
                        GameManager.S.PlayerEntity.IsActive = true;

                        GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.Unpause);
                    }

                    bossNodeData.SetIsHiding(false);
                    return true;
                }
            }

            return bossPhaseSelector.Run();
        }
    }
}