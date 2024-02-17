using UnityEngine;
using WGRF.AI;

namespace WGRF.Core
{
    public class GameEnder : MonoBehaviour
    {
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