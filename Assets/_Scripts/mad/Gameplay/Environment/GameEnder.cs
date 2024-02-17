using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WGRF.AI;

namespace WGRF.Core
{
    public class GameEnder : MonoBehaviour
    {
        void LateUpdate()
        {
            if(ManagerHub.S.ActiveRoom == (int)EnemyRoom.Room7)
            {
                if(ManagerHub.S.AIHandler.GetAliveAgentCount((int)EnemyRoom.Room7) <= 0)
                {
                    ManagerHub.S.HUDHandler.OpenScoreUI();
                    ManagerHub.S.HUDHandler.OpenMessageUI("Congratulations!\nYou won!");
                    ManagerHub.S.HUDHandler.OpenScoreboardUI();
                }
            }
        }
    }
}