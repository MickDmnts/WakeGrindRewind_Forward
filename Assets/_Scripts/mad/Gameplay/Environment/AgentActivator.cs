using UnityEngine;

using WGRF.AI;
using WGRF.Core;

namespace WGRF.Interactions
{
    public class AgentActivator : MonoBehaviour
    {
        ///<summary>The room this activator belongs to.</summary>
        [SerializeField] EnemyRoom currentRoom;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (currentRoom == EnemyRoom.Room1)
                { ManagerHub.S.GameSoundsHandler.PlayLoop(); }

                ManagerHub.S.InternalTime.ResetRoomTimer();
                ManagerHub.S.InternalTime.StartRoomTimer();
                ManagerHub.S.SetActiveRoom(currentRoom);
                ManagerHub.S.AIHandler.ActivateAgents(currentRoom);

                Destroy(gameObject);
            }
        }
    }
}