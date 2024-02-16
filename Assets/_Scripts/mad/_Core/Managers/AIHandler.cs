using System;
using System.Collections.Generic;
using UnityEngine;
using WGRF.AI;

namespace WGRF.Core
{
    /// <summary>
    /// Thsi handler is responsible for handling all the active/inactive agents in the game.
    /// </summary>
    public class AIHandler : CoreBehaviour
    {
        ///<summary>Caches the active agents based on their assigned rooms</summary>
        Dictionary<int, List<AIEntity>> registeredAgents;

        protected override void PreAwake()
        {
            registeredAgents = new Dictionary<int, List<AIEntity>>();
            int length = Enum.GetNames(typeof(EnemyRoom)).Length;
            for (int i = 0; i < length; i++)
            { registeredAgents.Add(i, new List<AIEntity>()); }
        }

        /// <summary>
        /// Activates the agents of the passed room.
        /// </summary>
        /// <param name="room">The room</param>
        public void ActivateAgents(EnemyRoom room)
        {
            foreach (AIEntity agent in registeredAgents[(int)room])
            { agent.SetIsAgentActive(true); }
        }

        /// <summary>
        /// Deactivates the agents of the passed room.
        /// </summary>
        public void DeactivateAllAgents()
        {
            foreach (KeyValuePair<int, List<AIEntity>> pair in registeredAgents)
            {
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    pair.Value[i].SetIsAgentActive(false);
                }
            }
        }

        public List<AIEntity> GetRoomAgents(int room)
        { return registeredAgents[room]; }

        /// <summary>
        /// Registers an agent at the passed room
        /// </summary>
        public void RegisterAgent(EnemyRoom room, AIEntity agent)
        { registeredAgents[(int)room].Add(agent); }

        /// <summary>
        /// Removes an agent from a room
        /// </summary>
        public void RemoveAgent(EnemyRoom room, AIEntity agent)
        { registeredAgents[(int)room].Remove(agent); }

        /// <summary>
        /// Returns the passed room agent count
        /// </summary>
        /// <param name="room">The requested room</param>
        public int GetRoomAgentCount(int room)
        { return registeredAgents[(int)room].Count; }

        /// <summary>
        /// Returns the alive agent count from the passed room index.
        /// </summary>
        public int GetAliveAgentCount(int room)
        {
            int cnt = 0;

            for (int i = 0; i < registeredAgents[(int)room].Count; i++)
            {
                if (!registeredAgents[(int)room][i].IsDead)
                { cnt++; }
            }

            return cnt;
        }

        void LateUpdate()
        {
            if (GetRoomAgentCount((int)EnemyRoom.Room7) <= 0)
            { ManagerHub.S.HUDHandler?.OpenScoreUI(); }
        }
    }
}