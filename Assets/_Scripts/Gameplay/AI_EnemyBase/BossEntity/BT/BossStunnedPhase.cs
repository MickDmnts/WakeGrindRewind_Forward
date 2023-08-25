using System;
using UnityEngine;
using UnityEngine.AI;

using WGR.Core;
using WGR.Core.Managers;

namespace WGR.AI.Nodes
{
    /* [Node documentation]
     * 
     * This node sends the boss back to his final room position and calls the callbackOnStunEnd Action when he reaches it.
     * 
     */
    public class BossStunnedPhase : INode
    {
        BossNodeData bossNodeData;

        NavMeshAgent bossAgent;
        Action callbackOnStunEnd;

        public BossStunnedPhase(BossNodeData bossNodeData, Action callbackOnStunEnd)
        {
            this.bossNodeData = bossNodeData;

            this.bossAgent = bossNodeData.GetNavMeshAgent();
            this.callbackOnStunEnd = callbackOnStunEnd;
        }

        public INodeData GetNodeData()
        {
            return bossNodeData;
        }

        /// <summary>
        /// Sends the boss back to his last room position, if he's nearing his room position
        /// the callbackOnStunEnd Action gets called.
        /// </summary>
        /// <returns>False if the boss is nto stunned, true when he is stunned and in his room position.</returns>
        public bool Run()
        {
            if (bossNodeData.GetIsStunned())
            {
                bossAgent.speed = 15;
                bossAgent.isStopped = false;
                bossAgent.SetDestination(bossNodeData.GetCurrentHideRoom().position);
                bossNodeData.SetOriginalPos(bossNodeData.GetCurrentHideRoom().position);

                if (Vector3.Distance(bossAgent.transform.position, bossNodeData.GetCurrentHideRoom().position) <= 1f)
                {
                    if (GameManager.S != null)
                    {
                        GameManager.S.PlayerEntity.IsActive = true;

                        GameManager.S.GameSoundsHandler.PlayOneShot(GameAudioClip.Unpause);
                    }

                    callbackOnStunEnd();
                }

                return true;
            }

            return false;
        }
    }
}