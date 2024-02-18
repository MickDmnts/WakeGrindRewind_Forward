using UnityEngine;
using WGRF.AI;

namespace WGRF.Core
{
    /// <summary>
    /// Enables the score UI when the final room is cleared
    /// </summary>
    public class GameEnder : MonoBehaviour
    {   
        ///<summary>Has the game ended?</summary>
        bool hasEnded = false;

        void LateUpdate()
        {
            if (ManagerHub.S.ActiveRoom == (int)EnemyRoom.Room7 && !hasEnded)
            {
                if (ManagerHub.S.AIHandler.GetAliveAgentCount((int)EnemyRoom.Room7) <= 0)
                {
                    ManagerHub.S.HUDHandler.OpenScoreUI();
                    hasEnded = true;
                }
            }
        }
    }
}